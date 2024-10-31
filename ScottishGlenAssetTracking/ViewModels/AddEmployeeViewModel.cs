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
    public partial class AddEmployeeViewModel : ObservableObject
    {
        private readonly DepartmentService _departmentService;
        private readonly EmployeeService _employeeService;

        public AddEmployeeViewModel()
        {
            // Initialize services
            _departmentService = new DepartmentService();
            _employeeService = new EmployeeService();

            // Load departments and remove any unwanted items
            Departments = new ObservableCollection<Department>(_departmentService.GetDepartments()
                .Where(d => d.Name != "Assets without Employee"));

            // Initialize new employee
            newEmployee = new Employee();

            // Initialize commands
            AddEmployeeCommand = new RelayCommand(AddEmployee);
        }

        // Collections
        public ObservableCollection<Department> Departments { get; }

        // Properties
        [ObservableProperty]
        private Employee newEmployee;

        [ObservableProperty]
        private string statusMessage;

        // Visibility properties
        [ObservableProperty]
        private Visibility statusVisibility = Visibility.Collapsed;

        // Commands
        public RelayCommand AddEmployeeCommand { get; }

        private void AddEmployee()
        {
            _employeeService.AddEmployee(NewEmployee);

            StatusMessage = "Employee Added";
            StatusVisibility = Visibility.Visible;
        }
    }
}
