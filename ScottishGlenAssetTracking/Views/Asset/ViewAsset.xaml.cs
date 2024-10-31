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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewAsset : Page
    {
        private Models.Asset _selectedAsset;

        public ViewAsset()
        {
            this.InitializeComponent();
            this.DataContext = new ViewAssetViewModel();
        }

        private void DepartmentSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ViewAssetViewModel viewModel)
            {
                viewModel.SelectDepartmentCommand.Execute(null);
            }
        }

        private void EmployeeSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ViewAssetViewModel viewModel) {
                viewModel.SelectEmployeeCommand.Execute(null);
            }
        }

        private void AssetSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ViewAssetViewModel viewModel)
            {
                viewModel.SelectAssetCommand.Execute(null);
            }
        }

        private void AssetDepartmentSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void AssetEmployeeSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
