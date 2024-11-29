using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MySqlConnector;
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
    /// <summary>
    /// Partial class for the ManageAccountsViewModel using the ObservableObject class.
    /// </summary>
    public partial class ManageAccountsViewModel : ObservableObject
    {
        // Private fields for the services.
        private readonly DepartmentService _departmentService;
        private readonly EmployeeService _employeeService;
        private readonly AccountService _accountService;

        // Private fields for the dialogs.
        private ContentDialog _updatePasswordDialog;
        private ContentDialog _setAdminDialog;
        private ContentDialog _setEmployeeDialog;

        /// <summary>
        /// Constructor for the ManageAccountsViewModel class using the DepartmentService, EmployeeService, and AccountService with dependency injection.
        /// </summary>
        /// <param name="departmentService">DepartmentService from dependency injection.</param>
        /// <param name="employeeService">EmployeeService from dependency injection.</param>
        /// <param name="accountService">AccountService from dependency injection.</param>
        public ManageAccountsViewModel(DepartmentService departmentService, EmployeeService employeeService, AccountService accountService)
        {
            // Initialize services.
            _departmentService = departmentService;
            _employeeService = employeeService;
            _accountService = accountService;

            // Initialize collections.
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

        // Methods

        /// <summary>
        /// Helper method to set the status message.
        /// </summary>
        /// <param name="message">Message to be displayed.</param>
        private void SetStatusMessage(string message)
        {
            StatusMessage = message;
            StatusVisibility = Visibility.Visible;
        }

        // Dialog commands.

        /// <summary>
        /// Set the update password dialog.
        /// </summary>
        /// <param name="dialog">Dialog to be used.</param>
        public void SetUpdatePasswordDialog(ContentDialog dialog) => _updatePasswordDialog = dialog;

        /// <summary>
        /// Command to show the update password dialog.
        /// </summary>
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

        /// <summary>
        /// Method to update the password of the selected account.
        /// </summary>
        /// <returns>True if password updated, false if not.</returns>
        public bool UpdatePassword()
        {
            // Check if the new password is null or empty.
            if (NewPassword == null || NewPassword == string.Empty)
            {
                SetDialogStatusMessage("Please enter a new password.");
                return false;
            }

            // Try to update the password, catching any exceptions that may occur.
            try
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
                    SetDialogStatusMessage("Password could not be updated.");
                    NewPassword = string.Empty;
                    return false;
                }
            }
            catch (ArgumentException ex)
            {
                SetDialogStatusMessage(ex.Message);
                NewPassword = string.Empty;
                return false;
            }
            catch (MySqlException)
            {
                SetDialogStatusMessage("There was an issue connecting to the database. Please try again later.");
                NewPassword = string.Empty;
                return false;
            }
            catch (Exception)
            {
                SetDialogStatusMessage("An unexpected error occurred. Please try again later.");
                NewPassword = string.Empty;
                return false;
            }
        }

        /// <summary>
        /// Helper method to reset the password properties.
        /// </summary>
        /// <param name="message">Message to be displayed.</param>
        private void SetDialogStatusMessage(string message)
        {
            DialogStatusMessage = message;
            DialogStatusVisibility = Visibility.Visible;
            OnPropertyChanged(nameof(DialogStatusMessage));
            OnPropertyChanged(nameof(DialogStatusVisibility));
        }

        /// <summary>
        /// Set the set admin dialog.
        /// </summary>
        /// <param name="dialog">Dialog to be used.</param>
        public void SetSetAdminDialog(ContentDialog dialog) => _setAdminDialog = dialog;

        /// <summary>
        /// Command to show the show the admin dialog.
        /// </summary>
        [RelayCommand]
        private async Task ShowSetAdminDialog()
        {
            // Show the dialog if an account is selected.
            if (SelectedAdminAccount != null)
            {
                await _setAdminDialog.ShowAsync();
            }
        }

        /// <summary>
        /// Method to set the selected account to an admin.
        /// </summary>
        /// <returns>True if set to admin, false if not.</returns>
        public bool SetAccountToAdmin()
        {
            // Check if an account is selected.
            if (SelectedAdminAccount == null)
            {
                return false;
            }

            // Try to set the account to an admin, catching any exceptions that may occur.
            try
            {
                if (_accountService.SetAccountToAdmin(SelectedAdminAccount.Email))
                {
                    ResetSelectedProperties();

                    // Reload the accounts.
                    AllAccounts = new ObservableCollection<Account>(_accountService.GetAccounts());
                    OnPropertyChanged(nameof(AllAccounts));

                    return true;
                }
            }
            catch (ArgumentException ex)
            {
                SetStatusMessage(ex.Message);
                return false;
            }
            catch (MySqlException)
            {
                SetStatusMessage("There was an issue connecting to the database. Please try again later.");
                return false;
            }
            catch (Exception)
            {
                SetStatusMessage("An unexpected error occurred. Please try again later.");
                return false;
            }

            return false;
        }

        /// <summary>
        /// Set the set employee dialog.
        /// </summary>
        /// <param name="dialog">Dialog to be used.</param>
        public void SetSetEmployeeDialog(ContentDialog dialog) => _setEmployeeDialog = dialog;

        /// <summary>
        /// Command to show the show the employee dialog.
        /// </summary>
        [RelayCommand]
        private async Task ShowSetEmployeeDialog()
        {
            // Show the dialog if an account is selected.
            if (SelectedSetEmployeeAccount != null)
            {
                await _setEmployeeDialog.ShowAsync();
                DialogStatusVisibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Method to slow the employees for the selected department.
        /// </summary>
        public void LoadEmployees()
        {
            if (SelectedDepartment != null)
            {
                // Only load employees that do not have an account.
                Employees = new ObservableCollection<Employee>(_employeeService.GetEmployees(SelectedDepartment.Id).Where(e => e.Account == null));
                OnPropertyChanged(nameof(Employees));
            }
        }

        /// <summary>
        /// Method to set the Employee for an Account.
        /// </summary>
        /// <returns>True if Employee set, false if not.</returns>
        public bool SetAccountEmployee()
        {
            if (SelectedSetEmployeeAccount == null)
            {
                SetDialogStatusMessage("Please select an account.");
                return false;
            }

            if (SelectedDepartment == null)
            {
                SetDialogStatusMessage("Please select a department.");
                return false;
            }

            if (SelectedEmployee == null)
            {
                SetDialogStatusMessage("Please select an employee.");
                return false;
            }

            // Try to set the employee for the account, catching any exceptions that may occur.
            try
            {
                if (_accountService.SetAccountEmployee(SelectedSetEmployeeAccount.Email, SelectedEmployee))
                {
                    ResetSelectedProperties();

                    // Reload the accounts.
                    AdminAccounts = new ObservableCollection<Account>(_accountService.GetAccounts(5, false));
                    OnPropertyChanged(nameof(AdminAccounts));

                    return true;
                }
            }
            catch (ArgumentException ex)
            {
                SetDialogStatusMessage(ex.Message);
                return false;
            }
            catch (MySqlException)
            {
                SetDialogStatusMessage("There was an issue connecting to the database. Please try again later.");
                return false;
            }
            catch (Exception)
            {
                SetDialogStatusMessage("An unexpected error occurred. Please try again later.");
                return false;
            }

            return false;
        }

        /// <summary>
        /// Helper method to reset the selected properties.
        /// </summary>
        public void ResetSelectedProperties()
        {
            SelectedAdminAccount = null;
            SelectedUpdatePasswordAccount = null;
            SelectedSetEmployeeAccount = null;
            SelectedDepartment = null;
            SelectedEmployee = null;
        }
    }
}
