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
    /// Partial class for the ViewAssetViewModel using the ObservableObject class.
    /// </summary>
    public partial class ViewAssetViewModel : ObservableObject
    {
        // Private fields for the DepartmentService, EmployeeService, and AssetService.
        private readonly DepartmentService _departmentService;
        private readonly EmployeeService _employeeService;
        private readonly AssetService _assetService;

        /// <summary>
        /// Constructor for the ViewAssetViewModel class using the DepartmentService, EmployeeService, and AssetService with dependency injection.
        /// </summary>
        /// <param name="departmentService">DepartmentService from dependency injection.</param>
        /// <param name="employeeService">EmployeeService from dependency injection.</param>
        /// <param name="assetService">AssetService from dependency injection.</param>
        public ViewAssetViewModel(DepartmentService departmentService, EmployeeService employeeService, AssetService assetService)
        {
            // Initialize services.
            _departmentService = departmentService;
            _employeeService = employeeService;
            _assetService = assetService;

            // Load departments.
            Departments = new ObservableCollection<Department>(_departmentService.GetDepartments());

            // Initialize collections.
            Employees = new ObservableCollection<Employee>();
        }

        // Collections

        /// <summary>
        /// ObservableCollection of Department objects used in the view.
        /// </summary>
        public ObservableCollection<Department> Departments { get; }

        /// <summary>
        /// ObservableCollection of Employee objects used in the view.
        /// </summary>
        public ObservableCollection<Employee> Employees { get; private set; }

        /// <summary>
        /// ObservableCollection of Asset objects used in the view.
        /// </summary>
        public ObservableCollection<Asset> Assets { get; private set; }

        // Properties.
        [ObservableProperty]
        private Department selectedDepartment;

        [ObservableProperty]
        private Employee selectedEmployee;

        [ObservableProperty]
        private Asset selectedAsset;

        // Need to use a DateTimeOffset property as a workaround for the DatePicker control not binding to a DateTime property.
        [ObservableProperty]
        private DateTimeOffset? purchaseDate;

        [ObservableProperty]
        private string purchaseDateFormatted;

        [ObservableProperty]
        private string statusMessage;

        // Visibility properties.
        [ObservableProperty]
        private Visibility selectsVisibility = Visibility.Visible;

        [ObservableProperty]
        private Visibility statusVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility viewAssetViewVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility editAssetViewVisibility = Visibility.Collapsed;

        // Commands

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

                // Notify the view that the Employees collection has changed.
                OnPropertyChanged(nameof(Employees));
            }
        }

        /// <summary>
        /// Command to load assets based on the selected employee.
        /// </summary>
        [RelayCommand]
        private void LoadAssets()
        {
            // Only load assets if an employee is selected.
            if (SelectedEmployee != null)
            {
                // Get assets based on the selected employee.
                var assets = _assetService.GetAssets(SelectedEmployee.Id);
                Assets = new ObservableCollection<Asset>(assets);

                // Notify the view that the Assets collection has changed.
                OnPropertyChanged(nameof(Assets));
            }
        }

        /// <summary>
        /// Command to populate the asset details.
        /// </summary>
        [RelayCommand]
        private void PopulateAssetDetails()
        {
            // Only populate the asset details if an asset is selected.
            if (SelectedAsset != null)
            {
                // Set the PurchaseDate property to the DateTimeOffset value of the PurchaseDate property and format it.
                if (SelectedAsset.PurchaseDate != null)
                {
                    PurchaseDate = (DateTimeOffset)SelectedAsset.PurchaseDate;
                    PurchaseDateFormatted = PurchaseDate?.ToString("MM/dd/yyyy");
                }
                // If the PurchaseDate is null, set the PurchaseDate property to null and the PurchaseDateFormatted property to an empty string.
                else if (SelectedAsset.PurchaseDate == null)
                {
                    PurchaseDate = null;
                    PurchaseDateFormatted = string.Empty;
                }

                // Set the selected employee and department to the employee and department from the Employees and Departments collections.
                SelectedAsset.Employee = Employees.FirstOrDefault(e => e.Id == SelectedEmployee.Id);
                SelectedAsset.Employee.Department = Departments.FirstOrDefault(d => d.Id == SelectedDepartment.Id);

                // Notify the view that the SelectedAsset property has changed.
                OnPropertyChanged(nameof(SelectedAsset));

                // Clear the status message and make it hidden.
                StatusVisibility = Visibility.Collapsed;
                StatusMessage = string.Empty;

                // Change the view to the view mode.
                ChangeViewToView();
            }
        }

        /// <summary>
        /// Command to delete an asset from the database.
        /// </summary>
        [RelayCommand]
        private void DeleteAsset()
        {
            // Only delete an asset if an asset is selected.
            if (SelectedAsset != null)
            {
                // Delete the selected asset from the database.
                _assetService.DeleteAsset(SelectedAsset.Id);

                // Set the status message and make it visible.
                StatusVisibility = Visibility.Visible;
                StatusMessage = "Asset Deleted";

                // Reload the assets.
                LoadAssets();

                // Change the view to the view mode.
                ChangeViewToView();
            }
            else
            {
                // Set the status message and make it visible.
                StatusMessage = "No Asset Selected";
                StatusVisibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Command to update an asset in the database.
        /// </summary>
        [RelayCommand]
        private void UpdateAsset()
        {
            // Only update an asset if an asset is selected.
            if (SelectedAsset != null)
            {
                // Set the PurchaseDate property of the selected asset to the DateTime value of the PurchaseDate property.
                SelectedAsset.PurchaseDate = PurchaseDate?.DateTime;

                // Update the selected asset in the database and notify the view.
                _assetService.UpdateAsset(SelectedAsset);
                OnPropertyChanged(nameof(SelectedAsset));

                // Keep a reference to the selected employee and asset Ids.
                int selectedEmployeeId = SelectedAsset.Employee.Id;
                int selectedAssetId = SelectedAsset.Id;

                // Reload the employees.
                LoadEmployees();

                // Set the selected to the employee in the Employees collection with the selectedEmployeeId.
                SelectedEmployee = Employees.FirstOrDefault(e => e.Id == selectedEmployeeId);

                // Reload the assets.
                LoadAssets();

                // Set the selected asset to the asset in the Assets collection with the selectedAssetId.
                SelectedAsset = Assets.FirstOrDefault(a => a.Id == selectedAssetId);

                // Set the status message and make it visible.
                StatusVisibility = Visibility.Visible;
                StatusMessage = "Asset Updated";

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
            // Only change the view to the edit mode if an asset is selected.
            if (SelectedAsset != null)
            {
                // Set the PurchaseDate property to the DateTimeOffset value of the PurchaseDate property.
                if (SelectedAsset.PurchaseDate != null)
                {
                    PurchaseDate = (DateTimeOffset)SelectedAsset.PurchaseDate;
                }
                // If the PurchaseDate is null, set the PurchaseDate property to null.
                else if (SelectedAsset.PurchaseDate == null)
                {
                    PurchaseDate = null;
                }

                // Set the visibility properties for the view.
                SelectsVisibility = Visibility.Collapsed;
                ViewAssetViewVisibility = Visibility.Collapsed;
                EditAssetViewVisibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Command to change the view to the view mode.
        /// </summary>
        [RelayCommand]
        private void ChangeViewToView()
        {
            // Set the visibility properties for the view.
            EditAssetViewVisibility = Visibility.Collapsed;
            SelectsVisibility = Visibility.Visible;
            ViewAssetViewVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Command to clear the PurchaseDate property.
        /// </summary>
        [RelayCommand]
        private void ClearPurchaseDate() => PurchaseDate = null;
    }
}
