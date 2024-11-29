using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using ScottishGlenAssetTracking.Data;
using ScottishGlenAssetTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.Services
{
    /// <summary>
    /// SoftwareAssetService class used to interact with the database for the SoftwareAsset entity.
    /// </summary>
    public class SoftwareAssetService
    {
        // Private field for the ScottishGlenContext.
        private readonly ScottishGlenContext _context;

        /// <summary>
        /// Constructor for the SoftwareAssetService class.
        /// </summary>
        /// <param name="context">ScottishGlenContext to be injected using dependency injection.</param>
        public SoftwareAssetService(ScottishGlenContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Method to add a SoftwareAsset to the database.
        /// </summary>
        /// <param name="softwareAsset">SoftwareAsset to be added to the database.</param>
        /// <returns>True if added to the database, false if not.</returns>
        public bool AddSoftwareAsset(SoftwareAsset softwareAsset)
        {
            ///// Logic for existing SoftwareAsset

            if (DoesSoftwareAssetExist(softwareAsset))
            {
                LinkHardwareAssetsToSoftwareAsset(softwareAsset);
                return true;
            }

            ///// Logic for new SoftwareAsset

            // Set the state of the HardwareAssets to Unchanged to prevent adding new HardwareAssets, and attach them to the context if there are any.
            if (softwareAsset.HardwareAssets != null)
            {
                foreach (var hardwareAsset in softwareAsset.HardwareAssets)
                {
                    HardwareAsset trackedHardwareAsset = _context.HardwareAssets.Local.FirstOrDefault(a => a.Id == hardwareAsset.Id);

                    if (trackedHardwareAsset == null)
                    {
                        _context.HardwareAssets.Attach(hardwareAsset);
                        _context.Entry(trackedHardwareAsset).State = EntityState.Unchanged;
                    }
                }
            }

            // Add the SoftwareAsset to the database and save the changes.
            _context.SoftwareAssets.Add(softwareAsset);

            _context.SaveChanges();

            // Return true if the SoftwareAsset was added to the database.
            return true;
        }

        /// <summary>
        /// Method to get a SoftwareAsset from the database by its Id.
        /// </summary>
        /// <param name="softwareAssetId">Id of the SoftwareAsset to be retrieved.</param>
        /// <returns>SoftwareAsset from the database with the chosen Id.</returns>
        public SoftwareAsset GetSoftwareAsset(int softwareAssetId)
        {
            // Return the SoftwareAsset from the database with the chosen Id.
            return _context.SoftwareAssets.Include(a => a.HardwareAssets).FirstOrDefault(a => a.Id == softwareAssetId);
        }

        /// <summary>
        /// Method to get all SoftwareAssets from the database.
        /// </summary>
        /// <returns>List of the SoftwareAssets from the database.</returns>
        public List<SoftwareAsset> GetSoftwareAssets()
        {
            // Return the list of SoftwareAssets from the database.
            return _context.SoftwareAssets.Include(a => a.HardwareAssets).ToList();
        }

        /// <summary>
        /// Method to update an SoftwareAsset in the database.
        /// </summary>
        /// <param name="softwareAsset">SoftwareAsset to be updated in the database.</param>
        /// <returns>True if updated in the database, false if not.</returns>
        public bool UpdateSoftwareAsset(SoftwareAsset softwareAsset)
        {
            // Check if a software asset with the same name and version already exists.
            var existingSoftwareAsset = _context.SoftwareAssets
                                                .Include(sa => sa.HardwareAssets)
                                                .FirstOrDefault(sa => sa.Name == softwareAsset.Name && sa.Version == softwareAsset.Version);

            // Check if the software asset already exists and is not the same as the provided software asset.
            if (existingSoftwareAsset != null && existingSoftwareAsset.Id != softwareAsset.Id)
            {
                // Attach hardware assets to the existing software asset.
                foreach (var hardwareAsset in softwareAsset.HardwareAssets)
                {
                    var existingHardwareAsset = _context.HardwareAssets.Find(hardwareAsset.Id)
                                              ?? _context.HardwareAssets.Attach(hardwareAsset).Entity;

                    existingHardwareAsset.SoftwareAsset = existingSoftwareAsset;
                    existingHardwareAsset.SoftwareLinkDate = DateTime.Now;
                }

                // Delete the provided software asset (the "old" one) from the database.
                if (_context.SoftwareAssets.Local.Contains(softwareAsset))
                {
                    _context.SoftwareAssets.Remove(softwareAsset);
                }
                else
                {
                    _context.Entry(softwareAsset).State = EntityState.Deleted;
                }
            }
            else
            {
                _context.SoftwareAssets.Update(softwareAsset);
            }

            // Save changes to the database.
            _context.SaveChanges();
            return true;
        }


        /// <summary>
        /// Method to delete a SoftwareAsset from the database.
        /// </summary>
        /// <param name="softwareAssetId">Id of the SoftwareAsset to be deleted from the database.</param>
        /// <returns>True if deleted from the database, false if not.</returns>
        public bool DeleteSoftwareAsset(int softwareAssetId)
        {
            // Find the SoftwareAsset in the database with the chosen Id.
            var softwareAsset = _context.SoftwareAssets.Find(softwareAssetId);

            // Remove the SoftwareAsset from the database and save the changes.
            _context.SoftwareAssets.Remove(softwareAsset);
            _context.SaveChanges();

            // Return true if the SoftwareAsset was deleted from the database.
            return true;
        }

        /// <summary>
        /// Method to create a SoftwareAsset with automatically retrieved system information.
        /// </summary>
        /// <returns>SoftwareAsset with the system information.</returns>
        public SoftwareAsset GetSoftwareAssetWithSystemInfo()
        {
            // Initialize the name, version, and manufacturer of the system with "Unknown" in case of failure.
            string name = "Unknown";
            string version = "Unknown";
            string manufacturer = "Unknown";

            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
            {
                // Iterate through the ManagementObjectSearcher to retrieve the system information.
                foreach (ManagementObject obj in searcher.Get())
                {
                    name = obj["Caption"]?.ToString() ?? "Unknown";
                    version = obj["Version"]?.ToString() ?? "Unknown";
                    manufacturer = obj["Manufacturer"]?.ToString() ?? "Unknown";
                }
            }

            // Return the SoftwareAsset with the system information.
            return new SoftwareAsset
            {
                Name = name,
                Version = version,
                Manufacturer = manufacturer
            };
        }

        /// <summary>
        /// Method to get an existing SoftwareAsset from the database by its name and version.
        /// </summary>
        /// <param name="name">Name of the software.</param>
        /// <param name="version">Version of the software.</param>
        /// <returns>SoftwareAsset from the database with the provided name and version.</returns>
        public SoftwareAsset GetExistingSoftwareAsset(string name, string version) => _context.SoftwareAssets.Include(a => a.HardwareAssets).FirstOrDefault(a => a.Name == name && a.Version == version);

        /// <summary>
        /// Method to check if a SoftwareAsset already exists in the database.
        /// </summary>
        /// <param name="softwareAsset">SoftwareAsset to check if it already exists in the database.</param>
        /// <returns>True if the SoftwareAsset already exists in the database, false if not.</returns>
        private bool DoesSoftwareAssetExist(SoftwareAsset softwareAsset)
        {
            // Check if the SoftwareAsset already exists in the database.
            SoftwareAsset existingSoftwareAsset = GetExistingSoftwareAsset(softwareAsset.Name, softwareAsset.Version);

            // Return true if the SoftwareAsset already exists in the database, false if not.
            return existingSoftwareAsset != null;
        }

        /// <summary>
        /// Helper method to link HardwareAssets to an existing SoftwareAsset in the database.
        /// </summary>
        /// <param name="softwareAsset">SoftwareAsset to link the HardwareAssets to.</param>
        /// <returns>True if linked, false if not.</returns>
        private bool LinkHardwareAssetsToSoftwareAsset(SoftwareAsset softwareAsset)
        {
            // Get the existing SoftwareAsset from the database.
            SoftwareAsset existingSoftwareAsset = GetExistingSoftwareAsset(softwareAsset.Name, softwareAsset.Version);

            // Set each HardwareAssets's SoftwareAsset to the existing SoftwareAsset in the database if there are any.
            if (softwareAsset.HardwareAssets != null)
            {
                foreach (var hardwareAsset in softwareAsset.HardwareAssets)
                {
                    HardwareAsset existingHardwareAsset = _context.HardwareAssets.Find(hardwareAsset.Id);
                    existingHardwareAsset.SoftwareAsset = existingSoftwareAsset;
                    existingHardwareAsset.SoftwareLinkDate = DateTime.Now;
                    _context.HardwareAssets.Attach(existingHardwareAsset);
                }
            }

            if (_context.SoftwareAssets.Contains(softwareAsset))
            {
                _context.SoftwareAssets.Remove(softwareAsset);
            }

            // Save the changes to the database and return true.
            _context.SaveChanges();
            return true;
        }
    }
}
