using ScottishGlenAssetTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.Services
{
    /// <summary>
    /// Service class for the management of accounts.
    /// </summary>
    public class AccountManager
    {
        // Private field for the AccountService.
        private readonly AccountService _accountService;

        /// <summary>
        /// Current account property for the AccountManager class, used to determine the currently logged in account.
        /// </summary>
        public Account CurrentAccount { get; private set; }

        /// <summary>
        /// Constructor for the AccountManager class using the AccountService with dependency injection.
        /// </summary>
        /// <param name="accountService">AccountService from dependency injection.</param>
        public AccountManager(AccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Logs in the user using the provided email and password.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <param name="password">User's password.</param>
        public void Login(string email, string password)
        {
            // Logic to authenticate and set the current account
            CurrentAccount = _accountService.AuthenticateAccount(email, password);
        }

        /// <summary>
        /// Logs out the user by setting the current account to null.
        /// </summary>
        public void Logout() => CurrentAccount = null;
    }
}
