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
    public class AccountService
    {
        private readonly ScottishGlenContext _context;

        public AccountService(ScottishGlenContext context)
        {
            _context = context;
        }

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

        public Account GetAccount(string email)
        {
            return _context.Accounts.Include(a => a.Employee).FirstOrDefault(a => a.Email == email);
        }

        public List<Account> GetAccounts()
        {
            return _context.Accounts.Include(a => a.Employee).ToList();
        }

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

        public Account AuthenticateAccount(string email, string password)
        {
            // Retrieve account and verify password
            var account = _context.Accounts.Include(a => a.Employee).FirstOrDefault(a => a.Email == email);

            if (account != null && account.VerifyPassword(password))
            {
                return account;
            }

            return null;
        }

        public bool UpdatePassword(string email, string password)
        {
            // Retrieve account and update password
            var account = _context.Accounts.FirstOrDefault(a => a.Email == email);

            if (account != null && account.VerifyPassword(password))
            {
                account.Password = password;
                _context.Accounts.Update(account);
                _context.SaveChanges();

                return true;
            }

            return false;
        }
    }
}
