using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using MySqlConnector;
using ScottishGlenAssetTracking.Models;
using ScottishGlenAssetTracking.Services;
using ScottishGlenAssetTracking.Views.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.ViewModels
{
    /// <summary>
    /// Partial class for the RegisterViewModel using the ObservableObject class.
    /// </summary>
    public partial class RegisterViewModel : ObservableObject
    {
        // Private field for the AccountService.
        private readonly AccountService _accountService;

        /// <summary>
        /// Constructor for the RegisterViewModel class using the AccountService with dependency injection.
        /// </summary>
        /// <param name="accountService">AccountService from dependency injection.</param>
        public RegisterViewModel(AccountService accountService)
        {
            // Set the account service.
            _accountService = accountService;
        }

        // Properties.
        [ObservableProperty]
        private Account newAccount;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string confirmPassword;

        [ObservableProperty]
        private string statusMessage;

        // Visibility properties.
        [ObservableProperty]
        private Visibility statusVisibility = Visibility.Visible;

        // Commands

        /// <summary>
        /// Command to register a new account.
        /// </summary>
        [RelayCommand]
        public void Register()
        {
            // Check if the email, password, and confirm password fields are filled in.
            if (Email == string.Empty || Email == null ||
                Password == string.Empty || Password == null ||
                ConfirmPassword == string.Empty || ConfirmPassword == null)
            {
                StatusMessage = "Please fill in all fields.";
                StatusVisibility = Visibility.Visible;
                return;
            }

            // Attempt to register the new account, catching any exceptions that may occur.
            try
            {
                // Create a new account with the provided email and password.
                NewAccount = new Account
                {
                    Email = Email,
                    Password = Password
                };

                // Verify the password and confirm password match.
                if (!NewAccount.VerifyPassword(ConfirmPassword))
                {
                    ConfirmPassword = string.Empty;
                    SetStatusMessage("Passwords do not match.");
                    return;
                }

                // Add the account to the database.
                if (_accountService.AddAccount(NewAccount))
                {
                    ResetProperties();
                    SetStatusMessage("Account added successfully.");
                }
                else
                {
                    // Do not mention if an email already exists in the database - security risk.
                    SetStatusMessage("Account could not be added.");
                }
            }
            catch (ArgumentException ex)
            {
                Password = string.Empty;
                ConfirmPassword = string.Empty;
                SetStatusMessage(ex.Message);
            }
            catch (MySqlException)
            {
                Password = string.Empty;
                ConfirmPassword = string.Empty;
                SetStatusMessage("There was an issue connecting to the database. Please try again later.");
            }
            catch (Exception)
            {
                Password = string.Empty;
                ConfirmPassword = string.Empty;
                SetStatusMessage("An unexpected error occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Command to return to the login page.
        /// </summary>
        [RelayCommand]
        public void ReturnToLogin()
        {
            ResetProperties();

            var loginPage = App.AppHost.Services.GetRequiredService<Login>();
            MainWindow.Frame.Navigate(typeof(Login));
        }

        /// <summary>
        /// Helper method to reset the properties of the RegisterViewModel.
        /// </summary>
        private void ResetProperties()
        {
            // Reset the new account, email, password, and status message properties.
            NewAccount = null;
            Email = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            StatusMessage = string.Empty;
        }

        /// <summary>
        /// Helper method to set the status message and make the status message visible.
        /// </summary>
        /// <param name="message">Message to be displayed.</param>
        private void SetStatusMessage(string message)
        {
            StatusMessage = message;
            StatusVisibility = Visibility.Visible;
        }
    }
}
