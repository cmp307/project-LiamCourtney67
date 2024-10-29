using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using ScottishGlenAssetTracking.Data;
using ScottishGlenAssetTracking.Services;
using ScottishGlenAssetTracking.Models;
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
    public sealed partial class AddEmployee : Page
    {
        public AddEmployee()
        {
            this.InitializeComponent();

            List<Department> departments = new DepartmentService().GetDepartments();

            departments.Remove(departments.Find(d => d.Name == "Assets without Employee"));

            DepartmentSelect.ItemsSource = departments;
        }

        private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
        {
            CreateEmployee();
            AddEmployeeStatus.Visibility = Visibility.Visible;
            AddEmployeeStatus.Text = "Employee Added";
        }

        private void CreateEmployee()
        {
            var employee = new Models.Employee
            {
                FirstName = EmployeeFirstName.Text,
                LastName = EmployeeLastName.Text,
                Email = EmployeeEmail.Text,
                Department = (Department)DepartmentSelect.SelectedItem
            };
            new EmployeeService().AddEmployee(employee);
        }
    }
}
