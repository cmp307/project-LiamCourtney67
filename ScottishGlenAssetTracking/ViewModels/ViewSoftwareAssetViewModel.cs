using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using ScottishGlenAssetTracking.Models;
using ScottishGlenAssetTracking.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

            // Initialize the UpdatedSoftwareAsset property.
            UpdatedSoftwareAsset = new SoftwareAsset();
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
        private Visibility viewSoftwareAssetViewVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility editSoftwareAssetViewVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility softwareAssetHardwareAssetsVisibility = Visibility.Collapsed;

        // Commands

        /// <summary>
        /// Helper method to load the SoftwareAssets.
        /// </summary>
        private void LoadSoftwareAssets() {
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
            if (SelectedSoftwareAsset != null)
            {
                // Create a new SoftwareAsset with the selected SoftwareAsset properties.
                UpdatedSoftwareAsset = new SoftwareAsset
                {
                    Name = SelectedSoftwareAsset.Name,
                    Version = SelectedSoftwareAsset.Version,
                    Manufacturer = SelectedSoftwareAsset.Manufacturer,
                    HardwareAssets = new List<HardwareAsset>(SelectedSoftwareAsset.HardwareAssets)
                };

                // Notify the view that the SelectedSoftwareAsset property has changed.
                OnPropertyChanged(nameof(SelectedSoftwareAsset));

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
                    // Set the SoftwareAssetHardwareAssets collection to the HardwareAssets of the selected SoftwareAsset or only the HardwareAssets linked to the Employee.
                    if (_account.IsAdmin)
                    {
                        SoftwareAssetHardwareAssets = new ObservableCollection<HardwareAsset>(SelectedSoftwareAsset.HardwareAssets);
                        OnPropertyChanged(nameof(SoftwareAssetHardwareAssets));
                    }
                    else if (_account.Employee != null)
                    {
                        SoftwareAssetHardwareAssets = new ObservableCollection<HardwareAsset>(SelectedSoftwareAsset.HardwareAssets
                            .Where(h => h.Employee != null && h.Employee.Id == _account.Employee.Id));
                        OnPropertyChanged(nameof(SoftwareAssetHardwareAssets));
                    }

                    SoftwareAssetHardwareAssetsVisibility = Visibility.Visible;
                }
            }
        }

        /// <summary>
        /// Command to delete a SoftwareAsset from the database.
        /// </summary>
        [RelayCommand]
        private void DeleteSoftwareAsset()
        {
            // Only delete a SoftwareAsset if a SoftwareAsset is selected.
            if (SelectedSoftwareAsset != null)
            {
                // Delete the selected SoftwareAsset from the database.
                _softwareAssetService.DeleteSoftwareAsset(SelectedSoftwareAsset.Id);

                // Set the status message and make it visible.
                StatusVisibility = Visibility.Visible;
                StatusMessage = "Software Asset Deleted";

                // Reload the SoftwareAssets.
                LoadSoftwareAssets();

                // Change the view to the view mode.
                ChangeViewToView();
            }
            else
            {
                // Set the status message and make it visible.
                StatusMessage = "No Software Asset Selected";
                StatusVisibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Command to update a SoftwareAsset in the database.
        /// </summary>
        [RelayCommand]
        private void UpdateSoftwareAsset()
        {
            // Only update a SoftwareAsset if a SoftwareAsset is selected.
            if (SelectedSoftwareAsset != null)
            {
                Employee employee = _employeeService.GetEmployee(28);

                // Update the selected SoftwareAsset in the database and notify the view.
                _softwareAssetService.UpdateSoftwareAsset(UpdatedSoftwareAsset, employee);
                // TODO for admin copy properties to selected asset

                SelectedSoftwareAsset = _softwareAssetService.GetExistingSoftwareAsset(UpdatedSoftwareAsset.Name, UpdatedSoftwareAsset.Version);
                OnPropertyChanged(nameof(SelectedSoftwareAsset));

                // Keep a reference to the selected SoftwareAsset Ids.
                int selectedSoftwareAssetId = SelectedSoftwareAsset.Id;

                // Reload the SoftwareAssets.
                LoadSoftwareAssets();

                // Set the selected SoftwareAsset to the SoftwareAsset in the SoftwareAssets collection with the selectedSoftwareAssetId.
                SelectedSoftwareAsset = SoftwareAssets.FirstOrDefault(a => a.Id == selectedSoftwareAssetId);

                // Set the status message and make it visible.
                StatusVisibility = Visibility.Visible;
                StatusMessage = "Software Asset Updated";

                // Change the view to the view mode.
                ChangeViewToView();
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
            ViewSoftwareAssetViewVisibility = Visibility.Visible;
        }
    }
}
