using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using ScottishGlenAssetTracking.Models;
using ScottishGlenAssetTracking.Services;
using ScottishGlenAssetTracking.Views.Employee;
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
    /// Partial class for the AddHardwareAssetViewModel using the ObservableObject class.
    /// </summary>
    public partial class AddHardwareAssetViewModel : ObservableObject
    {
        // Private fields for the DepartmentService, EmployeeService, and HardwareAssetService.
        private readonly DepartmentService _departmentService;
        private readonly EmployeeService _employeeService;
        private readonly HardwareAssetService _hardwareAssetService;
        private readonly SoftwareAssetService _softwareAssetService;

        // Private field for the Account.
        private readonly Account _account;

        /// <summary>
        /// Constructor for the AddHardwareAssetViewModel class using the DepartmentService, EmployeeService, and HardwareAssetService with dependency injection.
        /// </summary>
        /// <param name="departmentService">DepartmentService from dependency injection.</param>
        /// <param name="employeeService">EmployeeService from dependency injection.</param>
        /// <param name="hardwareAssetService">HardwareAssetService from dependency injection.</param>
        /// <param name="softwareAssetService">SoftwareAssetService from dependency injection.</param>
        public AddHardwareAssetViewModel(DepartmentService departmentService, 
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

            // Load departments and remove any unwanted items.
            Departments = new ObservableCollection<Department>(_departmentService.GetDepartments()
                .Where(d => d.Name != "HardwareAssets without Employee"));

            // Initialize collections.
            Employees = new ObservableCollection<Employee>();

            // Initialize new HardwareAsset with system info.
            newHardwareAsset = _hardwareAssetService.GetHardwareAssetWithSystemInfo();

            // Set purchase date to null.
            purchaseDate = null;

            // Load selections for the account type.
            LoadSelectionsForAccountType();
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

        // IsEnabled properties.
        [ObservableProperty]
        private bool departmentSelectIsEnabled = true;

        [ObservableProperty]
        private bool employeeSelectIsEnabled = true;

        // Commands

        /// <summary>
        /// Command to add an asset to the database.
        /// </summary>
        [RelayCommand]
        private void AddHardwareAsset()
        {
            // Set the PurchaseDate property of the new asset to the DateTime value of the PurchaseDate property.
            NewHardwareAsset.PurchaseDate = PurchaseDate?.DateTime;

            // Add the new hardware asset to the database.
            _hardwareAssetService.AddHardwareAsset(NewHardwareAsset);

            // Get a new SoftwareAsset with system info and set the SoftwareLinkDate property to the current date.
            NewHardwareAsset.SoftwareAsset = _softwareAssetService.GetSoftwareAssetWithSystemInfo();
            NewHardwareAsset.SoftwareLinkDate = DateTime.Now;

            // Add the new hardware asset to the SoftwareAsset's HardwareAssets collection.
            NewHardwareAsset.SoftwareAsset.HardwareAssets = new List<HardwareAsset> { NewHardwareAsset };

            // Add the new software asset to the database, if it already exists the HardwareAsset will be linked to it.
            _softwareAssetService.AddSoftwareAsset(NewHardwareAsset.SoftwareAsset);

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

                // Load employees based on the selected department and set the HardwareAssets's employee to the employee from the Employees collection.
                LoadEmployees();
                NewHardwareAsset.Employee = Employees.FirstOrDefault(e => e.Id == _account.Employee.Id);

                // Disable the selects.
                DepartmentSelectIsEnabled = false;
                EmployeeSelectIsEnabled = false;
            }
        }
    }
}
