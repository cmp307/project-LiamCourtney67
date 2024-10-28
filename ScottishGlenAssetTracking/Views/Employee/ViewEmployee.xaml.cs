using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using ScottishGlenAssetTracking.Models;
using ScottishGlenAssetTracking.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScottishGlenAssetTracking.Views.Employee
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewEmployee : Page
    {
        private Models.Employee _selectedEmployee;

        public ViewEmployee()
        {
            this.InitializeComponent();
            List<Department> departments = new DepartmentService().GetDepartments();
            DepartmentSelect.ItemsSource = departments;
            EmployeeDepartmentSelect.ItemsSource = departments;
        }
        private void DepartmentSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DepartmentSelect.SelectedItem != null)
            {
                EmployeeSelect.ItemsSource = new EmployeeService().GetEmployees(((Department)DepartmentSelect.SelectedItem).Id);
            }
        }

        private void EmployeeSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateEmployeeDetails();
            ViewEmployeeView.Visibility = Visibility.Visible;
        }

        private void PopulateEmployeeDetails()
        {
            var employee = (Models.Employee)EmployeeSelect.SelectedItem;
            if (employee != null)
            {
                EmployeeFirstName.Text = $"First Name: {employee.FirstName}";
                EmployeeLastName.Text = $"Last Name: {employee.LastName}";
                EmployeeEmail.Text = $"Email: {employee.Email}";
                EmployeeDepartment.Text = $"Department: {employee.Department.Name}";

                _selectedEmployee = employee;
            }
            else
            {
                EmployeeFirstName.Text = "First Name: ";
                EmployeeLastName.Text = "Last Name: ";
                EmployeeEmail.Text = "Email: ";
                EmployeeDepartment.Text = "Department: ";

                _selectedEmployee = null;
            }
            PopulateEmployeeInputs(employee);
        }

        private void PopulateEmployeeInputs(Models.Employee employee)
        {
            if (employee != null) {
                EmployeeFirstNameInput.Text = _selectedEmployee.FirstName;
                EmployeeLastNameInput.Text = _selectedEmployee.LastName;
                EmployeeEmailInput.Text = _selectedEmployee.Email;
                EmployeeAssets.ItemsSource = _selectedEmployee.Assets;

                // Set the employee department in the dropdown.
                EmployeeDepartmentSelect.SelectedItem = ((List<Department>)EmployeeDepartmentSelect.ItemsSource)
                    .FirstOrDefault(d => d.Id == employee.Department.Id);
            }
            else
            {
                EmployeeFirstNameInput.Text = string.Empty;
                EmployeeLastNameInput.Text = string.Empty;
                EmployeeEmailInput.Text = string.Empty;
                EmployeeDepartmentSelect.SelectedItem = null;
            }
        }

        private void DeleteEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEmployee != null)
            {
                new EmployeeService().DeleteEmployee(_selectedEmployee.Id);
                Status.Text = "Employee Deleted";
                PopulateEmployeeDetails();
                EmployeeSelect.ItemsSource = new EmployeeService().GetEmployees(((Department)DepartmentSelect.SelectedItem).Id);
            }
            else
            {
                Status.Text = "No Employee Selected";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            EditEmployeeView.Visibility = Visibility.Collapsed;
            ViewEmployeeView.Visibility = Visibility.Visible;
        }
        private void EditEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            ViewEmployeeView.Visibility = Visibility.Collapsed;
            EditEmployeeView.Visibility = Visibility.Visible;
        }
        private void UpdateEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedEmployee.FirstName = EmployeeFirstNameInput.Text;
            _selectedEmployee.LastName = EmployeeLastNameInput.Text;
            _selectedEmployee.Email = EmployeeEmailInput.Text;
            _selectedEmployee.Department = (Department)EmployeeDepartmentSelect.SelectedItem;
            new EmployeeService().UpdateEmployee(_selectedEmployee);
            Status.Text = "Employee Updated";
            PopulateEmployeeDetails();
            EditEmployeeView.Visibility = Visibility.Collapsed;
            ViewEmployeeView.Visibility = Visibility.Visible;
            PopulateEmployeeInputs(_selectedEmployee);
        }

        private void EmployeeDepartmentSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
