using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
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

        // Visibility properties.

        [ObservableProperty]
        private Visibility statusVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility viewAccountViewVisibility = Visibility.Visible;

        [ObservableProperty]
        private Visibility editAccountViewVisibility = Visibility.Collapsed;

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

                // Change the view to the login page.
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
    }
}
