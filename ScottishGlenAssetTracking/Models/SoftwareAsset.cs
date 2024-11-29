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
        private int _id;                                    // Set by the database, so no validation needed.
        private string _name;
        private string _version;
        private string _manufacturer;
        private List<HardwareAsset> _hardwareAssets;        // No validation needed, handled by HardwareAsset entity.

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
            set
            {
                if (IsValidName(value.Trim()))
                {
                    // Trim the value to remove leading and trailing whitespace.
                    _name = value.Trim();
                }
                else
                {
                    throw new ArgumentException("Name must be between 1 and 64 characters and contain only letters, digits, spaces, periods, and hyphens.");
                }
            }
        }

        /// <summary>
        /// Version property for the SoftwareAsset entity.
        /// </summary>
        public string Version
        {
            get { return _version; }
            set
            {
                if (IsValidVersion(value.Trim()))
                {
                    // Trim the value to remove leading and trailing whitespace.
                    _version = value.Trim();
                }
                else
                {
                    throw new ArgumentException("Version must be between 1 and 64 characters and contain only letters, digits, and periods.");
                }
            }
        }

        /// <summary>
        /// Manufacturer property for the SoftwareAsset entity.
        /// </summary>
        public string Manufacturer
        {
            get { return _manufacturer; }
            set
            {
                if (IsValidManufacturer(value.Trim()))
                {
                    // Trim the value to remove leading and trailing whitespace.
                    _manufacturer = value.Trim();
                }
                else
                {
                    throw new ArgumentException("Manufacturer must be between 1 and 64 characters and contain only letters, digits, spaces, hyphens, periods, and apostrophes.");
                }
            }
        }

        /// <summary>
        /// List of navigational properties for the HardwareAsset entities within the SoftwareAsset entity.
        /// </summary>
        public List<HardwareAsset> HardwareAssets
        {
            get { return _hardwareAssets; }
            set { _hardwareAssets = value; }
        }

        /// <summary>
        /// DisplayName property for the SoftwareAsset entity, returns the Name and Version properties concatenated together.
        /// </summary>
        public string DisplayName
        {
            get { return $"{Name} {Version}"; }
        }

        /// <summary>
        /// Validation method for the Name property of the SoftwareAsset entity, must be between 1 and 64 characters and contain only letters, digits, spaces, periods, and hyphens.
        /// </summary>
        /// <param name="name">Name to be validated.</param>
        /// <returns>True if valid, false if not.</returns>
        private bool IsValidName(string name)
        {
            bool isNullOrWhitespace = string.IsNullOrWhiteSpace(name);
            bool isTooLong = name.Length > 64;
            bool hasInvalidCharacters = name.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c) && c != '.' && c != '-');

            return !isNullOrWhitespace && !isTooLong && !hasInvalidCharacters;
        }

        /// <summary>
        /// Validation method for the Version property of the SoftwareAsset entity, must be between 1 and 64 characters and contain only letters, digits, and periods.
        /// </summary>
        /// <param name="version">Version to be validated.</param>
        /// <returns>True if valid, false if not.</returns>
        private bool IsValidVersion(string version)
        {
            bool isNullOrWhitespace = string.IsNullOrWhiteSpace(version);
            bool isTooLong = version.Length > 64;
            bool hasInvalidCharacters = version.Any(c => !char.IsLetterOrDigit(c) && c != '.');

            return !isNullOrWhitespace && !isTooLong && !hasInvalidCharacters;
        }

        /// <summary>
        /// Validation method for the Manufacturer property of the SoftwareAsset entity, must be between 1 and 64 characters and contain only letters, digits, spaces, hyphens, periods, and apostrophes.
        /// </summary>
        /// <param name="manufacturer">Manufacturer to be validated.</param>
        /// <returns>True if valid, false if not.</returns>
        private bool IsValidManufacturer(string manufacturer)
        {
            bool isNullOrWhitespace = string.IsNullOrWhiteSpace(manufacturer);
            bool isTooLong = manufacturer.Length > 64;
            bool hasInvalidCharacters = manufacturer.Any(c => !char.IsLetterOrDigit(c) && c != ' ' && c != '-' && c != '.' && c != '\'');

            return !isNullOrWhitespace && !isTooLong && !hasInvalidCharacters;
        }
    }
}
