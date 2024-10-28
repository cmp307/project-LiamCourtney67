using ScottishGlenAssetTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScottishGlenAssetTracking.Data;
using System.Management;
using System.Net;

namespace ScottishGlenAssetTracking.Services
{
    public class AssetService
    {
        public bool AddAsset(Asset asset)
        {
            using (var context = new ScottishGlenContext())
            {
                context.Assets.Add(asset);
                context.Employees.Attach(asset.Employee);
                context.SaveChanges();
                return true;
            }
        }

        public Asset GetAsset(int assetId)
        {
            using (var context = new ScottishGlenContext())
            {
                return context.Assets.Find(assetId);
            }
        }

        public List<Asset> GetAssets(int employeeId)
        {
            using (var context = new ScottishGlenContext())
            {
                return context.Assets.Where(a => a.Employee.Id == employeeId).ToList();
            }
        }

        public bool UpdateAsset(Asset asset)
        {
            using (var context = new ScottishGlenContext())
            {
                context.Assets.Update(asset);
                context.SaveChanges();
                return true;
            }
        }

        public bool DeleteAsset(int assetId)
        {
            using (var context = new ScottishGlenContext())
            {
                var asset = context.Assets.Find(assetId);
                context.Assets.Remove(asset);
                context.SaveChanges();
                return true;
            }
        }

        public Asset GetAssetWithSystemInfo ()
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
