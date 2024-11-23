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
    /// HardwareAssetService class used to interact with the database for the HardwareAsset entity.
    /// </summary>
    public class HardwareAssetService
    {
        // Private field for the ScottishGlenContext.
        private readonly ScottishGlenContext _context;

        /// <summary>
        /// Constructor for the HardwareAssetService class.
        /// </summary>
        /// <param name="context">ScottishGlenContext to be injected using dependency injection.</param>
        public HardwareAssetService(ScottishGlenContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Method to add a HardwareAsset to the database.
        /// </summary>
        /// <param name="hardwareAsset">HardwareAsset to be added to the database.</param>
        /// <returns>True if added to the database, false if not.</returns>
        public bool AddHardwareAsset(HardwareAsset hardwareAsset)
        {
            // Set the state of the Employee to Unchanged to prevent adding a new Employee.
            _context.Entry(hardwareAsset.Employee).State = EntityState.Unchanged;

            // Add the HardwareAsset to the database, attach the Employee and save the changes.
            _context.HardwareAssets.Add(hardwareAsset);
            _context.Employees.Attach(hardwareAsset.Employee);
            _context.SaveChanges();

            // Return true if the HardwareAsset was added to the database.
            return true;
        }

        /// <summary>
        /// Method to get a HardwareAsset from the database by its Id.
        /// </summary>
        /// <param name="hardwareAssetId">Id of the HardwareAsset to be retrieved.</param>
        /// <returns>HardwareAsset from the database with the chosen Id.</returns>
        public HardwareAsset GetHardwareAsset(int hardwareAssetId)
        {
            // Return the HardwareAsset from the database with the chosen Id.
            return _context.HardwareAssets.Include(a => a.Employee).ThenInclude(e => e.Department).FirstOrDefault(a => a.Id == hardwareAssetId);
        }

        /// <summary>
        /// Method to get all HardwareAssets from a specific Employee from the database.
        /// </summary>
        /// <param name="employeeId">Id of the Employee to retrieve the assets for.</param>
        /// <returns>List of the HardwareAssets from the database for the chosen Employee.</returns>
        public List<HardwareAsset> GetHardwareAssets(int employeeId)
        {
            // Return the list of HardwareAssets from the database for the chosen Employee.
            return _context.HardwareAssets.Include(a => a.Employee).ThenInclude(e => e.Department).Where(a => a.Employee.Id == employeeId).ToList();
        }

        /// <summary>
        /// Method to get all HardwareAssets from the database.
        /// </summary>
        /// <returns>List of the HardwareAssets from the database.</returns>
        public List<HardwareAsset> GetAllHardwareAssets()
        {
            // Return the list of HardwareAssets from the database.
            return _context.HardwareAssets.Include(a => a.Employee).ThenInclude(e => e.Department).ToList();
        }

        /// <summary>
        /// Method to update an HardwareAsset in the database.
        /// </summary>
        /// <param name="hardwareAsset">HardwareAsset to be updated in the database.</param>
        /// <returns>True if updated in the database, false if not.</returns>
        public bool UpdateHardwareAsset(HardwareAsset hardwareAsset)
        {
            // Set the state of the Employee to Unchanged to prevent adding a new Employee.
            _context.Entry(hardwareAsset.Employee).State = EntityState.Unchanged;

            // Update the HardwareAsset in the database and save the changes.
            _context.HardwareAssets.Update(hardwareAsset);
            _context.SaveChanges();

            // Return true if the HardwareAsset was updated in the database.
            return true;
        }

        /// <summary>
        /// Method to delete a HardwareAsset from the database.
        /// </summary>
        /// <param name="hardwareAssetId">Id of the HardwareAsset to be deleted from the database.</param>
        /// <returns>True if deleted from the database, false if not.</returns>
        public bool DeleteHardwareAsset(int hardwareAssetId)
        {
            // Find the HardwareAsset in the database with the chosen Id.
            var asset = _context.HardwareAssets.Find(hardwareAssetId);

            // Remove the HardwareAsset from the database and save the changes.
            _context.HardwareAssets.Remove(asset);
            _context.SaveChanges();

            // Return true if the HardwareAsset was deleted from the database.
            return true;
        }

        /// <summary>
        /// Method to create a HardwareAsset with automatically retrieved system information.
        /// </summary>
        /// <returns>HardwareAsset with the system information.</returns>
        public HardwareAsset GetHardwareAssetWithSystemInfo()
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

            // Return the HardwareAsset with the retrieved system information.
            return new HardwareAsset
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
