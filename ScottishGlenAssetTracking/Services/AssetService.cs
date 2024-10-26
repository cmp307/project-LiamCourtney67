using ScottishGlenAssetTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScottishGlenAssetTracking.Data;

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
    }
}
