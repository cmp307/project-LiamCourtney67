using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.Models
{
    /// <summary>
    /// Model class for the HardwareAsset entity.
    /// </summary>
    public class HardwareAsset
    {
        // Private fields for the HardwareAsset entity.
        private int _id;                                // Set by the database, so no validation needed.
        private string _name;
        private string _model;
        private string _manufacturer;
        private string _type;
        private string _ipAddress;
        private DateTime? _purchaseDate;                // Purchase date can be empty
        private string? _notes;                         // Notes can be empty
        private Employee _employee;                     // No validation needed, handled by Employee entity.
        private SoftwareAsset _softwareAsset;           // No validation needed, handled by SoftwareAsset entity.
        private DateTime _softwareLinkDate;

        /// <summary>
        /// Id property for the HardwareAsset entity.
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Name property for the HardwareAsset entity.
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
                    throw new ArgumentException("Name must be 15 characters or less and contain only letters, digits, and hyphens.");
                }
            }
        }

        /// <summary>
        /// Model property for the HardwareAsset entity.
        /// </summary>
        public string Model
        {
            get { return _model; }
            set
            {
                if (IsValidModel(value.Trim()))
                {
                    // Trim the value to remove leading and trailing whitespace.
                    _model = value.Trim();
                }
                else
                {
                    throw new ArgumentException("Model must be 64 characters or less and contain only letters, digits, spaces, hyphens, periods, and apostrophes.");
                }
            }
        }

        /// <summary>
        /// Manufacturer property for the HardwareAsset entity.
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
                    throw new ArgumentException("Manufacturer must be 64 characters or less and contain only letters, digits, spaces, hyphens, periods, and apostrophes.");
                }
            }
        }

        /// <summary>
        /// Type property for the HardwareAsset entity.
        /// </summary>
        public string Type
        {
            get { return _type; }
            set
            {
                if (IsValidType(value.Trim()))
                {
                    // Trim the value to remove leading and trailing whitespace.
                    _type = value.Trim();
                }
                else
                {
                    throw new ArgumentException("Type must be 64 characters or less and contain only letters, digits, spaces, and hyphens.");
                }
            }
        }

        /// <summary>
        /// IpAddress property for the HardwareAsset entity.
        /// </summary>
        public string IpAddress
        {
            get { return _ipAddress; }
            set
            {
                if (IsValidIpAddress(value))
                {
                    _ipAddress = value;
                }
                else
                {
                    throw new ArgumentException("Invalid IP Address.");
                }
            }
        }

        /// <summary>
        /// Optional PurchaseDate property for the HardwareAsset entity.
        /// </summary>
        public DateTime? PurchaseDate
        {
            get { return _purchaseDate; }
            set
            {
                if (value == null)
                {
                    _purchaseDate = null;
                }
                else if (IsValidPurchaseDate(value))
                {
                    _purchaseDate = value;
                }
                else
                {
                    throw new ArgumentException("Purchase date must be between 1990 and the current date.");
                }
            }
        }

        /// <summary>
        /// Optional Notes property for the HardwareAsset entity.
        /// </summary>
        public string? Notes
        {
            get { return _notes; }
            set
            {
                if (value == null || value == string.Empty)
                {
                    _notes = null;
                }
                else
                {
                    // Check if the notes are valid (256 characters or less
                    if (IsValidNotes(value.Trim()))
                    {
                        // Trim the value to remove leading and trailing whitespace.
                        _notes = value.Trim();
                    }
                    else
                    {
                        throw new ArgumentException("Notes must be 256 characters or less.");
                    }
                }
            }
        }

        /// <summary>
        /// Navigational property for the Employee entity within the HardwareAsset entity.
        /// </summary>
        public Employee Employee
        {
            get { return _employee; }
            set { _employee = value; }
        }

        /// <summary>
        /// Navigational property for the SoftwareAsset entity within the HardwareAsset entity.
        /// </summary>
        public SoftwareAsset SoftwareAsset
        {
            get { return _softwareAsset; }
            set { _softwareAsset = value; }
        }

        /// <summary>
        /// SoftwareLinkDate property for the HardwareAsset entity, the date the software was linked to the hardware.
        /// </summary>
        public DateTime SoftwareLinkDate
        {
            get { return _softwareLinkDate; }
            set { _softwareLinkDate = value; }
        }

        /// <summary>
        /// Validation method for the Name property of the HardwareAsset entity, must be 15 characters or less and contain only letters, digits, and hyphens.
        /// </summary>
        /// <param name="name">Name to be validated.</param>
        /// <returns>True if valid, false if not.</returns>
        private bool IsValidName(string name)
        {
            bool isNullOrWhiteSpace = string.IsNullOrWhiteSpace(name);
            bool isTooLong = name.Length > 15;
            bool hasInvalidCharacters = name.Any(c => !char.IsLetterOrDigit(c) && c != '-');

            return !isNullOrWhiteSpace && !isTooLong && !hasInvalidCharacters;
        }

        /// <summary>
        /// Validation method for the Model property of the HardwareAsset entity, must be 64 characters or less and contain only letters, digits, spaces, hyphens, periods, and apostrophes.
        /// </summary>
        /// <param name="model">Model to be validated.</param>
        /// <returns>True if valid, false if not.</returns>
        private bool IsValidModel(string model)
        {
            bool isNullOrWhiteSpace = string.IsNullOrWhiteSpace(model);
            bool isTooLong = model.Length > 64;
            bool hasInvalidCharacters = model.Any(c => !char.IsLetterOrDigit(c) && c != ' ' && c != '-' && c != '.' && c != '\'');

            return !isNullOrWhiteSpace && !isTooLong && !hasInvalidCharacters;
        }

        /// <summary>
        /// Validation method for the Manufacturer property of the HardwareAsset entity, must be 64 characters or less and contain only letters, digits, spaces, hyphens, periods, and apostrophes.
        /// </summary>
        /// <param name="manufacturer">Manufacturer to be validated.</param>
        /// <returns>True if valid, false if not.</returns>
        private bool IsValidManufacturer(string manufacturer)
        {
            bool isNullOrWhiteSpace = string.IsNullOrWhiteSpace(manufacturer);
            bool isTooLong = manufacturer.Length > 64;
            bool hasInvalidCharacters = manufacturer.Any(c => !char.IsLetterOrDigit(c) && c != ' ' && c != '-' && c != '.' && c != '\'');

            return !isNullOrWhiteSpace && !isTooLong && !hasInvalidCharacters;
        }

        /// <summary>
        /// Validation method for the Type property of the HardwareAsset entity, must be 64 characters or less and contain only letters, digits, spaces, and hyphens.
        /// </summary>
        /// <param name="type">Type to be validated</param>
        /// <returns>True if valid, false if not.</returns>
        private bool IsValidType(string type)
        {
            bool isNullOrWhiteSpace = string.IsNullOrWhiteSpace(type);
            bool isTooLong = type.Length > 64;
            bool hasInvalidCharacters = type.Any(c => !char.IsLetterOrDigit(c) && c != ' ' && c != '-');

            return !isNullOrWhiteSpace && !isTooLong && !hasInvalidCharacters;
        }

        /// <summary>
        /// Validation method for the IpAddress property of the HardwareAsset entity, using the IPAddress.TryParse method.
        /// </summary>
        /// <param name="ipAddress">IP Address to be validated.</param>
        /// <returns>True if valid, false if not.</returns>
        private bool IsValidIpAddress(string ipAddress) => IPAddress.TryParse(ipAddress, out _);

        /// <summary>
        /// Validation method for the PurchaseDate property of the HardwareAsset entity, must be between 1990 and the current date.
        /// </summary>
        /// <param name="purchaseDate">PurchaseDate to be validated.</param>
        /// <returns>True if valid, false if not.</returns>
        private bool IsValidPurchaseDate(DateTime? purchaseDate)
        {
            DateTime minDate = new DateTime(1990, 1, 1);        // TODO - Set to a reasonable minimum date
            DateTime maxDate = DateTime.Now;

            return purchaseDate >= minDate && purchaseDate <= maxDate;
        }

        /// <summary>
        /// Validation method for the Notes property of the HardwareAsset entity, must be 256 characters or less.
        /// </summary>
        /// <param name="notes">Notes to be validated.</param>
        /// <returns>True if valid, false if not.</returns>
        private bool IsValidNotes(string? notes) => notes?.Length <= 256;
    }
}
