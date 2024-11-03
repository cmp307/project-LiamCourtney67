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
    public class AssetService
    {
        private readonly ScottishGlenContext _context;

        public AssetService(ScottishGlenContext context)
        {
            _context = context;
        }

        public bool AddAsset(Asset asset)
        {
            _context.Entry(asset.Employee).State = EntityState.Unchanged;
            _context.Assets.Add(asset);
            _context.Employees.Attach(asset.Employee);
            _context.SaveChanges();
            return true;
        }

        public Asset GetAsset(int assetId)
        {
            return _context.Assets.Include(a => a.Employee).ThenInclude(e => e.Department).FirstOrDefault(a => a.Id == assetId);
        }

        public List<Asset> GetAssets(int employeeId)
        {
            return _context.Assets.Include(a => a.Employee).ThenInclude(e => e.Department).Where(a => a.Employee.Id == employeeId).ToList();
        }

        public bool UpdateAsset(Asset asset)
        {
            _context.Entry(asset.Employee).State = EntityState.Unchanged;
            _context.Assets.Update(asset);
            _context.SaveChanges();
            return true;
        }

        public bool DeleteAsset(int assetId)
        {
            var asset = _context.Assets.Find(assetId);
            _context.Assets.Remove(asset);
            _context.SaveChanges();
            return true;
        }

        public Asset GetAssetWithSystemInfo()
        {
            string name = Environment.MachineName;
            string manufacturer = "Unknown";
            string model = "Unknown";
            string type = "Unknown";

            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    manufacturer = obj["Manufacturer"]?.ToString() ?? "Unknown";
                    model = obj["Model"]?.ToString() ?? "Unknown";
                    type = obj["SystemType"]?.ToString() ?? "Unknown";
                }
            }

            string ipAddress = Dns.GetHostEntry(Dns.GetHostName())
               .AddressList
               .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
               ?.ToString() ?? "No IP Found";

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
