using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using ScottishGlenAssetTracking.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ScottishGlenAssetTracking.Views.Account
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManageAccounts : Page
    {
        public ManageAccounts()
        {
            this.InitializeComponent();

            // Set the DataContext of the page to the ManageAccountsViewModel with dependency injection.
            DataContext = App.AppHost.Services.GetRequiredService<ManageAccountsViewModel>();

            // Set the dialog for the UpdatePasswordDialog to the UpdatePasswordDialog.
            if (DataContext is ManageAccountsViewModel viewModel)
            {
                viewModel.SetDialog(UpdatePasswordDialog);
            }
        }

        private void UpdatePasswordDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (DataContext is ManageAccountsViewModel viewModel)
            {
                bool isSuccess = viewModel.UpdatePassword();

                if (isSuccess)
                {
                    args.Cancel = true;

                    viewModel.StatusVisibility = Visibility.Visible;
                    viewModel.StatusMessage = "Password updated successfully.";
                }

                args.Cancel = false;
            }
        }
    }
}
