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
    /// Page for viewing an account.
    /// </summary>
    public sealed partial class ViewAccount : Page
    {
        /// <summary>
        /// Constructor for the ViewAccount class.
        /// </summary>
        public ViewAccount()
        {
            this.InitializeComponent();

            // Set the DataContext of the page to the ViewAccountViewModel with dependency injection.
            DataContext = App.AppHost.Services.GetRequiredService<ViewAccountViewModel>();

            // Set the dialogs to the corresponding properties in the ViewModel.
            if (DataContext is ViewAccountViewModel viewModel)
            {
                viewModel.SetUpdatePasswordDialog(UpdatePasswordDialog);
                viewModel.SetDeleteAccountDialog(DeleteAccountDialog);
            }
        }

        /// <summary>
        /// Event handler for the primary button click event of the UpdatePasswordDialog.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="args">Event data that provides information about the primary button click event.</param>
        private void UpdatePasswordDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Check if the DataContext is a ViewAccountViewModel and execute the UpdatePassword method.
            if (DataContext is ViewAccountViewModel viewModel)
            {
                bool isSuccess = viewModel.UpdatePassword();

                // If the password was updated successfully, close the dialog and show a status message.
                if (isSuccess)
                {
                    args.Cancel = false;

                    viewModel.ChangeViewToViewCommand.Execute(null);

                    viewModel.StatusVisibility = Visibility.Visible;
                    viewModel.StatusMessage = "Password updated successfully.";
                }
                else
                {
                    args.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Event handler for the primary button click event of the DeleteAccountDialog.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="args">Event data that provides information about the primary button click event.</param>
        private void DeleteAccountDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Check if the DataContext is a ViewAccountViewModel and execute the DeleteAccount method.
            if (DataContext is ViewAccountViewModel viewModel)
            {
                args.Cancel = false;
                viewModel.DeleteAccountCommand.Execute(null);
            }
        }
    }
}
