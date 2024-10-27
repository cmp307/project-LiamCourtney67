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
using System.Management;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;

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
            string name = Environment.MachineName;
            string manufacturer = "Unknown";
            string model = "Unknown";
            string type = "Unknown";

            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    manufacturer = obj["Manufacturer"]?.ToString() ?? "Unknown";
                    model = obj["Model"]?.ToString() ?? "Unknown";
                    type = obj["SystemType"]?.ToString() ?? "Unknown";
                }
            }

            string ipAddress = Dns.GetHostEntry(Dns.GetHostName())
               .AddressList
               .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
               ?.ToString() ?? "No IP Found";

            var asset = new Models.Asset
            {
                Name = name,
                Model = model,
                Manufacturer = manufacturer,
                Type = type,
                IpAddress = ipAddress,
                PurchaseDate = AssetPurchaseDate.Date.Date,
                Notes = AssetNotes.Text,
                Employee = (Models.Employee)EmployeeSelect.SelectedItem
            };
            new AssetService().AddAsset(asset);
        }
    }
}
