using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using MySqlConnector;
using ScottishGlenAssetTracking.Data;
using ScottishGlenAssetTracking.Models;
using ScottishGlenAssetTracking.Services;
using ScottishGlenAssetTracking.Views.Account;
using ScottishGlenAssetTracking.Views.HardwareAsset;
using ScottishGlenAssetTracking.Views.Portals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.ViewModels
{
    /// <summary>
    /// Partial class for the LoginViewModel using the ObservableObject class.
    /// </summary>
    public partial class LoginViewModel : ObservableObject
    {
        // Private field for the AccountService.
        private readonly AccountService _accountService;

        /// <summary>
        /// Constructor for the LoginViewModel class using the AccountService with dependency injection.
        /// </summary>
        /// <param name="accountService">AccountService from dependency injection.</param>
        public LoginViewModel(AccountService accountService)
        {
            // Set the account service.
            _accountService = accountService;
        }

        // Properties.
        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string statusMessage;

        // Visibility properties.
        [ObservableProperty]
        private Visibility statusVisibility = Visibility.Visible;

        // Commands

        /// <summary>
        /// Command to log in the user.
        /// </summary>
        [RelayCommand]
        public void Login()
        {
            // Check if the email and password are empty or null.
            if (Email == string.Empty || Email == null || Password == string.Empty || Password == null)
            {
                SetStatusMessage("Please fill in all fields.");
                return;
            }

            // Attempt to log in the user, catching any exceptions that may occur.
            try
            { 
                if (App.AppHost.Services.GetRequiredService<AccountManager>().Login(Email, Password))
                {
                    ResetProperties();
                    NavigateToPortal();
                }
                else
                {
                    Password = string.Empty;
                    SetStatusMessage("Login failed.");
                }
            }
            catch (AuthenticationException ex)
            {
                Password = string.Empty;
                SetStatusMessage(ex.Message);
            }
            catch (MySqlException ex)
            {
                Password = string.Empty;
                SetStatusMessage("There was an issue connecting to the database. Please try again later.");
            }
            catch (Exception ex)
            {
                Password = string.Empty;
                SetStatusMessage("An unexpected error occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Command to navigate to the register page.
        /// </summary>
        [RelayCommand]
        public void NavigateToRegister()
        {
            ResetProperties();

            var registerPage = App.AppHost.Services.GetRequiredService<Register>();
            MainWindow.Frame.Navigate(registerPage.GetType());
        }

        /// <summary>
        /// Navigates to the appropriate portal based on the account type.
        /// </summary>
        private void NavigateToPortal()
        {
            // Get the current account from the AccountManager.
            Account account = App.AppHost.Services.GetRequiredService<AccountManager>().CurrentAccount;

            if (account == null)
            {
                return;
            }
            // Admin portal.
            else if (account.IsAdmin)
            {
                var adminPortal = App.AppHost.Services.GetRequiredService<AdminPortal>();
                MainWindow.Frame.Navigate(adminPortal.GetType());
            }
            // Employee portal.
            else if (account.Employee != null)
            {
                var employeePortal = App.AppHost.Services.GetRequiredService<EmployeePortal>();
                MainWindow.Frame.Navigate(employeePortal.GetType());
            }
            // New user portal.
            else
            {
                var newUserPortal = App.AppHost.Services.GetRequiredService<NewUserPortal>();
                MainWindow.Frame.Navigate(newUserPortal.GetType());
            }
        }

        /// <summary>
        /// Helper method to reset the properties of the view model.
        /// </summary>
        private void ResetProperties()
        {
            // Reset the email, password, and status message properties.
            Email = string.Empty;
            Password = string.Empty;
            StatusMessage = string.Empty;
        }

        /// <summary>
        /// Helper method to set the status message and make the status message visible.
        /// </summary>
        /// <param name="message">Message to be displayed.</param>
        private void SetStatusMessage(string message)
        {
            // Set the status message and make the status message visible.
            StatusMessage = message;
            StatusVisibility = Visibility.Visible;
        }
    }
}
