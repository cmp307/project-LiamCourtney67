using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using ScottishGlenAssetTracking.Models;
using ScottishGlenAssetTracking.Services;
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
                .Where(d => d.Name != "Assets without Employee"));

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
            // Add the new employee to the database.
            _employeeService.AddEmployee(NewEmployee);

            // Set the status message and make it visible.
            StatusMessage = "Employee Added";
            StatusVisibility = Visibility.Visible;
        }
    }
}
