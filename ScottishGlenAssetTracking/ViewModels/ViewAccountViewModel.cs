using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
    public partial class ViewAccountViewModel : ObservableObject
    {
        private readonly DepartmentService _departmentService;
        private readonly EmployeeService _employeeService;
        private readonly AccountService _accountService;

        private ContentDialog _updatePasswordDialog;

        public ViewAccountViewModel(DepartmentService departmentService, EmployeeService employeeService, AccountService accountService)
        {
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

        [RelayCommand]
        private void DeleteAccount()
        {
            // Only delete an account if an account is selected.
            if (SelectedAccount != null)
            {
                // Delete the selected account from the database.
                _accountService.DeleteAccount(SelectedAccount.Email);

                // Log out the account.
                App.AppHost.Services.GetRequiredService<AccountManager>().Logout();

                // Update the view to the login page.
                var loginPage = App.AppHost.Services.GetRequiredService<Login>();
                MainWindow.Frame.Navigate(loginPage.GetType());
            }
        }

        [RelayCommand]
        private void UpdateAccount()
        {
            // Only update an account if an account is selected.
            if (SelectedAccount != null)
            {
                // Update the selected account in the database and notify the view.
                _accountService.UpdateAccount(SelectedAccount);
                OnPropertyChanged(nameof(SelectedAccount));

                // Set the status message and make it visible.
                StatusVisibility = Visibility.Visible;
                StatusMessage = "Account Updated";

                // Change the view to the view mode.
                ChangeViewToView();
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
        }

        // Dialog commands.

        public void SetDialog(ContentDialog dialog) => _updatePasswordDialog = dialog;

        [RelayCommand]
        private async Task ShowDialog()
        {
            // Set the dialog visibility properties.
            if (SelectedAccount != null)
            {
                await _updatePasswordDialog.ShowAsync();
                DialogStatusVisibility = Visibility.Collapsed;
            }
        }

        public bool UpdatePassword()
        {
            // Only change the password if the current password is correct.
            if (!SelectedAccount.VerifyPassword(CurrentPassword))
            {
                // Set the status message and make it visible.
                DialogStatusVisibility = Visibility.Visible;
                DialogStatusMessage = "Incorrect Password";
                ResetPasswords();
                return false;
            }
            // Only change the password if the new password and confirm new password match.
            if (NewPassword != ConfirmNewPassword)
            {
                // Set the status message and make it visible.
                DialogStatusVisibility = Visibility.Visible;
                DialogStatusMessage = "Passwords do not match";
                ResetPasswords();
                return false;
            }

            // Update the password in the database.
            if (_accountService.UpdatePassword(SelectedAccount.Email, NewPassword))
            {
                // Set the new password for the selected account.
                SelectedAccount.Password = NewPassword;

                ResetPasswords();
                return true;
            }
            else
            {
                // Set the status message and make it visible.
                DialogStatusVisibility = Visibility.Visible;
                DialogStatusMessage = "Password could not be updated";
                ResetPasswords();
                return false;
            }
        }

        private void ResetPasswords()
        {
            // Reset the password properties.
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;
        }
    }
}
