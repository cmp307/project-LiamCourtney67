using Microsoft.EntityFrameworkCore;
using ScottishGlenAssetTracking.Data;
using ScottishGlenAssetTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.Services
{
    /// <summary>
    /// AccountService class used to interact with the database for the Account entity.
    /// </summary>
    public class AccountService
    {
        // Private field for the ScottishGlenContext.
        private readonly ScottishGlenContext _context;

        /// <summary>
        /// Constructor for the AccountService class.
        /// </summary>
        /// <param name="context">ScottishGlenContext to be injected using dependency injection.</param>
        public AccountService(ScottishGlenContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Method to add an Account to the database.
        /// </summary>
        /// <param name="account">Account to be added to the database.</param>
        /// <returns>True if added to the database, false if not.</returns>
        public bool AddAccount(Account account)
        {
            // Check if the account already exists
            if (_context.Accounts.Any(a => a.Email == account.Email))
            {
                return false;
            }

            // Add the account to the database and save the changes
            _context.Accounts.Add(account);
            _context.SaveChanges();

            return true;
        }

        /// <summary>
        /// Method to get an Account from the database by its email.
        /// </summary>
        /// <param name="email">Email of the Account to be retrieved.</param>
        /// <returns>Account from the database with the chosen email.</returns>
        public Account GetAccount(string email)
        {
            return _context.Accounts.Include(a => a.Employee).ThenInclude(e => e.Department).FirstOrDefault(a => a.Email == email);
        }

        /// <summary>
        /// Method to get all Accounts from the database.
        /// </summary>
        /// <returns>List of all Accounts from the database.</returns>
        public List<Account> GetAccounts()
        {
            return _context.Accounts.Include(a => a.Employee).ToList();
        }

        /// <summary>
        /// Method to get all Accounts from the database for a specific department.
        /// </summary>
        /// <param name="departmentId">Id of Department to get the Accounts for.</param>
        /// <returns>List of Accounts from the database for the chosen Department.</returns>
        public List<Account> GetAccounts(int departmentId)
        {
            return _context.Accounts.Include(a => a.Employee).Where(a => a.Employee.Department.Id == departmentId).ToList();
        }

        /// <summary>
        /// Method to get all Accounts from the database for a specific department and admin status.
        /// </summary>
        /// <param name="departmentId">Id of the Department to retrieve all the accounts for.</param>
        /// <param name="isAdmin">If the Account is an admin or not.</param>
        /// <returns>List of Accounts from the database for the chosen Department and admin status.</returns>
        public List<Account> GetAccounts(int departmentId, bool isAdmin)
        {
            return _context.Accounts.Include(a => a.Employee).Where(a => a.Employee.Department.Id == departmentId && a.IsAdmin == isAdmin).ToList();
        }

        /// <summary>
        /// Method to update an Account in the database.
        /// </summary>
        /// <param name="account">Account to be updated in the database.</param>
        /// <returns>True if updated in the database, false if not.</returns>
        public bool UpdateAccount(Account account)
        {
            // Check if the account exists
            if (_context.Accounts.Any(a => a.Id == account.Id))
            {
                // Update the account and save the changes
                _context.Accounts.Update(account);
                _context.SaveChanges();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Method to delete an Account from the database.
        /// </summary>
        /// <param name="email">Email of the Account to be deleted.</param>
        /// <returns></returns>
        public bool DeleteAccount(string email)
        {
            // Check if the account exists
            var account = _context.Accounts.FirstOrDefault(a => a.Email == email);

            if (account != null)
            {
                // Remove the account from the database and save the changes
                _context.Accounts.Remove(account);
                _context.SaveChanges();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Method to authenticate an Account using the email and password.
        /// </summary>
        /// <param name="email">Email of the Account to be authenticated.</param>
        /// <param name="password">Password of the Account to be authenticated.</param>
        /// <returns>Authenticated Account if successful, null if not.</returns>
        public Account AuthenticateAccount(string email, string password)
        {
            // Retrieve account and verify password
            Account account = GetAccount(email);

            if (account != null && account.VerifyPassword(password))
            {
                return account;
            }

            return null;
        }

        /// <summary>
        /// Method to update the password of an Account.
        /// </summary>
        /// <param name="email">Email of the Account for password change.</param>
        /// <param name="password">New password to be changed to.</param>
        /// <returns></returns>
        public bool UpdatePassword(string email, string password)
        {
            // Retrieve account and update password
            var account = _context.Accounts.FirstOrDefault(a => a.Email == email);

            if (account != null)
            {
                account.Password = password;
                _context.Accounts.Update(account);
                _context.SaveChanges();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Method to set an Account as an administrator.
        /// </summary>
        /// <param name="email">Email of the Account to be set to admin.</param>
        /// <returns>True if set to admin, false if not.</returns>
        public bool SetAccountToAdmin(string email)
        {
            // Retrieve account and update admin status
            var account = _context.Accounts.FirstOrDefault(a => a.Email == email);

            if (account != null)
            {
                account.IsAdmin = true;
                _context.Accounts.Update(account);
                _context.SaveChanges();

                return true;
            }

            return false;
        }
    }
}
