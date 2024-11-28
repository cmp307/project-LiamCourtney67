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
    /// Page for managing user accounts.
    /// </summary>
    public sealed partial class ManageAccounts : Page
    {
        /// <summary>
        /// Constructor for the ManageAccounts class.
        /// </summary>
        public ManageAccounts()
        {
            this.InitializeComponent();

            // Set the DataContext of the page to the ManageAccountsViewModel with dependency injection.
            DataContext = App.AppHost.Services.GetRequiredService<ManageAccountsViewModel>();

            // Set the dialog properties.
            if (DataContext is ManageAccountsViewModel viewModel)
            {
                viewModel.SetUpdatePasswordDialog(UpdatePasswordDialog);
                viewModel.SetSetAdminDialog(SetAdminDialog);
                viewModel.SetSetEmployeeDialog(SetEmployeeDialog);
            }
        }

        /// <summary>
        /// Event handler for the primary button click event of the UpdatePasswordDialog.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="args">Event data that provides information about the primary button click event.</param>
        private void UpdatePasswordDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Check if the DataContext is a ManageAccountsViewModel and execute the UpdatePassword method.
            if (DataContext is ManageAccountsViewModel viewModel)
            {
                bool isSuccess = viewModel.UpdatePassword();

                // If the password was updated successfully, close the dialog, deselect the account, and show a status message.
                if (isSuccess)
                {
                    args.Cancel = false;

                    viewModel.SelectedUpdatePasswordAccount = null;
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
        /// Event handler for the primary button click event of the SetAdminDialog.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="args">Event data that provides information about the primary button click event.</param>
        private void SetAdminDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Check if the DataContext is a ManageAccountsViewModel and execute the SetAccountToAdmin method.
            if (DataContext is ManageAccountsViewModel viewModel)
            {
                // If the account was set to an admin successfully, deselect the account, and show a status message.
                if (viewModel.SetAccountToAdmin())
                {
                    viewModel.SelectedAdminAccount = null;
                    viewModel.StatusVisibility = Visibility.Visible;
                    viewModel.StatusMessage = "Admin status updated successfully.";
                }
                else
                {
                    viewModel.StatusVisibility = Visibility.Visible;
                    viewModel.StatusMessage = "Admin status could not be updated.";
                }
            }
        }

        /// <summary>
        /// Event handler for the primary button click event of the SetEmployeeDialog.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="args">Event data that provides information about the primary button click event.</param>
        private void SetEmployeeDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Check if the DataContext is a ManageAccountsViewModel and execute the SetAccountEmployee method.
            if (DataContext is ManageAccountsViewModel viewModel)
            {
                bool isSuccess = viewModel.SetAccountEmployee();

                // If the account's employee was updated successfully, deselect the account, and show a status message.
                if (isSuccess)
                {
                    args.Cancel = false;

                    viewModel.SelectedSetEmployeeAccount = null;
                    viewModel.StatusVisibility = Visibility.Visible;
                    viewModel.StatusMessage = "Account's Employee updated successfully.";
                }
                else
                {
                    args.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Event handler for the selection changed event of the DepartmentSelect ComboBox.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Event data that provides information about the selection changed event.</param>
        private void DepartmentSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if the DataContext is a ManageAccountsViewModel and execute the LoadEmployees method.
            if (DataContext is ManageAccountsViewModel viewModel)
            {
                viewModel.LoadEmployees();
            }
        }
    }
}
