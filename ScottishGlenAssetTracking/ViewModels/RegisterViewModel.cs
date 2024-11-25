using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
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
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly AccountService _accountService;

        public RegisterViewModel(AccountService accountService)
        {
            _accountService = accountService;

            NewAccount = new Account();
        }

        // Properties.

        [ObservableProperty]
        private Account newAccount;

        [ObservableProperty]
        private string confirmPassword;

        [ObservableProperty]
        private string statusMessage;

        // Visibility properties.
        [ObservableProperty]
        private Visibility statusVisibility = Visibility.Visible;

        // Commands

        [RelayCommand]
        public void Register()
        {
            if (NewAccount.VerifyPassword(ConfirmPassword))
            {
                if (_accountService.AddAccount(NewAccount))
                {
                    // Reset the new account and confirm password properties.
                    NewAccount = new Account();
                    ConfirmPassword = string.Empty;

                    StatusMessage = "Account added successfully";
                    StatusVisibility = Visibility.Visible;
                }
                else
                {
                    StatusMessage = "Failed to add account";
                    StatusVisibility = Visibility.Visible;
                }
            }
            else
            {
                StatusMessage = "Passwords do not match";
                StatusVisibility = Visibility.Visible;
            }
        }

        [RelayCommand]
        public void ReturnToLogin()
        {
            // Reset the new account and confirm password properties.
            NewAccount = new Account();
            ConfirmPassword = string.Empty;

            var loginPage = App.AppHost.Services.GetRequiredService<Login>();
            MainWindow.Frame.Navigate(typeof(Login));
        }
    }
}
