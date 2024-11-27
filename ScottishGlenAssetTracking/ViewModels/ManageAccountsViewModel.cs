using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ScottishGlenAssetTracking.Models;
using ScottishGlenAssetTracking.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.ViewModels
{
    public partial class ManageAccountsViewModel : ObservableObject
    {
        private readonly AccountService _accountService;

        private ContentDialog _updatePasswordDialog;

        public ManageAccountsViewModel(AccountService accountService)
        {
            _accountService = accountService;

            AdminAccounts = new ObservableCollection<Account>(_accountService.GetAccounts(5, false));

            AllAccounts = new ObservableCollection<Account>(_accountService.GetAccounts());
        }

        // Collections.

        [ObservableProperty]
        private ObservableCollection<Account> adminAccounts;

        [ObservableProperty]
        private ObservableCollection<Account> allAccounts;

        // Properties.

        [ObservableProperty]
        private Account selectedAdminAccount;

        [ObservableProperty]
        private Account selectedAccount;

        [ObservableProperty]
        private string statusMessage;

        // Dialog properties.

        [ObservableProperty]
        private string newPassword;

        [ObservableProperty]
        private string dialogStatusMessage;

        // Visibility properties.

        [ObservableProperty]
        private Visibility statusVisibility = Visibility.Visible;

        // Dialog visibility properties.

        [ObservableProperty]
        private Visibility dialogStatusVisibility = Visibility.Collapsed;

        // Commands.

        [RelayCommand]
        private void SetAccountToAdmin()
        {
            if (SelectedAdminAccount != null)
            {
                if (_accountService.SetAccountToAdmin(SelectedAdminAccount.Email))
                {
                    StatusMessage = $"{SelectedAdminAccount.EmployeeName} set to Admin";
                    StatusVisibility = Visibility.Visible;
                }
            }
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
            // Update the password in the database.
            if (_accountService.UpdatePassword(SelectedAccount.Email, NewPassword))
            {
                // Set the new password for the selected account.
                SelectedAccount.Password = NewPassword;

                NewPassword = string.Empty;
                return true;
            }
            else
            {
                // Set the status message and make it visible.
                DialogStatusVisibility = Visibility.Visible;
                DialogStatusMessage = "Password could not be updated";

                NewPassword = string.Empty;
                return false;
            }
        }
    }
}
