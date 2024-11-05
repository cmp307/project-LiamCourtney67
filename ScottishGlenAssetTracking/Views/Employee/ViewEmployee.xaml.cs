using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using ScottishGlenAssetTracking.Models;
using ScottishGlenAssetTracking.Services;
using ScottishGlenAssetTracking.ViewModels;
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
    /// Page for viewing an Employee.
    /// </summary>
    public sealed partial class ViewEmployee : Page
    {
        /// <summary>
        /// Constructor for the ViewEmployee class.
        /// </summary>
        public ViewEmployee()
        {
            this.InitializeComponent();

            // Set the DataContext of the page to the ViewEmployeeViewModel with dependency injection.
            DataContext = App.AppHost.Services.GetRequiredService<ViewEmployeeViewModel>();
        }

        /// <summary>
        /// Event handler for the selection changed event of the DepartmentSelect ComboBox.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void DepartmentSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if the DataContext is a ViewEmployeeViewModel and execute the LoadEmployeesCommand.
            // Selection changed cannot be bound to a command, so it is done in the code-behind.
            if (DataContext is ViewEmployeeViewModel viewModel)
            {
                viewModel.LoadEmployeesCommand.Execute(null);
            }
        }

        /// <summary>
        /// Event handler for the selection changed event of the EmployeeSelect ComboBox.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void EmployeeSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if the DataContext is a ViewEmployeeViewModel and execute the PopulateEmployeeDetailsCommand.
            // Selection changed cannot be bound to a command, so it is done in the code-behind.
            if (DataContext is ViewEmployeeViewModel viewModel)
            {
                viewModel.PopulateEmployeeDetailsCommand.Execute(null);
            }
        }
    }
}
