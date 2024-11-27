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
        private readonly DepartmentService _departmentService;
        private readonly EmployeeService _employeeService;
        private readonly AccountService _accountService;

        private ContentDialog _updatePasswordDialog;
        private ContentDialog _setAdminDialog;
        private ContentDialog _setEmployeeDialog;

        public ManageAccountsViewModel(DepartmentService departmentService, EmployeeService employeeService, AccountService accountService)
        {
            _departmentService = departmentService;
            _employeeService = employeeService;
            _accountService = accountService;

            AdminAccounts = new ObservableCollection<Account>(_accountService.GetAccounts(5, false));

            AllAccounts = new ObservableCollection<Account>(_accountService.GetAccounts());

            Departments = new ObservableCollection<Department>(_departmentService.GetDepartments().Where(d => d.Name != "HardwareAssets without Employee"));
        }

        // Collections.

        public ObservableCollection<Account> AdminAccounts { get; private set; }

        public ObservableCollection<Account> AllAccounts { get; private set; }

        public ObservableCollection<Department> Departments { get; }

        public ObservableCollection<Employee> Employees { get; private set; }

        // Properties.

        [ObservableProperty]
        private Account selectedAdminAccount;

        [ObservableProperty]
        private Account selectedUpdatePasswordAccount;

        [ObservableProperty]
        private Account selectedSetEmployeeAccount;

        [ObservableProperty]
        private Department selectedDepartment;

        [ObservableProperty]
        private Employee selectedEmployee;

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

        // Dialog commands.

        public void SetUpdatePasswordDialog(ContentDialog dialog) => _updatePasswordDialog = dialog;

        [RelayCommand]
        private async Task ShowUpdatePasswordDialog()
        {
            // Set the dialog visibility properties.
            if (SelectedUpdatePasswordAccount != null)
            {
                await _updatePasswordDialog.ShowAsync();
                DialogStatusVisibility = Visibility.Collapsed;
            }
        }

        public bool UpdatePassword()
        {
            // Update the password in the database.
            if (_accountService.UpdatePassword(SelectedUpdatePasswordAccount.Email, NewPassword))
            {
                // Set the new password for the selected account.
                SelectedUpdatePasswordAccount.Password = NewPassword;

                ResetSelectedProperties();
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

        public void SetSetAdminDialog(ContentDialog dialog) => _setAdminDialog = dialog;

        [RelayCommand]
        private async Task ShowSetAdminDialog()
        {
            // Show the dialog if an account is selected.
            if (SelectedAdminAccount != null)
            {
                await _setAdminDialog.ShowAsync();
            }
        }

        public bool SetAccountToAdmin()
        {
            if (SelectedAdminAccount != null)
            {
                if (_accountService.SetAccountToAdmin(SelectedAdminAccount.Email))
                {
                    ResetSelectedProperties();
                    AllAccounts = new ObservableCollection<Account>(_accountService.GetAccounts());
                    OnPropertyChanged(nameof(AllAccounts));
                    return true;
                }
            }

            return false;
        }

        public void SetSetEmployeeDialog(ContentDialog dialog) => _setEmployeeDialog = dialog;

        [RelayCommand]
        private async Task ShowSetEmployeeDialog()
        {
            // Show the dialog if an account is selected.
            if (SelectedSetEmployeeAccount != null)
            {
                await _setEmployeeDialog.ShowAsync();
            }
        }

        public void LoadEmployees()
        {
            if (SelectedDepartment != null)
            {
                // Only load employees that do not have an account.
                Employees = new ObservableCollection<Employee>(_employeeService.GetEmployees(SelectedDepartment.Id).Where(e => e.Account == null));
                OnPropertyChanged(nameof(Employees));
            }
        }

        public bool SetAccountEmployee()
        {
            if (SelectedSetEmployeeAccount == null)
            {
                return false;
            }

            if (SelectedEmployee == null)
            {
                return false;
            }

            if (_accountService.SetAccountEmployee(SelectedSetEmployeeAccount.Email, SelectedEmployee))
            {
                ResetSelectedProperties();
                AdminAccounts = new ObservableCollection<Account>(_accountService.GetAccounts(5, false));
                OnPropertyChanged(nameof(AdminAccounts));
                return true;
            }

            return false;
        }

        private void ResetSelectedProperties()
        {
            SelectedAdminAccount = null;
            SelectedUpdatePasswordAccount = null;
            SelectedSetEmployeeAccount = null;
            SelectedDepartment = null;
            SelectedEmployee = null;
        }
    }
}
