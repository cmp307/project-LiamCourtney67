using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.Models
{
    /// <summary>
    /// Model class for the Employee entity.
    /// </summary>
    public class Employee
    {
        // Private fields for the Employee entity.
        private int _id;                                // Set by the database, so no validation needed.
        private string _firstName;
        private string _lastName;
        private string _email;
        private Department _department;                 // No validation needed, handled by Department entity.
        private List<HardwareAsset> _hardwareAssets;    // No validation needed, handled by HardwareAsset entity.
        private Account _account;                       // No validation needed, handled by Account entity.

        /// <summary>
        /// Id property for the Employee entity.
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// FirstName property for the Employee entity.
        /// </summary>
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (IsValidName(value.Trim()))
                {
                    // Trim the value to remove leading and trailing whitespace.
                    _firstName = value.Trim();
                }
                else
                {
                    throw new ArgumentException("First name must be between 1 and 64 characters and contain only letters, spaces, apostrophes, and hyphens.");
                }
            }
        }

        /// <summary>
        /// LastName property for the Employee entity.
        /// </summary>
        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (IsValidName(value.Trim()))
                {
                    // Trim the value to remove leading and trailing whitespace.
                    _lastName = value.Trim();
                }
                else
                {
                    throw new ArgumentException("Last name must be between 1 and 64 characters and contain only letters, spaces, apostrophes, and hyphens.");
                }
            }
        }

        /// <summary>
        /// Non-mapped property for the full name of the employee entity.
        /// </summary>
        [NotMapped]
        public string Name => $"{FirstName} {LastName}";

        /// <summary>
        /// Email property for the Employee entity.
        /// </summary>
        public string Email
        {
            get { return _email; }
            set
            {
                if (IsValidEmail(value.Trim()))
                {
                    // Trim the value to remove leading and trailing whitespace.
                    _email = value.Trim();
                }
                else
                {
                    throw new ArgumentException("Email must be a valid email address.");
                }
            }
        }

        /// <summary>
        /// Navigational property for the Department entity within the Employee entity.
        /// </summary>
        public Department Department
        {
            get { return _department; }
            set { _department = value; }
        }

        /// <summary>
        /// List of navigational properties for the HardwareAsset entities within the Employee entity.
        /// </summary>
        public List<HardwareAsset> HardwareAssets
        {
            get { return _hardwareAssets; }
            set { _hardwareAssets = value; }
        }

        /// <summary>
        /// Account property for the Employee entity.
        /// </summary>
        public Account Account
        {
            get { return _account; }
            set { _account = value; }
        }

        /// <summary>
        /// Validation method for the Name property of the Employee entity, must be between 1 and 64 characters and contain only letters, spaces, apostrophes, and hyphens.
        /// </summary>
        /// <param name="name">Name to be validated.</param>
        /// <returns>True if valid, false if not.</returns>
        private bool IsValidName(string name)
        {
            // Validation is based off https://www.w3.org/International/questions/qa-personal-names
            bool isNullOrWhiteSpace = string.IsNullOrWhiteSpace(name);
            bool isTooLong = name.Length > 64;
            bool hasInvalidCharacters = name.Any(c => !char.IsLetter(c) && !char.IsWhiteSpace(c) && c != '\'' && c != '-' && c != '.');

            return !isNullOrWhiteSpace && !isTooLong && !hasInvalidCharacters;
        }

        /// <summary>
        /// Validation method for the Email property of the Employee entity, using the MailAddress class to validate.
        /// </summary>
        /// <param name="email">Email to be validated.</param>
        /// <returns>True if valid, false if not.</returns>
        private bool IsValidEmail(string email)
        {
            try
            {
                // Attempt to create a MailAddress object from the email string.
                MailAddress mailAddress = new MailAddress(email);

                // Valid email.
                return true;
            }
            catch (FormatException)
            {
                // Invalid email.
                return false;
            }
        }
    }
}
