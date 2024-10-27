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
            DepartmentSelect.ItemsSource = new DepartmentService().GetDepartments();
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
        }

        private void PopulateEmployeeDetails()
        {
            var employee = (Models.Employee)EmployeeSelect.SelectedItem;
            if (employee != null)
            {
                EmployeeFirstName.Text = employee.FirstName;
                EmployeeLastName.Text = employee.LastName;
                EmployeeEmail.Text = employee.Email;
                _selectedEmployee = employee;
            }
            else
            {
                EmployeeFirstName.Text = string.Empty;
                EmployeeLastName.Text = string.Empty;
                EmployeeEmail.Text = string.Empty;
                _selectedEmployee = null;
            }
        }

        private void DeleteEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEmployee != null)
            {
                new EmployeeService().DeleteEmployee(_selectedEmployee.Id);
                DeleteEmployeeStatus.Text = "Employee Deleted";
                PopulateEmployeeDetails();
                EmployeeSelect.ItemsSource = new EmployeeService().GetEmployees(((Department)DepartmentSelect.SelectedItem).Id);
            }
            else
            {
                DeleteEmployeeStatus.Text = "No Employee Selected";
            }
        }
    }
}
