using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using ScottishGlenAssetTracking.Models;
using ScottishGlenAssetTracking.Services;
using ScottishGlenAssetTracking.Views.HardwareAsset;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ScottishGlenAssetTracking.ViewModels
{
    /// <summary>
    /// Partial class for the ViewHardwareAssetViewModel using the ObservableObject class.
    /// </summary>
    public partial class ViewHardwareAssetViewModel : ObservableObject
    {
        // Private fields for the DepartmentService, EmployeeService, and HardwareAssetService.
        private readonly DepartmentService _departmentService;
        private readonly EmployeeService _employeeService;
        private readonly HardwareAssetService _hardwareAssetService;
        private readonly SoftwareAssetService _softwareAssetService;

        // Private field for the Account.
        private readonly Account _account;

        /// <summary>
        /// Constructor for the ViewHardwareAssetViewModel class using the DepartmentService, EmployeeService, SoftwareAssetService, and HardwareAssetService with dependency injection.
        /// </summary>
        /// <param name="departmentService">DepartmentService from dependency injection.</param>
        /// <param name="employeeService">EmployeeService from dependency injection.</param>
        /// <param name="hardwareAssetService">HardwareAssetService from dependency injection.</param>
        /// <param name="softwareAssetService">SoftwareAssetService from dependency injection.</param>
        public ViewHardwareAssetViewModel(DepartmentService departmentService,
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

            // Load departments.
            Departments = new ObservableCollection<Department>(_departmentService.GetDepartments());

            // Initialize collections.
            Employees = new ObservableCollection<Employee>();

            // Load software assets.
            SoftwareAssets = new ObservableCollection<SoftwareAsset>(_softwareAssetService.GetSoftwareAssets());

            // Load selections for the account type.
            LoadSelectionsForAccountType();
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
        /// ObservableCollection of HardwareAsset objects used in the view.
        /// </summary>
        public ObservableCollection<HardwareAsset> HardwareAssets { get; private set; }

        /// <summary>
        /// ObservableCollection of SoftwareAsset objects used in the view.
        /// </summary>
        public ObservableCollection<SoftwareAsset> SoftwareAssets { get; private set; }

        // Properties.
        [ObservableProperty]
        private Department selectedDepartment;

        [ObservableProperty]
        private Employee selectedEmployee;

        [ObservableProperty]
        private HardwareAsset selectedHardwareAsset;

        // Need to use a DateTimeOffset property as a workaround for the DatePicker control not binding to a DateTime property.
        [ObservableProperty]
        private DateTimeOffset? purchaseDate;

        [ObservableProperty]
        private string purchaseDateFormatted;

        [ObservableProperty]
        private string softwareLinkDateFormatted;

        [ObservableProperty]
        private DateTimeOffset minYear = new DateTimeOffset(new DateTime(1990, 1, 1));

        [ObservableProperty]
        private DateTimeOffset maxYear = DateTimeOffset.Now;

        [ObservableProperty]
        private string? notes;

        [ObservableProperty]
        private string statusMessage;

        // Visibility properties.
        [ObservableProperty]
        private Visibility selectsVisibility = Visibility.Visible;

        [ObservableProperty]
        private Visibility statusVisibility = Visibility.Visible;

        [ObservableProperty]
        private Visibility viewHardwareAssetViewVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility editHardwareAssetViewVisibility = Visibility.Collapsed;

        // IsEnabled properties.
        [ObservableProperty]
        private bool departmentSelectIsEnabled = true;

        [ObservableProperty]
        private bool employeeSelectIsEnabled = true;

        [ObservableProperty]
        private bool hardwareAssetDepartmentSelectIsEnabled = true;

        [ObservableProperty]
        private bool hardwareAssetEmployeeSelectIsEnabled = true;

        // Commands

        /// <summary>
        /// Command to load employees based on the selected department.
        /// </summary>
        [RelayCommand]
        private void LoadEmployees()
        {
            // Only load employees if a department is selected.
            if (SelectedDepartment == null)
            {
                StatusMessage = "Please select a Department.";
                StatusVisibility = Visibility.Visible;
                return;
            }

            // Get employees based on the selected department.
            var employees = _employeeService.GetEmployees(SelectedDepartment.Id);
            Employees = new ObservableCollection<Employee>(employees);

            // Notify the view that the Employees collection has changed.
            OnPropertyChanged(nameof(Employees));
        }

        /// <summary>
        /// Command to load HardwareAssets based on the selected employee.
        /// </summary>
        [RelayCommand]
        private void LoadHardwareAssets()
        {
            // Only load HardwareAssets if an employee is selected.
            if (SelectedEmployee == null)
            {
                StatusMessage = "Please select an Employee.";
                StatusVisibility = Visibility.Visible;
                return;
            }

            // Get assets based on the selected employee.
            var assets = _hardwareAssetService.GetHardwareAssets(SelectedEmployee.Id);
            HardwareAssets = new ObservableCollection<HardwareAsset>(assets);

            // Notify the view that the HardwareAssets collection has changed.
            OnPropertyChanged(nameof(HardwareAssets));
        }

        /// <summary>
        /// Command to populate the HardwareAsset details.
        /// </summary>
        [RelayCommand]
        private void PopulateHardwareAssetDetails()
        {
            // Only populate the asset details if an asset is selected.
            if (!IsHardwareSelected()) { return; }

            // Set the PurchaseDate property to the DateTimeOffset value of the PurchaseDate property and format it.
            if (SelectedHardwareAsset.PurchaseDate != null)
            {
                PurchaseDate = (DateTimeOffset)SelectedHardwareAsset.PurchaseDate;
                PurchaseDateFormatted = PurchaseDate?.ToString("MM/dd/yyyy");
            }
            // If the PurchaseDate is null, set the PurchaseDate property to null and the PurchaseDateFormatted property to an empty string.
            else if (SelectedHardwareAsset.PurchaseDate == null)
            {
                PurchaseDate = null;
                PurchaseDateFormatted = string.Empty;
            }

            // Set the Notes property to the Notes property of the selected HardwareAsset.
            Notes = SelectedHardwareAsset.Notes;

            // Set the selected employee, department, and software asset to the employee, department, and software asset from the Employees, Departments, and SoftwareAssets collections.
            SelectedHardwareAsset.Employee = Employees.FirstOrDefault(e => e.Id == SelectedEmployee.Id);
            SelectedHardwareAsset.Employee.Department = Departments.FirstOrDefault(d => d.Id == SelectedDepartment.Id);

            if (SelectedHardwareAsset.SoftwareAsset != null)
            {
                SelectedHardwareAsset.SoftwareAsset = SoftwareAssets.FirstOrDefault(s => s.Id == SelectedHardwareAsset.SoftwareAsset.Id);

                // Set the SoftwareLinkDateFormatted property to the DateTimeOffset value of the SoftwareLinkDate property and format it or an empty string.
                SoftwareLinkDateFormatted = SelectedHardwareAsset.SoftwareLinkDate.ToString("MM/dd/yyyy") ?? string.Empty;
            }

            // Notify the view that the SelectedHardwareAsset property has changed.
            OnPropertyChanged(nameof(SelectedHardwareAsset));

            // Clear the status message and make it hidden.
            StatusVisibility = Visibility.Collapsed;
            StatusMessage = string.Empty;

            // Change the view to the view mode.
            ChangeViewToView();
        }

        /// <summary>
        /// Command to delete a HardwareAsset from the database.
        /// </summary>
        [RelayCommand]
        private void DeleteHardwareAsset()
        {
            // Only delete a HardwareAsset if a HardwareAsset is selected.
            if (!IsHardwareSelected()) { return; }

            try
            {
                // Delete the selected HardwareAsset from the database.
                _hardwareAssetService.DeleteHardwareAsset(SelectedHardwareAsset.Id);

                // Clear the PurchaseDate field so it doesn't show in the view.
                ClearPurchaseDate();

                // Clear the Notes.
                Notes = string.Empty;

                // Clear the SoftwareLinkDateFormatted field so it doesn't show in the view.
                SoftwareLinkDateFormatted = string.Empty;

                // Reload the HardwareAssets.
                LoadHardwareAssets();

                // Change the view to the view mode.
                ChangeViewToView();

                // Hide the view for the HardwareAsset.
                ViewHardwareAssetViewVisibility = Visibility.Collapsed;

                // Reset the selections and properties.
                ResetSelectionsAndProperties();

                // Set the status message and make it visible.
                StatusVisibility = Visibility.Visible;
                StatusMessage = "Hardware Asset Deleted";
            }
            catch (ArgumentException ex)
            {
                StatusMessage = ex.Message;
                StatusVisibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Command to update a HardwareAsset in the database.
        /// </summary>
        [RelayCommand]
        private void UpdateHardwareAsset()
        {
            // Only update a HardwareAsset if a HardwareAsset is selected.
            if (!IsHardwareSelected()) { return; }

            HardwareAsset newSystemData = _hardwareAssetService.GetHardwareAssetWithSystemInfo();

            try
            {
                // Set the SelectedHardwareAsset properties to the properties of the new system data and user input.
                SelectedHardwareAsset.Name = newSystemData.Name;
                SelectedHardwareAsset.Model = newSystemData.Model;
                SelectedHardwareAsset.Manufacturer = newSystemData.Manufacturer;
                SelectedHardwareAsset.Type = newSystemData.Type;
                SelectedHardwareAsset.IpAddress = newSystemData.IpAddress;
                SelectedHardwareAsset.PurchaseDate = PurchaseDate?.DateTime;
                SelectedHardwareAsset.Notes = Notes;

                // Update the selected HardwareAsset in the database and notify the view.
                _hardwareAssetService.UpdateHardwareAsset(SelectedHardwareAsset);
                OnPropertyChanged(nameof(SelectedHardwareAsset));

                // Keep a reference to the selected employee and HardwareAsset Ids.
                int selectedEmployeeId = SelectedHardwareAsset.Employee.Id;
                int selectedHardwareAssetId = SelectedHardwareAsset.Id;

                // Reload the employees.
                LoadEmployees();

                // Set the selected to the employee in the Employees collection with the selectedEmployeeId.
                SelectedEmployee = Employees.FirstOrDefault(e => e.Id == selectedEmployeeId);

                // Reload the HardwareAssets.
                LoadHardwareAssets();

                // Set the selected HardwareAsset to the HardwareAsset in the HardwareAssets collection with the selectedHardwareAssetId.
                SelectedHardwareAsset = HardwareAssets.FirstOrDefault(a => a.Id == selectedHardwareAssetId);

                // Set the status message and make it visible.
                StatusVisibility = Visibility.Visible;
                StatusMessage = "Hardware Asset Updated";

                // Change the view to the view mode.
                ChangeViewToView();
            }
            catch (ArgumentException ex)
            {
                StatusMessage = ex.Message;
                StatusVisibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Command to change the view to the edit mode.
        /// </summary>
        [RelayCommand]
        private void ChangeViewToEdit()
        {
            // Only change the view to the edit mode if an HardwareAsset is selected.
            if (!IsHardwareSelected()) { return; }

            // Set the PurchaseDate property to the DateTimeOffset value of the PurchaseDate property.
            if (SelectedHardwareAsset.PurchaseDate != null)
            {
                PurchaseDate = (DateTimeOffset)SelectedHardwareAsset.PurchaseDate;
            }
            // If the PurchaseDate is null, set the PurchaseDate property to null.
            else if (SelectedHardwareAsset.PurchaseDate == null)
            {
                PurchaseDate = null;
            }

            // Set the visibility properties for the view.
            SelectsVisibility = Visibility.Collapsed;
            ViewHardwareAssetViewVisibility = Visibility.Collapsed;
            EditHardwareAssetViewVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Command to change the view to the view mode.
        /// </summary>
        [RelayCommand]
        private void ChangeViewToView()
        {
            // Set the visibility properties for the view.
            EditHardwareAssetViewVisibility = Visibility.Collapsed;
            SelectsVisibility = Visibility.Visible;
            ViewHardwareAssetViewVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Command to clear the PurchaseDate property.
        /// </summary>
        [RelayCommand]
        private void ClearPurchaseDate()
        {
            PurchaseDate = null;
            PurchaseDateFormatted = string.Empty;
        }

        /// <summary>
        /// Load selections for the account type and disable the selects if the account is not an admin.
        /// </summary>
        private void LoadSelectionsForAccountType()
        {
            // Only load selections for the account type if the account is not an admin.
            if (!_account.IsAdmin)
            {
                // Set the selected department to the department from the Departments collection.
                SelectedDepartment = Departments.FirstOrDefault(d => d.Id == _account.Employee.Department.Id);

                // Load employees based on the selected department and set the selected employee to the employee from the Employees collection.
                LoadEmployees();
                SelectedEmployee = Employees.FirstOrDefault(e => e.Id == _account.Employee.Id);

                // Disable the selects.
                DisableSelects();
                HardwareAssetDepartmentSelectIsEnabled = false;
                HardwareAssetEmployeeSelectIsEnabled = false;
            }
        }

        /// <summary>
        /// Helper method to reset the selections and properties.
        /// </summary>
        private void ResetSelectionsAndProperties()
        {
            // Reset the NewHardwareAsset.
            SelectedHardwareAsset = null;

            if (_account.IsAdmin)
            {
                // Reset the employees and hardware assets.
                Employees = null;
                HardwareAssets = null;

                // Notify the view that the Employees and HardwareAssets collections have changed.
                OnPropertyChanged(nameof(Employees));
                OnPropertyChanged(nameof(HardwareAssets));

                // Reset the selected department and employee.
                SelectedDepartment = null;
                SelectedEmployee = null;
            }

            // Reset the purchase date and notes.
            ClearPurchaseDate();
        }

        /// <summary>
        /// Helper method to disable the selects.
        /// </summary>
        private void DisableSelects()
        {
            // Disable the selects.
            DepartmentSelectIsEnabled = false;
            EmployeeSelectIsEnabled = false;
        }

        /// <summary>
        /// Helper method to enable the selects.
        /// </summary>
        private void EnableSelects()
        {
            // Enable the selects.
            if (_account.IsAdmin)
            {
                DepartmentSelectIsEnabled = true;
                EmployeeSelectIsEnabled = true;
            }
        }

        /// <summary>
        /// Helper method to check if a hardware asset is selected.
        /// </summary>
        /// <returns>True if a hardware asset is selected, false if not.</returns>
        private bool IsHardwareSelected()
        {
            if (SelectedHardwareAsset == null)
            {
                // Set the status message and make it visible.
                StatusMessage = "Please select a Hardware Asset.";
                StatusVisibility = Visibility.Visible;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
