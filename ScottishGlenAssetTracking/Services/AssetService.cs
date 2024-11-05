using ScottishGlenAssetTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScottishGlenAssetTracking.Data;
using System.Management;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace ScottishGlenAssetTracking.Services
{
    /// <summary>
    /// AssetService class used to interact with the database for the Asset entity.
    /// </summary>
    public class AssetService
    {
        // Private field for the ScottishGlenContext.
        private readonly ScottishGlenContext _context;

        /// <summary>
        /// Constructor for the AssetService class.
        /// </summary>
        /// <param name="context">ScottishGlenContext to be injected using dependency injection.</param>
        public AssetService(ScottishGlenContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Method to add an Asset to the database.
        /// </summary>
        /// <param name="asset">Asset to be added to the database.</param>
        /// <returns>True if added to the database, false if not.</returns>
        public bool AddAsset(Asset asset)
        {
            // Set the state of the Employee to Unchanged to prevent adding a new Employee.
            _context.Entry(asset.Employee).State = EntityState.Unchanged;

            // Add the Asset to the database, attach the Employee and save the changes.
            _context.Assets.Add(asset);
            _context.Employees.Attach(asset.Employee);
            _context.SaveChanges();

            // Return true if the Asset was added to the database.
            return true;
        }

        /// <summary>
        /// Method to get an Asset from the database by its Id.
        /// </summary>
        /// <param name="assetId">Id of the Asset to be retrieved.</param>
        /// <returns>Asset from the database with the chosen Id.</returns>
        public Asset GetAsset(int assetId)
        {
            // Return the Asset from the database with the chosen Id.
            return _context.Assets.Include(a => a.Employee).ThenInclude(e => e.Department).FirstOrDefault(a => a.Id == assetId);
        }

        /// <summary>
        /// Method to get all Assets from a specific Employee from the database.
        /// </summary>
        /// <param name="employeeId">Id of the Employee to retrieve the assets for.</param>
        /// <returns>List of the Assets from the database for the chosen Employee.</returns>
        public List<Asset> GetAssets(int employeeId)
        {
            // Return the list of Assets from the database for the chosen Employee.
            return _context.Assets.Include(a => a.Employee).ThenInclude(e => e.Department).Where(a => a.Employee.Id == employeeId).ToList();
        }

        /// <summary>
        /// Method to update an Asset in the database.
        /// </summary>
        /// <param name="asset">Asset to be updated in the database.</param>
        /// <returns>True if updated in the database, false if not.</returns>
        public bool UpdateAsset(Asset asset)
        {
            // Set the state of the Employee to Unchanged to prevent adding a new Employee.
            _context.Entry(asset.Employee).State = EntityState.Unchanged;

            // Update the Asset in the database and save the changes.
            _context.Assets.Update(asset);
            _context.SaveChanges();

            // Return true if the Asset was updated in the database.
            return true;
        }

        /// <summary>
        /// Method to delete an asset from the database.
        /// </summary>
        /// <param name="assetId">Id of the Asset to be deleted from the database.</param>
        /// <returns>True if deleted from the database, false if not.</returns>
        public bool DeleteAsset(int assetId)
        {
            // Find the Asset in the database with the chosen Id.
            var asset = _context.Assets.Find(assetId);

            // Remove the Asset from the database and save the changes.
            _context.Assets.Remove(asset);
            _context.SaveChanges();

            // Return true if the Asset was deleted from the database.
            return true;
        }

        /// <summary>
        /// Method to create an Asset with automatically retrieved system information.
        /// </summary>
        /// <returns>Asset with the system information.</returns>
        public Asset GetAssetWithSystemInfo()
        {
            // Retrieve the name of the system.
            string name = Environment.MachineName;

            // Initialize the manufacturer, model, and type of the system with "Unknown" in case of failure.
            string manufacturer = "Unknown";
            string model = "Unknown";
            string type = "Unknown";


            // Retrieve the manufacturer, model, and type of the system using ManagementObjectSearcher.
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem"))
            {
                // Iterate through the ManagementObjectSearcher to retrieve the system information.
                foreach (ManagementObject obj in searcher.Get())
                {
                    // Set the manufacturer, model, and type of the system to the retrieved values or "Unknown" if null.
                    manufacturer = obj["Manufacturer"]?.ToString() ?? "Unknown";
                    model = obj["Model"]?.ToString() ?? "Unknown";
                    type = obj["SystemType"]?.ToString() ?? "Unknown";
                }
            }

            // Retrieve the IP address of the system or set it to "No IP Found" if not found.
            string ipAddress = Dns.GetHostEntry(Dns.GetHostName())
               .AddressList
               .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
               ?.ToString() ?? "No IP Found";

            // Return the Asset with the retrieved system information.
            return new Asset
            {
                Name = name,
                Model = model,
                Manufacturer = manufacturer,
                Type = type,
                IpAddress = ipAddress,
            };
        }
    }
}
