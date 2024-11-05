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
    /// Partial class for the AddAssetViewModel using the ObservableObject class.
    /// </summary>
    public partial class AddAssetViewModel : ObservableObject
    {
        // Private fields for the DepartmentService, EmployeeService, and AssetService.
        private readonly DepartmentService _departmentService;
        private readonly EmployeeService _employeeService;
        private readonly AssetService _assetService;

        /// <summary>
        /// Constructor for the AddAssetViewModel class using the DepartmentService, EmployeeService, and AssetService with dependency injection.
        /// </summary>
        /// <param name="departmentService">DepartmentService from dependency injection.</param>
        /// <param name="employeeService">EmployeeService from dependency injection.</param>
        /// <param name="assetService">AssetService from dependency injection.</param>
        public AddAssetViewModel(DepartmentService departmentService, EmployeeService employeeService, AssetService assetService)
        {
            // Initialize services.
            _departmentService = departmentService;
            _employeeService = employeeService;
            _assetService = assetService;

            // Load departments and remove any unwanted items.
            Departments = new ObservableCollection<Department>(_departmentService.GetDepartments()
                .Where(d => d.Name != "Assets without Employee"));

            // Initialize collections.
            Employees = new ObservableCollection<Employee>();

            // Initialize new asset with system info.
            newAsset = _assetService.GetAssetWithSystemInfo();

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
        private Asset newAsset;

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
        private void AddAsset()
        {
            // Set the PurchaseDate property of the new asset to the DateTime value of the PurchaseDate property.
            NewAsset.PurchaseDate = PurchaseDate?.DateTime;

            // Add the new asset to the database.
            _assetService.AddAsset(NewAsset);

            // Set the status message and make it visible.
            StatusMessage = "Asset Added";
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
