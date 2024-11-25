using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    /// Partial class for the AddSoftwareAssetViewModel using the ObservableObject class.
    /// </summary>
    public partial class AddSoftwareAssetViewModel : ObservableObject
    {
        // Private fields for the DepartmentService, EmployeeService, HardwareAssetService, and SoftwareAssetService.
        private readonly DepartmentService _departmentService;
        private readonly EmployeeService _employeeService;
        private readonly HardwareAssetService _hardwareAssetService;
        private readonly SoftwareAssetService _softwareAssetService;

        /// <summary>
        /// Constructor for the AddSoftwareAssetViewModel class using the DepartmentService, EmployeeService, HardwareAssetService and SoftwareAssetService with dependency injection.
        /// </summary>
        /// <param name="departmentService">DepartmentService from dependency injection.</param>
        /// <param name="employeeService">EmployeeService from dependency injection.</param>
        /// <param name="hardwareAssetService">HardwareAssetService from dependency injection.</param>
        /// <param name="softwareAssetService">SoftwareAssetService from dependency injection.</param>
        public AddSoftwareAssetViewModel(DepartmentService departmentService,
                                         EmployeeService employeeService,
                                         HardwareAssetService hardwareAssetService,
                                         SoftwareAssetService softwareAssetService)
        {
            // Initialize services.
            _departmentService = departmentService;
            _employeeService = employeeService;
            _hardwareAssetService = hardwareAssetService;
            _softwareAssetService = softwareAssetService;

            // Load HardwareAssets.
            HardwareAssets = new ObservableCollection<HardwareAsset>(_hardwareAssetService.GetAllHardwareAssets());

            // Initialize new SoftwareAsset with system info.
            newSoftwareAsset = _softwareAssetService.GetSoftwareAssetWithSystemInfo();
            newSoftwareAsset.HardwareAssets = new List<HardwareAsset>();
        }

        // Collections.

        /// <summary>
        /// ObservableCollection of HardwareAssets objects used in the view.
        /// </summary>
        public ObservableCollection<HardwareAsset> HardwareAssets { get; }


        // Properties.
        [ObservableProperty]
        private SoftwareAsset newSoftwareAsset;

        [ObservableProperty]
        private HardwareAsset selectedHardwareAsset;

        [ObservableProperty]
        private string statusMessage;

        // Visibility properties.
        [ObservableProperty]
        private Visibility statusVisibility = Visibility.Collapsed;

        // Commands

        /// <summary>
        /// Command to add an asset to the database.
        /// </summary>
        [RelayCommand]
        private void AddSoftwareAsset()
        {
            NewSoftwareAsset.Version = "TEST";
            
            if (SelectedHardwareAsset != null)
            {
                NewSoftwareAsset.HardwareAssets.Add(SelectedHardwareAsset);
            }

            // Add the new software asset to the database.
            _softwareAssetService.AddSoftwareAsset(NewSoftwareAsset);

            // Set the status message and make it visible.
            StatusMessage = "Software Asset Added";
            StatusVisibility = Visibility.Visible;
        }
    }
}
