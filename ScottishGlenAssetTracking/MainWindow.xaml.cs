using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using ScottishGlenAssetTracking.Services;
using ScottishGlenAssetTracking.ViewModels;
using ScottishGlenAssetTracking.Views.HardwareAsset;
using ScottishGlenAssetTracking.Views.Employee;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using ScottishGlenAssetTracking.Views.SoftwareAsset;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScottishGlenAssetTracking
{
    /// <summary>
    /// Main window for the application and navigation view.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        /// <summary>
        /// Constructor for the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Event handler for the item invoked event of the navigation view.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the item invoked event.</param>
        private void Nav_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            // Check if the invoked item has a tag and navigate to the corresponding page.
            if (args.InvokedItemContainer.Tag is not null)
            {
                var itemTag = args.InvokedItemContainer.Tag.ToString();
                NavigateToPage(itemTag);
            }
        }

        /// <summary>
        /// Navigates to the selected page.
        /// </summary>
        /// <param name="pageTag">Tag for the invoked item.</param>
        private void NavigateToPage(string pageTag)
        {
            // Check the tag of the invoked item and navigate to the corresponding page using the MainFrame and dependency injection.
            switch (pageTag)
            {
                // HardwareAsset
                case "AddHardwareAsset":
                    var addHardwareAssetPage = App.AppHost.Services.GetRequiredService<AddHardwareAsset>();
                    MainFrame.Navigate(addHardwareAssetPage.GetType());
                    break;

                case "ViewHardwareAsset":
                    var viewHardwareAssetPage = App.AppHost.Services.GetRequiredService<ViewHardwareAsset>();
                    MainFrame.Navigate(viewHardwareAssetPage.GetType());
                    break;

                // SoftwareAsset
                case "AddSoftwareAsset":
                    var addSoftwareAssetPage = App.AppHost.Services.GetRequiredService<AddSoftwareAsset>();
                    MainFrame.Navigate(addSoftwareAssetPage.GetType());
                    break;

                case "ViewSoftwareAsset":
                    var viewSoftwareAssetPage = App.AppHost.Services.GetRequiredService<ViewSoftwareAsset>();
                    MainFrame.Navigate(viewSoftwareAssetPage.GetType());
                    break;

                // Employee
                case "AddEmployee":
                    var addEmployeePage = App.AppHost.Services.GetRequiredService<AddEmployee>();
                    MainFrame.Navigate(addEmployeePage.GetType());
                    break;

                case "ViewEmployee":
                    var viewEmployeePage = App.AppHost.Services.GetRequiredService<ViewEmployee>();
                    MainFrame.Navigate(viewEmployeePage.GetType());
                    break;

                default:
                    break;
            }
        }
    }
}
