using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MySqlConnector;
using ScottishGlenAssetTracking.Models;
using ScottishGlenAssetTracking.Services;
using ScottishGlenAssetTracking.Views.Account;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.ViewModels
{
    /// <summary>
    /// Partial class for the ViewAccountViewModel using the ObservableObject class.
    /// </summary>
    public partial class ViewAccountViewModel : ObservableObject
    {
        // Private fields for the services.
        private readonly DepartmentService _departmentService;
        private readonly EmployeeService _employeeService;
        private readonly AccountService _accountService;

        // Private field for the dialog.
        private ContentDialog _updatePasswordDialog;
        private ContentDialog _deleteAccountDialog;

        /// <summary>
        /// Constructor for the ViewAccountViewModel class using the DepartmentService, EmployeeService, and AccountService with dependency injection.
        /// </summary>
        /// <param name="departmentService">DepartmentService from dependency injection.</param>
        /// <param name="employeeService">EmployeeService from dependency injection.</param>
        /// <param name="accountService">AccountService from dependency injection.</param>
        public ViewAccountViewModel(DepartmentService departmentService, EmployeeService employeeService, AccountService accountService)
        {
            // Set the selected account to the current account.
            SelectedAccount = App.AppHost.Services.GetRequiredService<AccountManager>().CurrentAccount;

            // Initialize services.
            _departmentService = departmentService;
            _employeeService = employeeService;
            _accountService = accountService;
        }

        // Properties.
        [ObservableProperty]
        private Account selectedAccount;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string statusMessage;


        // Dialog properties.

        [ObservableProperty]
        private string currentPassword;

        [ObservableProperty]
        private string newPassword;

        [ObservableProperty]
        private string confirmNewPassword;

        [ObservableProperty]
        private string dialogStatusMessage;

        // Visibility properties.

        [ObservableProperty]
        private Visibility statusVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility viewAccountViewVisibility = Visibility.Visible;

        [ObservableProperty]
        private Visibility editAccountViewVisibility = Visibility.Collapsed;

        // Dialog visibility properties.

        [ObservableProperty]
        private Visibility dialogStatusVisibility = Visibility.Collapsed;

        // Commands.

        /// <summary>
        /// Command to delete an account.
        /// </summary>
        [RelayCommand]
        private void DeleteAccount()
        {
            // Only delete an account if an account is selected.
            if (SelectedAccount != null)
            {
                // Try to delete the account, catching any exceptions that may occur.
                try
                {
                    _accountService.DeleteAccount(SelectedAccount.Email);

                    // Log out the account.
                    App.AppHost.Services.GetRequiredService<AccountManager>().Logout();

                    // Update the view to the login page.
                    var loginPage = App.AppHost.Services.GetRequiredService<Login>();
                    MainWindow.Frame.Navigate(loginPage.GetType());
                }
                catch (MySqlException)
                {
                    SetStatusMessage("There was an issue connecting to the database. Please try again later.");
                }
                catch (Exception)
                {
                    SetStatusMessage("An unexpected error occurred. Please try again later.");
                }
            }
        }

        /// <summary>
        /// Command to update an account.
        /// </summary>
        [RelayCommand]
        private void UpdateAccount()
        {
            // Only update an account if an account is selected.
            if (SelectedAccount != null)
            {
                // Try to update the account, catching any exceptions that may occur.
                try
                {
                    // Set the new email for the account.
                    SelectedAccount.Email = Email;
                    Email = string.Empty;

                    _accountService.UpdateAccount(SelectedAccount);
                    OnPropertyChanged(nameof(SelectedAccount));

                    SetStatusMessage("Account updated successfully.");

                    // Change the view to the view mode.
                    ChangeViewToView();
                }
                catch (ArgumentException ex)
                {
                    SetStatusMessage(ex.Message);
                }
                catch (MySqlException)
                {
                    SetStatusMessage("There was an issue connecting to the database. Please try again later.");
                }
                catch (Exception)
                {
                    SetStatusMessage("An unexpected error occurred. Please try again later.");
                }
            }
        }

        /// <summary>
        /// Command to change the view to the edit mode.
        /// </summary>
        [RelayCommand]
        private void ChangeViewToEdit()
        {
            // Only change the view to the edit mode if an account is selected.
            if (SelectedAccount != null)
            {
                // Set the Email property.
                Email = SelectedAccount.Email;

                // Set the visibility properties for the view.
                ViewAccountViewVisibility = Visibility.Collapsed;
                EditAccountViewVisibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Command to change the view to the view mode.
        /// </summary>
        [RelayCommand]
        private void ChangeViewToView()
        {
            // Set the visibility properties for the view.
            EditAccountViewVisibility = Visibility.Collapsed;
            ViewAccountViewVisibility = Visibility.Visible;

            // Reset the Email property.
            Email = string.Empty;
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

        // Dialog commands.

        /// <summary>
        /// Set the update password set dialog.
        /// </summary>
        /// <param name="dialog">Dialog to be used.</param>
        public void SetUpdatePasswordDialog(ContentDialog dialog) => _updatePasswordDialog = dialog;

        /// <summary>
        /// Set the delete account dialog.
        /// </summary>
        /// <param name="dialog">Dialog to be used.</param>
        public void SetDeleteAccountDialog(ContentDialog dialog) => _deleteAccountDialog = dialog;

        /// <summary>
        /// Command to show the update password dialog.
        /// </summary>
        [RelayCommand]
        private async Task ShowUpdatePasswordDialog()
        {
            // Set the dialog visibility properties.
            if (SelectedAccount != null)
            {
                ResetPasswords();
                await _updatePasswordDialog.ShowAsync();
                DialogStatusVisibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Command to show the delete account dialog.
        /// </summary>
        [RelayCommand]
        private async Task ShowDeleteAccountDialog() => await _deleteAccountDialog.ShowAsync();

        /// <summary>
        /// Method to update the password.
        /// </summary>
        /// <returns>True if password was updated, false if not.</returns>
        public bool UpdatePassword()
        {
            // Check if the passwords are empty.
            if (CurrentPassword == null || CurrentPassword == string.Empty ||
                NewPassword == null || NewPassword == string.Empty ||
                ConfirmNewPassword == null || ConfirmNewPassword == string.Empty)
            {
                SetDialogStatusMessage("Please fill in all fields.");
                ResetPasswords();
                return false;
            }

            // Only change the password if the current password is correct.
            if (!SelectedAccount.VerifyPassword(CurrentPassword))
            {
                SetDialogStatusMessage("Invalid current password");
                ResetPasswords();
                return false;
            }
            // Only change the password if the new password and confirm new password match.
            if (NewPassword != ConfirmNewPassword)
            {
                SetDialogStatusMessage("New passwords do not match.");
                ResetPasswords();
                return false;
            }

            // Try to update the password, catching any exceptions that may occur.
            try
            {
                // Update the password in the database.
                if (_accountService.UpdatePassword(SelectedAccount.Email, NewPassword))
                {
                    // Set the new password for the selected account.
                    SelectedAccount.Password = NewPassword;

                    ResetPasswords();
                    NewPassword = string.Empty;
                    return true;
                }
                else
                {
                    SetDialogStatusMessage("Password could not be updated.");
                    NewPassword = string.Empty;
                    return false;
                }
            }
            catch (ArgumentException ex)
            {
                SetDialogStatusMessage(ex.Message);

                // Only reset the new passwords since they are the only ones that could be incorrect.
                NewPassword = string.Empty;
                ConfirmNewPassword = string.Empty;
                return false;
            }
            catch (MySqlException)
            {
                SetDialogStatusMessage("There was an issue connecting to the database. Please try again later.");
                ResetPasswords();
                return false;
            }
            catch (Exception)
            {
                SetDialogStatusMessage("An unexpected error occurred. Please try again later.");
                ResetPasswords();
                return false;
            }
        }

        /// <summary>
        /// Helper method to set the dialog status message.
        /// </summary>
        /// <param name="message">Message to be displayed.</param>
        private void SetDialogStatusMessage(string message)
        {
            DialogStatusMessage = message;
            DialogStatusVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Helper method to reset the password properties.
        /// </summary>
        private void ResetPasswords()
        {
            // Reset the password properties.
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;
        }
    }
}
