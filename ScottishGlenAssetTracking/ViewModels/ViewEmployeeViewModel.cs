using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
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
    /// Partial class for the ViewEmployeeViewModel using the ObservableObject class.
    /// </summary>
    public partial class ViewEmployeeViewModel : ObservableObject
    {
        // Private fields for the DepartmentService and EmployeeService.
        private readonly DepartmentService _departmentService;
        private readonly EmployeeService _employeeService;

        /// <summary>
        /// Constructor for the ViewEmployeeViewModel class using the DepartmentService and EmployeeService with dependency injection.
        /// </summary>
        /// <param name="departmentService">DepartmentService from dependency injection.</param>
        /// <param name="employeeService">EmployeeService from dependency injection.</param>
        public ViewEmployeeViewModel(DepartmentService departmentService, EmployeeService employeeService)
        {
            // Initialize services.
            _departmentService = departmentService;
            _employeeService = employeeService;

            // Load departments and remove any unwanted items.
            Departments = new ObservableCollection<Department>(_departmentService.GetDepartments()
                .Where(d => d.Name != "HardwareAssets without Employee"));

            // Initialize collections.
            Employees = new ObservableCollection<Employee>();
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
            // Only populate the employee details if an employee is selected.
            if (SelectedEmployee != null)
            {
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
            if (SelectedEmployee != null)
            {
                // Delete the selected employee from the database.
                _employeeService.DeleteEmployee(SelectedEmployee.Id);

                // Set the status message and make it visible.
                StatusVisibility = Visibility.Visible;
                StatusMessage = "Employee Deleted";

                // Reload the employees.
                LoadEmployees();

                // Change the view to the view mode.
                ChangeViewToView();
            }
            else
            {
                // Set the status message if no employee is selected.
                StatusVisibility = Visibility.Visible;
                StatusMessage = "No Employee Selected";
            }
        }

        /// <summary>
        /// Command to update an employee in the database.
        /// </summary>
        [RelayCommand]
        private void UpdateEmployee()
        {
            // Only update an employee if an employee is selected.
            if (SelectedEmployee != null)
            {
                // Update the selected employee in the database and notify the view.
                _employeeService.UpdateEmployee(SelectedEmployee);
                OnPropertyChanged(nameof(SelectedEmployee));

                // Keep a reference to the selected employee's Id.
                int selectedEmployeeId = SelectedEmployee.Id;

                // Set the selected employee's department to the department from the Departments collection and notify the view.
                SelectedDepartment = Departments.FirstOrDefault(d => d.Id == SelectedEmployee.Department.Id);
                LoadEmployees();

                // Set the selected employee to the employee with the selectedEmployeeId.
                SelectedEmployee = Employees.FirstOrDefault(e => e.Id == selectedEmployeeId);

                // Set the status message and make it visible.
                StatusVisibility = Visibility.Visible;
                StatusMessage = "Employee Updated";

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
            // Only change the view to the edit mode if an employee is selected.
            if (SelectedEmployee != null)
            {
                // Set the visibility properties for the view.
                SelectsVisibility = Visibility.Collapsed;
                ViewEmployeeViewVisibility = Visibility.Collapsed;
                EditEmployeeViewVisibility = Visibility.Visible;
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
        }
    }
}
