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
    public partial class ViewEmployeeViewModel : ObservableObject
    {
        private readonly DepartmentService _departmentService;
        private readonly EmployeeService _employeeService;

        public ViewEmployeeViewModel(DepartmentService departmentService, EmployeeService employeeService)
        {
            // Initialize services
            _departmentService = departmentService;
            _employeeService = employeeService;

            // Load departments and remove any unwanted items
            Departments = new ObservableCollection<Department>(_departmentService.GetDepartments()
                .Where(d => d.Name != "Assets without Employee"));

            // Initialize collections
            Employees = new ObservableCollection<Employee>();

            // Initialize commands
            SelectDepartmentCommand = new RelayCommand(LoadEmployees);
            SelectEmployeeCommand = new RelayCommand(PopulateEmployeeDetails);

            DeleteEmployeeCommand = new RelayCommand(DeleteEmployee);
            UpdateEmployeeCommand = new RelayCommand(UpdateEmployee);

            ChangeViewToEditCommand = new RelayCommand(ChangeViewToEdit);
            ChangeViewToViewCommand = new RelayCommand(ChangeViewToView);
        }

        // Collections
        public ObservableCollection<Department> Departments { get; }
        public ObservableCollection<Employee> Employees { get; private set; }

        // Properties
        [ObservableProperty]
        private Department selectedDepartment;

        [ObservableProperty]
        private Employee selectedEmployee;

        [ObservableProperty]
        private string statusMessage;

        // Visibility properties
        [ObservableProperty]
        private Visibility statusVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility viewEmployeeViewVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility editEmployeeViewVisibility = Visibility.Collapsed;

        // Commands
        public RelayCommand SelectDepartmentCommand { get; }
        public RelayCommand SelectEmployeeCommand { get; }
        public RelayCommand DeleteEmployeeCommand { get; }
        public RelayCommand UpdateEmployeeCommand { get; }
        public RelayCommand ChangeViewToEditCommand { get; }
        public RelayCommand ChangeViewToViewCommand { get; }

        private void LoadEmployees()
        {
            if (SelectedDepartment != null)
            {
                var employees = _employeeService.GetEmployees(SelectedDepartment.Id);
                Employees = new ObservableCollection<Employee>(employees);
                OnPropertyChanged(nameof(Employees));
            }
        }

        private void PopulateEmployeeDetails()
        {
            if (SelectedEmployee != null)
            {
                SelectedEmployee.Department = Departments.FirstOrDefault(d => d.Id == SelectedEmployee.Department.Id);
                OnPropertyChanged(nameof(SelectedEmployee));
                StatusVisibility = Visibility.Collapsed;
                StatusMessage = string.Empty;
                ChangeViewToView();
            }
        }

        private void DeleteEmployee()
        {
            if (SelectedEmployee != null)
            {
                _employeeService.DeleteEmployee(SelectedEmployee.Id);
                StatusVisibility = Visibility.Visible;
                StatusMessage = "Employee Deleted";
                LoadEmployees();
                ViewEmployeeViewVisibility = Visibility.Collapsed;
            }
            else
            {
                StatusMessage = "No Employee Selected";
            }
        }

        private void UpdateEmployee()
        {
            if (SelectedEmployee != null)
            {
                _employeeService.UpdateEmployee(SelectedEmployee);
                OnPropertyChanged(nameof(SelectedEmployee));

                int selectedEmployeeId = SelectedEmployee.Id;

                SelectedDepartment = Departments.FirstOrDefault(d => d.Id == SelectedEmployee.Department.Id);
                LoadEmployees();
                SelectedEmployee = Employees.FirstOrDefault(e => e.Id == selectedEmployeeId);

                StatusVisibility = Visibility.Visible;
                StatusMessage = "Employee Updated";

                ChangeViewToView();
            }
        }

        private void ChangeViewToEdit()
        {
            if (SelectedEmployee != null)
            {
                ViewEmployeeViewVisibility = Visibility.Collapsed;
                EditEmployeeViewVisibility = Visibility.Visible;
            }
        }

        private void ChangeViewToView()
        {
            EditEmployeeViewVisibility = Visibility.Collapsed;
            ViewEmployeeViewVisibility = Visibility.Visible;
        }
    }
}
