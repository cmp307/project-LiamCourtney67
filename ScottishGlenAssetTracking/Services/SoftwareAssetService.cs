using Microsoft.EntityFrameworkCore;
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
            // Check if the SoftwareAsset already exists in the database.
            if (_context.SoftwareAssets.Any(a => a.Name == softwareAsset.Name && a.Version == softwareAsset.Version))
            {
                // Set the SoftwareAsset to the existing SoftwareAsset in the database.
                foreach (var hardwareAsset in softwareAsset.HardwareAssets)
                {
                    HardwareAsset existingHardwareAsset = _context.HardwareAssets.FirstOrDefault(a => a.Id == hardwareAsset.Id);
                    existingHardwareAsset.SoftwareAsset = softwareAsset;
                    existingHardwareAsset.SoftwareLinkDate = DateTime.Now;
                }

                // Save the changes to the database and return true.
                _context.SaveChanges();
                return true;
            }

            // Set the state of the HardwareAssets to Unchanged to prevent adding new HardwareAssets.
            foreach (var hardwareAsset in softwareAsset.HardwareAssets)
            {
                _context.Entry(hardwareAsset).State = EntityState.Unchanged;
            }

            // Add the SoftwareAsset to the database, attach the HardwareAssets and save the changes.
            _context.SoftwareAssets.Add(softwareAsset);

            foreach (var hardwareAsset in softwareAsset.HardwareAssets)
            {
                _context.HardwareAssets.Attach(hardwareAsset);
            }

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
            // Set the state of the HardwareAssets to Unchanged to prevent adding new HardwareAssets.
            foreach (var hardwareAsset in softwareAsset.HardwareAssets)
            {
                _context.Entry(hardwareAsset).State = EntityState.Unchanged;
            }

            // Update the SoftwareAsset in the database and save the changes.
            _context.SoftwareAssets.Update(softwareAsset);
            _context.SaveChanges();

            // Return true if the SoftwareAsset was updated in the database.
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
            var asset = _context.SoftwareAssets.Find(softwareAssetId);

            // Remove the SoftwareAsset from the database and save the changes.
            _context.SoftwareAssets.Remove(asset);
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
            // Create a new SoftwareAsset.
            var softwareAsset = new SoftwareAsset();

            // Get the system information for the SoftwareAsset.
            softwareAsset.Name = Environment.OSVersion.Platform.ToString();
            softwareAsset.Version = Environment.OSVersion.Version.ToString();

            // Set the manufacturer based on the OS, since this is only on Windows, Microsoft should be the only manufacturer, but add a check for unknown in case.
            if (Environment.OSVersion.VersionString.Contains("Microsoft"))
            {
                softwareAsset.Manufacturer = "Microsoft";
            }
            else
            {
                softwareAsset.Manufacturer = "Unknown";
            }

            // Return the SoftwareAsset with the system information.
            return softwareAsset;
        }

        // TODO: Add method to check for vulnerabilities in software assets.
    }
}
