using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using ScottishGlenAssetTracking.Models;
using ScottishGlenAssetTracking.Services;
using ScottishGlenAssetTracking.Views.Account;
using ScottishGlenAssetTracking.Views.HardwareAsset;
using ScottishGlenAssetTracking.Views.Portals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AccountService _accountService;

        public LoginViewModel(AccountService accountService)
        {
            _accountService = accountService;
        }

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string statusMessage;

        [ObservableProperty]
        private Visibility statusVisibility = Visibility.Visible;

        [RelayCommand]
        public void Login()
        {
            if (App.AppHost.Services.GetRequiredService<AccountManager>().Login(Email, Password))
            {
                // Reset the email and password properties.
                Email = string.Empty;
                Password = string.Empty;

                NavigateToPortal();
            }
            else
            {
                StatusMessage = "Login failed";
                StatusVisibility = Visibility.Visible;
            }
        }

        [RelayCommand]
        public void NavigateToRegister()
        {
            // Reset the email and password properties.
            Email = string.Empty;
            Password = string.Empty;

            var registerPage = App.AppHost.Services.GetRequiredService<Register>();
            MainWindow.Frame.Navigate(registerPage.GetType());
        }

        private void NavigateToPortal()
        {
            Account account = App.AppHost.Services.GetRequiredService<AccountManager>().CurrentAccount;

            if (account == null)
            {
                return;
            }
            else if (account.IsAdmin) {
                var adminPortal = App.AppHost.Services.GetRequiredService<AdminPortal>();
                MainWindow.Frame.Navigate(adminPortal.GetType());
            }
            else if (account.Employee != null)
            {
                var employeePortal = App.AppHost.Services.GetRequiredService<EmployeePortal>();
                MainWindow.Frame.Navigate(employeePortal.GetType());
            }
            else
            {
                var newUserPortal = App.AppHost.Services.GetRequiredService<NewUserPortal>();
                MainWindow.Frame.Navigate(newUserPortal.GetType());
            }
        }
    }
}
