using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using ScottishGlenAssetTracking.Models;
using ScottishGlenAssetTracking.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace ScottishGlenAssetTracking.ViewModels
{
    public partial class AddEmployeeViewModel : ObservableObject
    {
        private readonly EmployeeService _employeeService;

        public AddEmployeeViewModel()
        {
            // Get the departments.
            DepartmentService departmentService = new DepartmentService();
            Departments = new ObservableCollection<Department>(departmentService.GetDepartments());

            // Remove the invalid department.
            Department invalidDepartment = Departments.FirstOrDefault(d => d.Name == "Assets without Employee");
            if (invalidDepartment != null)
            {
                Departments.Remove(invalidDepartment);
            }

            // Initialize the new employee, employee service, and command.
            newEmployee = new Employee();
            _employeeService = new EmployeeService();
            AddEmployeeCommand = new RelayCommand(AddEmployee);
        }

        [ObservableProperty]
        private Employee newEmployee;

        [ObservableProperty]
        private string statusMessage;

        [ObservableProperty]
        private Visibility statusVisibility = Visibility.Collapsed;

        public ObservableCollection<Department> Departments { get; }

        public RelayCommand AddEmployeeCommand { get; }

        private void AddEmployee()
        {
            _employeeService.AddEmployee(NewEmployee);

            StatusMessage = "Employee Added";
            StatusVisibility = Visibility.Visible;
        }
    }
}
