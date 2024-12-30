using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
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

namespace ScottishGlenAssetTracking.Views.SoftwareAsset
{
    /// <summary>
    /// Page for viewing an SoftwareAsset.
    /// </summary>
    public sealed partial class ViewSoftwareAsset : Page
    {
        /// <summary>
        /// Constructor for the ViewSoftwareAsset class.
        /// </summary>
        public ViewSoftwareAsset()
        {
            this.InitializeComponent();

            // Set the DataContext of the page to the ViewSoftwareAssetViewModel with dependency injection.
            DataContext = App.AppHost.Services.GetRequiredService<ViewSoftwareAssetViewModel>();

            // Set the dialogs to the corresponding properties in the ViewModel.
            if (DataContext is ViewSoftwareAssetViewModel viewModel)
            {
                viewModel.SetDeleteSoftwareAssetDialog(DeleteSoftwareAssetDialog);
            }
        }

        /// <summary>
        /// Event handler for the selection changed event of the SoftwareAssetSelect ComboBox.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void SoftwareAssetSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if the DataContext is a ViewSoftwareAssetViewModel and execute the PopulateSoftwareAssetDetailsCommand.
            // Selection changed cannot be bound to a command, so it is done in the code-behind.
            if (DataContext is ViewSoftwareAssetViewModel viewModel)
            {
                viewModel.PopulateSoftwareAssetDetailsCommand.Execute(null);
            }
        }

        /// <summary>
        /// Event handler for the primary button click event of the DeleteSoftwareAssetDialog.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="args">Event data that provides information about the primary button click event.</param>
        private void DeleteSoftwareAssetDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Check if the DataContext is a ViewSoftwareAssetViewModel and execute the DeleteSoftwareAsset method.
            if (DataContext is ViewSoftwareAssetViewModel viewModel)
            {
                args.Cancel = false;
                viewModel.DeleteSoftwareAssetCommand.Execute(null);
            }
        }
    }
}
