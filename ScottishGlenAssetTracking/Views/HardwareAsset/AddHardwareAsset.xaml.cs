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
using System.Management;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScottishGlenAssetTracking.Views.HardwareAsset
{
    /// <summary>
    /// Page for adding an HardwareAsset.
    /// </summary>
    public sealed partial class AddHardwareAsset : Page
    {
        /// <summary>
        /// Constructor for the AddHardwareAsset class.
        /// </summary>
        public AddHardwareAsset()
        {
            this.InitializeComponent();

            // Set the DataContext of the page to the AddHardwareAssetViewModel with dependency injection.
            DataContext = App.AppHost.Services.GetRequiredService<AddHardwareAssetViewModel>();
        }

        /// <summary>
        /// Event handler for the selection changed event of the DepartmentSelect ComboBox.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void DepartmentSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if the DataContext is an AddHardwareAssetViewModel and execute the LoadEmployeesCommand.
            // Selection changed cannot be bound to a command, so it is done in the code-behind.
            if (DataContext is AddHardwareAssetViewModel viewModel)
            {
                viewModel.LoadEmployeesCommand.Execute(null);
            }
        }
    }
}
