using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.Models
{
    /// <summary>
    /// Model class for the Account entity.
    /// </summary>
    public class Account
    {
        // Private fields for the Account entity.
        private int _id;                            // Set by the database, so no validation needed.
        private string _email;
        private string _password;
        private Employee _employee;                 // No validation needed, handled by Employee entity.
        private int _employeeId;                    // Reference property for the EmployeeId of the Employee entity.
        private bool _isAdmin;

        /// <summary>
        /// Id property for the Account entity.
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Email property for the Account entity.
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
        /// Password property for the Account entity.
        /// </summary>
        public string Password
        {
            get { return _password; }
            set
            {
                if (IsValidPassword(value))
                {
                    _password = HashPassword(value);
                }
                else
                {
                    throw new ArgumentException("Password must be between 8 and 64 characters.");
                }
            }
        }

        /// <summary>
        /// Navigational property for the Employee entity within the Account entity.
        /// </summary>
        public Employee Employee
        {
            get { return _employee; }
            set { _employee = value; }
        }

        /// <summary>
        /// Reference property for the EmployeeId of the Employee entity within the Account entity.
        /// </summary>
        public int? EmployeeId => Employee.Id;

        /// <summary>
        /// Used to display the full name of the Employee entity within the Account entity.
        /// </summary>
        public string EmployeeName => Employee.Name;

        /// <summary>
        /// IsAdmin property for the Account entity, used to determine if the account is an administrator.
        /// </summary>
        public bool IsAdmin
        {
            get { return _isAdmin; }
            set
            {
                if (value == false)
                {
                    _isAdmin = false;
                }
                else if (CanBeAdmin())
                {
                    _isAdmin = true;
                }
                else
                {
                    _isAdmin = false;
                    throw new ArgumentException("Only IT department employees can be administrators.");
                }
            }
        }

        /// <summary>
        /// Hashes a password using BCrypt.
        /// </summary>
        /// <param name="password">Password to be hashed.</param>
        /// <returns>Hashed password using BCrypt.</returns>
        private string HashPassword(string password) { return BCrypt.Net.BCrypt.EnhancedHashPassword(password); }

        /// <summary>
        /// Verifies a password using BCrypt.
        /// </summary>
        /// <param name="password">Password to be verified.</param>
        /// <returns>True if verified using BCrypt, false if not valid.</returns>
        public bool VerifyPassword(string password) { return BCrypt.Net.BCrypt.EnhancedVerify(password, _password); }

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

        /// <summary>
        /// Validation method for the Password property of the Account entity, must be between 8 and 64 characters.
        /// </summary>
        /// <param name="password">Password to be validated.</param>
        /// <returns>True if valid, false if not.</returns>
        private bool IsValidPassword(string password) => password.Length >= 8 && password.Length <= 64;

        /// <summary>
        /// Validates the IsAdmin property of the Account entity, only the IT department can be an administrator.
        /// </summary>
        /// <returns>True if valid, false if not.</returns>
        private bool CanBeAdmin() => Employee.Department.Id == 5;    // Department 5 is the IT department, Departments are static and will not change.
    }
}
