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

namespace ScottishGlenAssetTracking.Views.Asset
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddAsset : Page
    {
        public AddAsset()
        {
            this.InitializeComponent();
            DepartmentSelect.ItemsSource = new DepartmentService().GetDepartments();
        }

        private void DepartmentSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EmployeeSelect.ItemsSource = new EmployeeService().GetEmployees(((Department)DepartmentSelect.SelectedItem).Id);
        }

        private void AddAssetButton_Click(object sender, RoutedEventArgs e)
        {
            CreateAsset();
            AddAssetStatus.Text = "Asset Added";
        }

        private void CreateAsset()
        {
            var asset = new Models.Asset
            {
                Name = AssetName.Text,
                Model = AssetModel.Text,
                Manufacturer = AssetManufacturer.Text,
                Type = AssetType.Text,
                IpAddress = AssetIpAddress.Text,
                PurchaseDate = AssetPurchaseDate.Date.Date,
                Notes = AssetNotes.Text,
                Employee = (Models.Employee)EmployeeSelect.SelectedItem
            };
            new AssetService().AddAsset(asset);
        }
    }
}
