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

namespace ScottishGlenAssetTracking.Views.Account
{
    /// <summary>
    /// Page for registering a new user.
    /// </summary>
    public sealed partial class Register : Page
    {
        /// <summary>
        /// Constructor for the Register class.
        /// </summary>
        public Register()
        {
            this.InitializeComponent();

            // Set the DataContext of the page to the RegisterViewModel with dependency injection.
            DataContext = App.AppHost.Services.GetRequiredService<RegisterViewModel>();
        }
    }
}
