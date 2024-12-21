using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MySqlConnector;
using ScottishGlenAssetTracking.Models;
using ScottishGlenAssetTracking.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.ViewModels
{
    /// <summary>
    /// Partial class for the ViewSoftwareAssetViewModel using the ObservableObject class.
    /// </summary>
    public partial class ViewSoftwareAssetViewModel : ObservableObject
    {
        // Private fields for the DepartmentService, EmployeeService, HardwareAssetService, and SoftwareAssetService.
        private readonly DepartmentService _departmentService;
        private readonly EmployeeService _employeeService;
        private readonly HardwareAssetService _hardwareAssetService;
        private readonly SoftwareAssetService _softwareAssetService;

        // Private field for the Account.
        private readonly Account _account;

        /// <summary>
        /// Constructor for the ViewSoftwareAssetViewModel class using the DepartmentService, EmployeeService, and SoftwareAssetService with dependency injection.
        /// </summary>
        /// <param name="departmentService">DepartmentService from dependency injection.</param>
        /// <param name="employeeService">EmployeeService from dependency injection.</param>
        /// <param name="hardwareAssetService">HardwareAssetService from dependency injection.</param>
        /// <param name="softwareAssetService">SoftwareAssetService from dependency injection.</param>
        public ViewSoftwareAssetViewModel(DepartmentService departmentService,
                                          EmployeeService employeeService,
                                          HardwareAssetService hardwareAssetService,
                                          SoftwareAssetService softwareAssetService)
        {
            // Get the current account from the AccountManager.
            _account = App.AppHost.Services.GetRequiredService<AccountManager>().CurrentAccount;

            // Initialize services.
            _departmentService = departmentService;
            _employeeService = employeeService;
            _hardwareAssetService = hardwareAssetService;
            _softwareAssetService = softwareAssetService;

            // Load SoftwareAssets.
            LoadSoftwareAssets();
        }

        // Collections

        /// <summary>
        /// ObservableCollection of SoftwareAsset objects used in the view.
        /// </summary>
        public ObservableCollection<SoftwareAsset> SoftwareAssets { get; private set; }

        /// <summary>
        /// ObservableCollection of HardwareAssets belonging to the selected SoftwareAsset that are linked to the Employee.
        /// </summary>
        public ObservableCollection<HardwareAsset> SoftwareAssetHardwareAssets { get; private set; }

        /// <summary>
        /// ObservableCollection of Vulnerability objects used in the view.
        /// </summary>
        public ObservableCollection<Vulnerability> SoftwareAssetVulnerabilities { get; private set; }


        // Properties.
        [ObservableProperty]
        private SoftwareAsset selectedSoftwareAsset;

        [ObservableProperty]
        private SoftwareAsset updatedSoftwareAsset;

        [ObservableProperty]
        private string statusMessage;

        // Visibility properties.
        [ObservableProperty]
        private Visibility selectsVisibility = Visibility.Visible;

        [ObservableProperty]
        private Visibility statusVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility progressRingVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility viewSoftwareAssetViewVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility editSoftwareAssetViewVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility softwareAssetHardwareAssetsVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility softwareAssetVulnerabilitiesVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility buttonsVisibility = Visibility.Collapsed;

        // IsActive properties.
        [ObservableProperty]
        private bool progressRingIsActive;

        // Commands

        /// <summary>
        /// Helper method to load the SoftwareAssets.
        /// </summary>
        private void LoadSoftwareAssets()
        {
            // Load SoftwareAssets.
            SoftwareAssets = new ObservableCollection<SoftwareAsset>(_softwareAssetService.GetSoftwareAssets());
            OnPropertyChanged(nameof(SoftwareAssets));
        }

        /// <summary>
        /// Command to populate the SoftwareAsset details.
        /// </summary>
        [RelayCommand]
        private void PopulateSoftwareAssetDetails()
        {
            // Only populate the asset details if an asset is selected.
            if (SelectedSoftwareAsset == null)
            {
                return;
            }

            // Try to get the SoftwareAsset with the SystemInfo and handle any exceptions.
            try
            {
                // Get the SoftwareAsset with the SystemInfo and set the HardwareAssets.
                UpdatedSoftwareAsset = _softwareAssetService.GetSoftwareAssetWithSystemInfo();

                // Clear the status message and make it hidden.
                StatusVisibility = Visibility.Collapsed;
                StatusMessage = string.Empty;

                // Change the view to the view mode.
                ChangeViewToView();

                // Chck if the selected SoftwareAsset has any HardwareAssets and set the visibility of the SoftwareAssetHardwareAssetsVisibility property.
                if (SelectedSoftwareAsset.HardwareAssets == null || SelectedSoftwareAsset.HardwareAssets.Count == 0)
                {
                    SoftwareAssetHardwareAssetsVisibility = Visibility.Collapsed;
                }
                else if (SelectedSoftwareAsset.HardwareAssets.Count > 0)
                {
                    // Set the SoftwareAssetHardwareAssets collection to the HardwareAssets of the selected SoftwareAsset.
                    if (_account.IsAdmin)
                    {
                        SoftwareAssetHardwareAssets = new ObservableCollection<HardwareAsset>(SelectedSoftwareAsset.HardwareAssets);
                        OnPropertyChanged(nameof(SoftwareAssetHardwareAssets));
                    }

                    SoftwareAssetHardwareAssetsVisibility = Visibility.Visible;
                }
            }
            catch (ArgumentException ex)
            {
                SetStatusMessage(ex.Message);
            }
            catch (MySqlException)
            {
                SetStatusMessage("There was an issue connecting to the database. Please try again later.");
            }
            catch (Exception)
            {
                SetStatusMessage("An unexpected error occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Command to delete a SoftwareAsset from the database.
        /// </summary>
        [RelayCommand]
        private void DeleteSoftwareAsset()
        {
            // Only delete a SoftwareAsset if a SoftwareAsset is selected.
            if (SelectedSoftwareAsset == null)
            {
                SetStatusMessage("Please select a Software Asset");
                return;
            }

            if (!_account.IsAdmin)
            {
                return;
            }

            // Try to delete the SoftwareAsset from the database and handle any exceptions.
            try
            {
                _softwareAssetService.DeleteSoftwareAsset(SelectedSoftwareAsset.Id);

                SetStatusMessage("Software Asset Deleted");

                // Reload the SoftwareAssets.
                LoadSoftwareAssets();

                ChangeViewToView();
                ViewSoftwareAssetViewVisibility = Visibility.Collapsed;
            }
            catch (ArgumentException ex)
            {
                SetStatusMessage(ex.Message);
            }
            catch (MySqlException)
            {
                SetStatusMessage("There was an issue connecting to the database. Please try again later.");
            }
            catch (Exception)
            {
                SetStatusMessage("An unexpected error occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Command to update a SoftwareAsset in the database.
        /// </summary>
        [RelayCommand]
        private void UpdateSoftwareAsset()
        {
            // Only update a SoftwareAsset if a SoftwareAsset is selected.
            if (SelectedSoftwareAsset == null)
            {
                SetStatusMessage("Please select a Software Asset");
                return;
            }

            if (!_account.IsAdmin)
            {
                return;
            }

            try
            {
                // Update the selected SoftwareAsset in the database and notify the view.
                SelectedSoftwareAsset.Name = UpdatedSoftwareAsset.Name;
                SelectedSoftwareAsset.Version = UpdatedSoftwareAsset.Version;
                SelectedSoftwareAsset.Manufacturer = UpdatedSoftwareAsset.Manufacturer;
                _softwareAssetService.UpdateSoftwareAsset(SelectedSoftwareAsset);

                SelectedSoftwareAsset = _softwareAssetService.GetExistingSoftwareAsset(UpdatedSoftwareAsset.Name, UpdatedSoftwareAsset.Version);
                SoftwareAssetHardwareAssets = new ObservableCollection<HardwareAsset>(SelectedSoftwareAsset.HardwareAssets);
                OnPropertyChanged(nameof(SelectedSoftwareAsset));
                OnPropertyChanged(nameof(SoftwareAssetHardwareAssets));

                // Keep a reference to the selected SoftwareAsset Ids.
                int selectedSoftwareAssetId = SelectedSoftwareAsset.Id;

                // Reload the SoftwareAssets.
                LoadSoftwareAssets();

                // Set the selected SoftwareAsset to the SoftwareAsset in the SoftwareAssets collection with the selectedSoftwareAssetId.
                SelectedSoftwareAsset = SoftwareAssets.FirstOrDefault(a => a.Id == selectedSoftwareAssetId);

                PopulateSoftwareAssetDetails();
                ChangeViewToView();
                SetStatusMessage("Software asset updated successfully.");
            }
            catch (ArgumentException ex)
            {
                SetStatusMessage(ex.Message);
            }
            catch (MySqlException)
            {
                SetStatusMessage("There was an issue connecting to the database. Please try again later.");
            }
            catch (Exception)
            {
                SetStatusMessage("An unexpected error occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Command to check the vulnerabilities for the selected SoftwareAsset.
        /// </summary>
        [RelayCommand]
        private async Task CheckVulnerabilities()
        {
            if (SelectedSoftwareAsset == null)
            {
                return;
            }

            if (!_account.IsAdmin)
            {
                return;
            }

            // Try to get the vulnerabilities for the selected SoftwareAsset and handle any exceptions.
            try
            {
                // Alert the user that the vulnerabilities are being checked.
                SetStatusMessage("Checking for vulnerabilities, please wait, this may take up to 60 seconds.");
                ProgressRingIsActive = true;
                ProgressRingVisibility = Visibility.Visible;

                // Get the vulnerabilities for the selected SoftwareAsset and set the Vulnerabilities collection.
                var vulnerabilitiesList = await _softwareAssetService.GetVulnerabilitiesAsync(SelectedSoftwareAsset.Version);
                SoftwareAssetVulnerabilities = new ObservableCollection<Vulnerability>(vulnerabilitiesList);
                OnPropertyChanged(nameof(SoftwareAssetVulnerabilities));

                // Set the visibility of the SoftwareAssetVulnerabilitiesVisibility property based on the number of vulnerabilities.
                if (SoftwareAssetVulnerabilities != null && SoftwareAssetVulnerabilities.Count > 0)
                {
                    SoftwareAssetVulnerabilitiesVisibility = Visibility.Visible;
                    SetStatusMessage(string.Empty);
                    OnPropertyChanged(nameof(SoftwareAssetVulnerabilities));
                }
                else
                {
                    SoftwareAssetVulnerabilitiesVisibility = Visibility.Collapsed;
                    SetStatusMessage("No vulnerabilities found.");
                }

            }
            catch (HttpRequestException ex)
            {
                SoftwareAssetVulnerabilitiesVisibility = Visibility.Collapsed;
                SetStatusMessage(ex.Message);
            }
            catch (TaskCanceledException)
            {
                SoftwareAssetVulnerabilitiesVisibility = Visibility.Collapsed;
                SetStatusMessage("The request timed out. Please try again later.");
            }
            catch (ArgumentException ex)
            {
                SoftwareAssetVulnerabilitiesVisibility = Visibility.Collapsed;
                SetStatusMessage(ex.Message);
            }
            catch (Exception)
            {
                SoftwareAssetVulnerabilitiesVisibility = Visibility.Collapsed;
                SetStatusMessage("An unexpected error occurred. Please try again later.");
            }
            finally
            {
                ProgressRingIsActive = false;
                ProgressRingVisibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Command to change the view to the edit mode.
        /// </summary>
        [RelayCommand]
        private void ChangeViewToEdit()
        {
            // Only change the view to the edit mode if an SoftwareAsset is selected.
            if (SelectedSoftwareAsset != null)
            {
                // Set the visibility properties for the view.
                SelectsVisibility = Visibility.Collapsed;
                SoftwareAssetVulnerabilitiesVisibility = Visibility.Collapsed;
                ViewSoftwareAssetViewVisibility = Visibility.Collapsed;
                EditSoftwareAssetViewVisibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Command to change the view to the view mode.
        /// </summary>
        [RelayCommand]
        private void ChangeViewToView()
        {
            // Set the visibility properties for the view.
            EditSoftwareAssetViewVisibility = Visibility.Collapsed;
            SelectsVisibility = Visibility.Visible;
            SoftwareAssetVulnerabilitiesVisibility = Visibility.Collapsed;
            ViewSoftwareAssetViewVisibility = Visibility.Visible;

            if (_account.IsAdmin)
            {
                ButtonsVisibility = Visibility.Visible;
            }
            else
            {
                ButtonsVisibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Helper method to set the status message and make it visible.
        /// </summary>
        /// <param name="message">Message to be displayed.</param>
        private void SetStatusMessage(string message)
        {
            StatusMessage = message;
            StatusVisibility = Visibility.Visible;
        }
    }
}
