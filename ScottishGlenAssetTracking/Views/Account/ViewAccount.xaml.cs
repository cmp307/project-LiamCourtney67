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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewAccount : Page
    {
        public ViewAccount()
        {
            this.InitializeComponent();

            // Set the DataContext of the page to the ViewAccountViewModel with dependency injection.
            DataContext = App.AppHost.Services.GetRequiredService<ViewAccountViewModel>();

            // Set the dialog for the UpdatePasswordDialog to the UpdatePasswordDialog.
            if (DataContext is ViewAccountViewModel viewModel)
            {
                viewModel.SetDialog(UpdatePasswordDialog);
            }
        }

        private void UpdatePasswordDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (DataContext is ViewAccountViewModel viewModel)
            {
                bool isSuccess = viewModel.UpdatePassword();

                if (isSuccess)
                {
                    args.Cancel = true;

                    viewModel.ChangeViewToViewCommand.Execute(null);

                    viewModel.StatusVisibility = Visibility.Visible;
                    viewModel.StatusMessage = "Password updated successfully.";
                }

                args.Cancel = false;
            }
        }
    }
}
