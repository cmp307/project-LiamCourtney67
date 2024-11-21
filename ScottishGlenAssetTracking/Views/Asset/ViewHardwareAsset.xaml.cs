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

namespace ScottishGlenAssetTracking.Views.Asset
{
    /// <summary>
    /// Page for viewing an HardwareAsset.
    /// </summary>
    public sealed partial class ViewHardwareAsset : Page
    {
        /// <summary>
        /// Constructor for the ViewHardwareAsset class.
        /// </summary>
        public ViewHardwareAsset()
        {
            this.InitializeComponent();

            // Set the DataContext of the page to the ViewHardwareAssetViewModel with dependency injection.
            DataContext = App.AppHost.Services.GetRequiredService<ViewHardwareAssetViewModel>();
        }

        /// <summary>
        /// Event handler for the selection changed event of the DepartmentSelect ComboBox.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void DepartmentSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if the DataContext is a ViewHardwareAssetViewModel and execute the LoadEmployeesCommand.
            // Selection changed cannot be bound to a command, so it is done in the code-behind.
            if (DataContext is ViewHardwareAssetViewModel viewModel)
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
            // Check if the DataContext is a ViewHardwareAssetViewModel and execute the LoadAssetsCommand.
            // Selection changed cannot be bound to a command, so it is done in the code-behind.
            if (DataContext is ViewHardwareAssetViewModel viewModel) {
                viewModel.LoadHardwareAssetsCommand.Execute(null);
            }
        }

        /// <summary>
        /// Event handler for the selection changed event of the AssetSelect ComboBox.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void HardwareAssetSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if the DataContext is a ViewHardwareAssetViewModel and execute the PopulateAssetDetailsCommand.
            // Selection changed cannot be bound to a command, so it is done in the code-behind.
            if (DataContext is ViewHardwareAssetViewModel viewModel)
            {
                viewModel.PopulateHardwareAssetDetailsCommand.Execute(null);
            }
        }

        private void HardwareAssetDepartmentSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Not implemented
        }

        private void HardwareAssetEmployeeSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Not implemented
        }
    }
}
