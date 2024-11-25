using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.Models
{
    /// <summary>
    /// Model class for the SoftwareAsset entity.
    /// </summary>
    public class SoftwareAsset
    {
        // Private fields for the SoftwareAsset entity.
        private int _id;
        private string _name;
        private string _version;
        private string _manufacturer;
        private List<HardwareAsset> _hardwareAssets;

        /// <summary>
        /// Id property for the SoftwareAsset entity.
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Name property for the SoftwareAsset entity.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Version property for the SoftwareAsset entity.
        /// </summary>
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        /// <summary>
        /// Manufacturer property for the SoftwareAsset entity.
        /// </summary>
        public string Manufacturer
        {
            get { return _manufacturer; }
            set { _manufacturer = value; }
        }

        /// <summary>
        /// List of navigational properties for the HardwareAsset entities within the SoftwareAsset entity.
        /// </summary>
        public List<HardwareAsset> HardwareAssets
        {
            get { return _hardwareAssets; }
            set { _hardwareAssets = value; }
        }
    }
}
