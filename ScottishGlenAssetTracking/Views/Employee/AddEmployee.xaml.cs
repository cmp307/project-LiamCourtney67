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
using ScottishGlenAssetTracking.ViewModels;
using Microsoft.Extensions.DependencyInjection;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScottishGlenAssetTracking.Views.Employee
{
    /// <summary>
    /// Page for adding an Employee.
    /// </summary>
    public sealed partial class AddEmployee : Page
    {
        /// <summary>
        /// Constructor for the AddEmployee class.
        /// </summary>
        public AddEmployee()
        {
            this.InitializeComponent();

            // Set the DataContext of the page to the AddEmployeeViewModel with dependency injection.
            DataContext = App.AppHost.Services.GetRequiredService<AddEmployeeViewModel>();
        }
    }
}
