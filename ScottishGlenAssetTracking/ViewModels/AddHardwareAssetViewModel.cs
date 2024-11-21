using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using ScottishGlenAssetTracking.Models;
using ScottishGlenAssetTracking.Services;
using ScottishGlenAssetTracking.Views.Employee;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.ViewModels
{
    /// <summary>
    /// Partial class for the AddHardwareAssetViewModel using the ObservableObject class.
    /// </summary>
    public partial class AddHardwareAssetViewModel : ObservableObject
    {
        // Private fields for the DepartmentService, EmployeeService, and HardwareAssetService.
        private readonly DepartmentService _departmentService;
        private readonly EmployeeService _employeeService;
        private readonly HardwareAssetService _hardwareAssetService;

        /// <summary>
        /// Constructor for the AddHardwareAssetViewModel class using the DepartmentService, EmployeeService, and HardwareAssetService with dependency injection.
        /// </summary>
        /// <param name="departmentService">DepartmentService from dependency injection.</param>
        /// <param name="employeeService">EmployeeService from dependency injection.</param>
        /// <param name="hardwareAssetService">HardwareAssetService from dependency injection.</param>
        public AddHardwareAssetViewModel(DepartmentService departmentService, EmployeeService employeeService, HardwareAssetService hardwareAssetService)
        {
            // Initialize services.
            _departmentService = departmentService;
            _employeeService = employeeService;
            _hardwareAssetService = hardwareAssetService;

            // Load departments and remove any unwanted items.
            Departments = new ObservableCollection<Department>(_departmentService.GetDepartments()
                .Where(d => d.Name != "HardwareAssets without Employee"));

            // Initialize collections.
            Employees = new ObservableCollection<Employee>();

            // Initialize new HardwareAsset with system info.
            newHardwareAsset = _hardwareAssetService.GetHardwareAssetWithSystemInfo();

            // Set purchase date to null.
            purchaseDate = null;
        }

        // Collections.

        /// <summary>
        /// ObservableCollection of Department objects used in the view.
        /// </summary>
        public ObservableCollection<Department> Departments { get; }

        /// <summary>
        /// ObservableCollection of Employee objects used in the view.
        /// </summary>
        public ObservableCollection<Employee> Employees { get; private set; }

        // Properties.
        [ObservableProperty]
        private HardwareAsset newHardwareAsset;

        // Need to use a DateTimeOffset property as a workaround for the DatePicker control not binding to a DateTime property.
        [ObservableProperty]
        private DateTimeOffset? purchaseDate;

        [ObservableProperty]
        private Department selectedDepartment;

        [ObservableProperty]
        private Employee selectedEmployee;

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
        private void AddHardwareAsset()
        {
            // Set the PurchaseDate property of the new asset to the DateTime value of the PurchaseDate property.
            NewHardwareAsset.PurchaseDate = PurchaseDate?.DateTime;

            // Add the new asset to the database.
            _hardwareAssetService.AddHardwareAsset(NewHardwareAsset);

            // Set the status message and make it visible.
            StatusMessage = "Hardware Asset Added";
            StatusVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Command to load employees based on the selected department.
        /// </summary>
        [RelayCommand]
        private void LoadEmployees()
        {
            // Only load employees if a department is selected.
            if (SelectedDepartment != null)
            {
                // Get employees based on the selected department.
                var employees = _employeeService.GetEmployees(SelectedDepartment.Id);
                Employees = new ObservableCollection<Employee>(employees);

                // Notify the view that the Employees collection has changed
                OnPropertyChanged(nameof(Employees));
            }
        }

        /// <summary>
        /// Command to clear the PurchaseDate property.
        /// </summary>
        [RelayCommand]
        private void ClearPurchaseDate() => PurchaseDate = null;
    }
}
