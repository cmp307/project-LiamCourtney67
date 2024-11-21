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
        private HardwareAsset _hardwareAsset;

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
        /// Navigational property for the HardwareAsset entity within the SoftwareAsset entity.
        /// </summary>
        public HardwareAsset HardwareAsset
        {
            get { return _hardwareAsset; }
            set { _hardwareAsset = value; }
        }
    }
}
