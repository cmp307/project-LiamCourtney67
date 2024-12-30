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
    /// Partial class for the ViewEmployeeViewModel using the ObservableObject class.
    /// </summary>
    public partial class ViewEmployeeViewModel : ObservableObject
    {
        // Private fields for the DepartmentService and EmployeeService.
        private readonly DepartmentService _departmentService;
        private readonly EmployeeService _employeeService;
        private readonly AccountService _accountService;

        // Private field for the Account.
        private readonly Account _account;

        // Private field for the dialog.
        private ContentDialog _deleteEmployeeDialog;

        /// <summary>
        /// Constructor for the ViewEmployeeViewModel class using the DepartmentService and EmployeeService with dependency injection.
        /// </summary>
        /// <param name="departmentService">DepartmentService from dependency injection.</param>
        /// <param name="employeeService">EmployeeService from dependency injection.</param>
        /// <param name="accountService">AccountService from dependency injection.</param>
        public ViewEmployeeViewModel(DepartmentService departmentService, EmployeeService employeeService, AccountService accountService)
        {
            // Get the current account from the AccountManager.
            _account = App.AppHost.Services.GetRequiredService<AccountManager>().CurrentAccount;

            // Initialize services.
            _departmentService = departmentService;
            _employeeService = employeeService;
            _accountService = accountService;

            // Load departments and remove any unwanted items.
            Departments = new ObservableCollection<Department>(_departmentService.GetDepartments()
                .Where(d => d.Name != "HardwareAssets without Employee"));

            // Initialize collections.
            Employees = new ObservableCollection<Employee>();

            // Load selections for the account type.
            LoadSelectionsForAccountType();
        }

        // Collections.

        /// <summary>
        /// ObservableCollection of Department objects used in the view.
        /// </summary>
        public ObservableCollection<Department> Departments { get; }

        /// <summary>
        /// ObservableCollection of Employee objects used in the view.
        /// </summary>
        public ObservableCollection<Employee> Employees { get; private set; }

        // Properties.
        [ObservableProperty]
        private Department selectedDepartment;

        [ObservableProperty]
        private Employee selectedEmployee;

        [ObservableProperty]
        private string firstName;

        [ObservableProperty]
        private string lastName;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string statusMessage;

        // Visibility properties.
        [ObservableProperty]
        private Visibility selectsVisibility = Visibility.Visible;

        [ObservableProperty]
        private Visibility statusVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility employeeHardwareAssetsVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility viewEmployeeViewVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility editEmployeeViewVisibility = Visibility.Collapsed;

        // IsEnabled properties.
        [ObservableProperty]
        private bool departmentSelectIsEnabled = true;

        [ObservableProperty]
        private bool employeeSelectIsEnabled = true;

        [ObservableProperty]
        private bool employeeDepartmentSelectIsEnabled = true;


        // IsEnabled properties.
        [ObservableProperty]
        private bool emailIsEnabled = true;

        //Commands

        /// <summary>
        /// Command to load employees based on the selected department.
        /// </summary>
        [RelayCommand]
        private void LoadEmployees()
        {
            // Only load employees if a department is selected.
            if (SelectedDepartment != null)
            {
                // Get employees based on the selected department.
                var employees = _employeeService.GetEmployees(SelectedDepartment.Id);
                Employees = new ObservableCollection<Employee>(employees);

                // Notify the view that the Employees collection has changed.
                OnPropertyChanged(nameof(Employees));
            }
        }

        /// <summary>
        /// Command to populate the employee details.
        /// </summary>
        [RelayCommand]
        private void PopulateEmployeeDetails()
        {
            // Hide the Employees Hardware Assets visibility.
            EmployeeHardwareAssetsVisibility = Visibility.Collapsed;

            // Only populate the employee details if an employee is selected.
            if (SelectedEmployee != null)
            {
                // Set the properties to the Employee's properties.
                FirstName = SelectedEmployee.FirstName;
                LastName = SelectedEmployee.LastName;
                Email = SelectedEmployee.Email;

                // Set the selected employee's department to the department from the Departments collection and notify the view.
                SelectedEmployee.Department = Departments.FirstOrDefault(d => d.Id == SelectedEmployee.Department.Id);
                OnPropertyChanged(nameof(SelectedEmployee));

                // Clear the status message and make it hidden.
                StatusVisibility = Visibility.Collapsed;
                StatusMessage = string.Empty;

                // Change the view to the view mode.
                ChangeViewToView();

                // Check if the selected employee has any hardware assets and set the visibility of the EmployeeHardwareAssetsVisibility property.
                if (SelectedEmployee.HardwareAssets == null || SelectedEmployee.HardwareAssets.Count == 0)
                {
                    EmployeeHardwareAssetsVisibility = Visibility.Collapsed;
                }
                else if (SelectedEmployee.HardwareAssets.Count > 0)
                {
                    EmployeeHardwareAssetsVisibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Command to delete an employee from the database.
        /// </summary>
        [RelayCommand]
        private void DeleteEmployee()
        {
            // Only delete an employee if an employee is selected.
            if (SelectedEmployee == null)
            {
                SetStatusMessage("Please select an employee.");
                return;
            }

            // Attempt to delete the employee, catching any exceptions that may occur.
            try
            {
                // Check if the employee is the current account.
                bool isCurrentAccount = false;

                if (_account.Employee != null)
                {
                    isCurrentAccount = SelectedEmployee.Id == _account.Employee.Id;
                }

                _employeeService.DeleteEmployee(SelectedEmployee.Id);
                SelectedEmployee = null;

                // If the employee is the current account, log out the account and navigate to the login page.
                if (isCurrentAccount)
                {
                    App.AppHost.Services.GetRequiredService<AccountManager>().Logout();
                    MainWindow.Frame.Navigate(typeof(Login));
                    return;
                }

                // Reload the employees.
                LoadEmployees();

                ChangeViewToView();
                SetStatusMessage("Employee deleted successfully.");

                // Hide the view for the employee.
                ViewEmployeeViewVisibility = Visibility.Collapsed;
            }
            catch (ArgumentException ex)
            {
                SetStatusMessage(ex.Message);
            }
            catch (MySqlException)
            {
                SetStatusMessage("There was an issue connecting to the database. Please try again later.");
            }
            catch (Exception ex)
            {
                SetStatusMessage("An unexpected error occurred. Please try again later.");
                SetStatusMessage(ex.Message);
            }
        }

        /// <summary>
        /// Command to update an employee in the database.
        /// </summary>
        [RelayCommand]
        private void UpdateEmployee()
        {
            // Only update an employee if an employee is selected.
            if (SelectedEmployee == null)
            {
                SetStatusMessage("Please select an employee.");
                return;
            }

            // Attempt to update the employee, catching any exceptions that may occur.
            try
            {
                // Set the selected employee's properties.
                SelectedEmployee.FirstName = FirstName;
                SelectedEmployee.LastName = LastName;
                SelectedEmployee.Email = Email;

                _employeeService.UpdateEmployee(SelectedEmployee);
                OnPropertyChanged(nameof(SelectedEmployee));

                // Keep a reference to the selected employee's Id.
                int selectedEmployeeId = SelectedEmployee.Id;

                // Set the selected employee's department to the department from the Departments collection and notify the view.
                SelectedDepartment = Departments.FirstOrDefault(d => d.Id == SelectedEmployee.Department.Id);
                LoadEmployees();

                // Set the selected employee to the employee with the selectedEmployeeId.
                SelectedEmployee = Employees.FirstOrDefault(e => e.Id == selectedEmployeeId);

                ChangeViewToView();
                ResetProperties();
                PopulateEmployeeDetails();
                SetStatusMessage("Employee updated successfully.");
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

        /// <summary>
        /// Command to change the view to the edit mode.
        /// </summary>
        [RelayCommand]
        private void ChangeViewToEdit()
        {
            // Only change the view to the edit mode if an employee is selected.
            if (SelectedEmployee != null)
            {
                // Set the visibility properties for the view.
                SelectsVisibility = Visibility.Collapsed;
                ViewEmployeeViewVisibility = Visibility.Collapsed;
                EditEmployeeViewVisibility = Visibility.Visible;

                EmailIsEnabled = !_accountService.EmployeeHasAccount(SelectedEmployee.Id);

                // If the email is not enabled, set the status message.
                if (!EmailIsEnabled)
                {
                    SetStatusMessage("Cannot update employee's email.");
                }

                // Check if the employee is the current account.
                if (_account.Employee != null)
                {
                    bool isCurrentAccount = SelectedEmployee.Id == _account.Employee.Id;

                    // If the employee is the current account, enable the email field.
                    if (isCurrentAccount)
                    {
                        SetStatusMessage("Please navigate to Account to update email.");
                    }
                }
            }
        }

        /// <summary>
        /// Command to change the view to the view mode.
        /// </summary>
        [RelayCommand]
        private void ChangeViewToView()
        {
            // Set the visibility properties for the view.
            EditEmployeeViewVisibility = Visibility.Collapsed;
            SelectsVisibility = Visibility.Visible;
            ViewEmployeeViewVisibility = Visibility.Visible;
            StatusMessage = string.Empty;
        }

        /// <summary>
        /// Load selections for the account type and disable the selects if the account is not an admin.
        /// </summary>
        private void LoadSelectionsForAccountType()
        {
            // Only load selections for the account type if the account is not an admin.
            if (!_account.IsAdmin)
            {
                // Set the selected department to the department from the Departments collection.
                SelectedDepartment = Departments.FirstOrDefault(d => d.Id == _account.Employee.Department.Id);

                // Load employees based on the selected department and set the selected employee to the employee from the Employees collection.
                LoadEmployees();
                SelectedEmployee = Employees.FirstOrDefault(e => e.Id == _account.Employee.Id);

                // Change the view to the view mode.
                ChangeViewToView();

                // Disable the selects.
                DepartmentSelectIsEnabled = false;
                EmployeeSelectIsEnabled = false;
                EmployeeDepartmentSelectIsEnabled = false;
            }
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

        /// <summary>
        /// Helper method to reset the properties of the view model.
        /// </summary>
        private void ResetProperties()
        {
            // Reset the properties of the view model.
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
        }

        /// <summary>
        /// Set the delete employee dialog.
        /// </summary>
        /// <param name="dialog">Dialog to be used.</param>
        public void SetDeleteEmployeeDialog(ContentDialog dialog) => _deleteEmployeeDialog = dialog;

        /// <summary>
        /// Command to show the delete employee dialog.
        /// </summary>
        [RelayCommand]
        private async Task ShowDeleteEmployeeDialog() => await _deleteEmployeeDialog.ShowAsync();
    }
}
