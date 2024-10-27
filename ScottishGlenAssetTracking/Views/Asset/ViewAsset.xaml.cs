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
    public sealed partial class ViewAsset : Page
    {
        private Models.Asset _selectedAsset;

        public ViewAsset()
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
            if (EmployeeSelect.SelectedItem != null)
            {
                AssetSelect.ItemsSource = new AssetService().GetAssets(((Models.Employee)EmployeeSelect.SelectedItem).Id);
            }
        }

        private void AssetSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PopulateAssetDetails();
        }

        private void PopulateAssetDetails()
        {
            var asset = (Models.Asset)AssetSelect.SelectedItem;
            if (asset != null)
            {
                AssetName.Text = asset.Name;
                AssetModel.Text = asset.Model;
                AssetManufacturer.Text = asset.Manufacturer;
                AssetType.Text = asset.Type;
                AssetIpAddress.Text = asset.IpAddress;
                AssetPurchaseDate.Text = asset.PurchaseDate.ToString();
                AssetNotes.Text = asset.Notes;
                _selectedAsset = asset;
            }
            else
            {
                AssetName.Text = string.Empty;
                AssetModel.Text = string.Empty;
                AssetManufacturer.Text = string.Empty;
                AssetType.Text = string.Empty;
                AssetIpAddress.Text = string.Empty;
                AssetPurchaseDate.Text = string.Empty;
                AssetNotes.Text = string.Empty;
                _selectedAsset = null;
            }
        }

        private void DeleteAssetButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedAsset != null)
            {
                new AssetService().DeleteAsset(_selectedAsset.Id);
                DeleteAssetStatus.Text = "Asset Deleted";
                PopulateAssetDetails();
                AssetSelect.ItemsSource = new AssetService().GetAssets(((Models.Employee)EmployeeSelect.SelectedItem).Id);
            }
            else
            {
                DeleteAssetStatus.Text = "No Asset Selected";
            }
        }
    }
}
