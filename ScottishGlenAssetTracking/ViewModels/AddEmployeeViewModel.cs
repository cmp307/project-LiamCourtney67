using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using MySqlConnector;
using ScottishGlenAssetTracking.Models;
using ScottishGlenAssetTracking.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Store;

namespace ScottishGlenAssetTracking.ViewModels
{
    /// <summary>
    /// Partial class for the AddEmployeeViewModel using the ObservableObject class.
    /// </summary>
    public partial class AddEmployeeViewModel : ObservableObject
    {
        // Private fields for the DepartmentService and EmployeeService.
        private readonly DepartmentService _departmentService;
        private readonly EmployeeService _employeeService;

        /// <summary>
        /// Constructor for the AddEmployeeViewModel class using the DepartmentService and EmployeeService with dependency injection.
        /// </summary>
        /// <param name="departmentService">DepartmentService from dependency injection.</param>
        /// <param name="employeeService">EmployeeService from dependency injection.</param>
        public AddEmployeeViewModel(DepartmentService departmentService, EmployeeService employeeService)
        {
            // Initialize services.
            _departmentService = departmentService;
            _employeeService = employeeService;

            // Load departments and remove any unwanted items.
            Departments = new ObservableCollection<Department>(_departmentService.GetDepartments()
                .Where(d => d.Name != "HardwareAssets without Employee"));

            // Initialize new employee.
            newEmployee = new Employee();
        }

        // Collections.

        /// <summary>
        /// ObservableCollection of Department objects used in the view.
        /// </summary>
        public ObservableCollection<Department> Departments { get; }

        // Properties.
        [ObservableProperty]
        private Employee newEmployee;

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
        private Visibility statusVisibility = Visibility.Collapsed;

        /// <summary>
        /// Command to add an employee to the database.
        /// </summary>
        [RelayCommand]
        private void AddEmployee()
        {
            if (NewEmployee.Department == null)
            {
                SetStatusMessage("Please select a department.");
                return;
            }

            if (FirstName == string.Empty || FirstName == null ||
                LastName == string.Empty || LastName == null ||
                Email == string.Empty || Email == null)
            {
                SetStatusMessage("Please fill in all fields.");
                return;
            }

            // Attempt to add the employee, catching any exceptions that may occur.
            try
            {
                // Set the new employee properties.
                NewEmployee.FirstName = FirstName;
                NewEmployee.LastName = LastName;
                NewEmployee.Email = Email;

                _employeeService.AddEmployee(NewEmployee);
                SetStatusMessage("Employee added successfully.");
                ResetProperties();
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
        /// Helper method to set the status message and make it visible.
        /// </summary>
        /// <param name="message">Message to be displayed.</param>
        private void SetStatusMessage(string message)
        {
            // Set the status message and make it visible.
            StatusMessage = message;
            StatusVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Helper method to reset the properties.
        /// </summary>
        private void ResetProperties()
        {
            // Reset the new employee, first name, last name, and email properties.
            NewEmployee = new Employee();
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
        }
    }
}
