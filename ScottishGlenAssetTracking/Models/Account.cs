using System;
using System.Collections.Generic;
using System.Linq;
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
        private int _id;
        private string _email;
        private string _password;
        private Employee _employee;
        private int _employeeId;
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
            set { _email = value; }
        }

        /// <summary>
        /// Password property for the Account entity.
        /// </summary>
        public string Password
        {
            get { return _password; }
            set { _password = HashPassword(value); }
        }

        /// <summary>
        /// Navigational property for the Employee entity within the Account entity.
        /// </summary>
        public Employee Employee
        {
            get { return _employee; }
            set { _employee = value; }
        }

        public int? EmployeeId => Employee.Id;

        /// <summary>
        /// IsAdmin property for the Account entity, used to determine if the account is an administrator.
        /// </summary>
        public bool IsAdmin
        {
            get { return _isAdmin; }
            set { SetAdmin(); }
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

        private bool SetAdmin()
        {
            // TODO
            IsAdmin = true;
            return true;
        }
    }
}
