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
    public partial class AddAssetViewModel : ObservableObject
    {
        private readonly DepartmentService _departmentService;
        private readonly EmployeeService _employeeService;
        private readonly AssetService _assetService;

        public AddAssetViewModel(DepartmentService departmentService, EmployeeService employeeService, AssetService assetService)
        {
            // Initialize services
            _departmentService = departmentService;
            _employeeService = employeeService;
            _assetService = assetService;

            // Load departments and remove any unwanted items
            Departments = new ObservableCollection<Department>(_departmentService.GetDepartments()
                .Where(d => d.Name != "Assets without Employee"));

            // Initialize collections
            Employees = new ObservableCollection<Employee>();

            // Initialize new asset with system info
            newAsset = _assetService.GetAssetWithSystemInfo();

            // Set purchase date to null
            purchaseDate = null;

            // Initialize commands
            AddAssetCommand = new RelayCommand(AddAsset);
            SelectDepartmentCommand = new RelayCommand(LoadEmployees);
            ClearPurchaseDateCommand = new RelayCommand(() => PurchaseDate = null);
        }

        // Collections
        public ObservableCollection<Department> Departments { get; }
        public ObservableCollection<Employee> Employees { get; private set; }

        // Properties
        [ObservableProperty]
        private Asset newAsset;

        [ObservableProperty]
        private DateTimeOffset? purchaseDate;

        [ObservableProperty]
        private Department selectedDepartment;

        [ObservableProperty]
        private Employee selectedEmployee;

        [ObservableProperty]
        private string statusMessage;

        // Visibility properties
        [ObservableProperty]
        private Visibility statusVisibility = Visibility.Collapsed;

        // Commands
        public RelayCommand AddAssetCommand { get; }

        public RelayCommand SelectDepartmentCommand { get; }

        public RelayCommand ClearPurchaseDateCommand { get; }

        private void AddAsset()
        {
            NewAsset.PurchaseDate = PurchaseDate?.DateTime;
            _assetService.AddAsset(NewAsset);

            StatusMessage = "Asset Added";
            StatusVisibility = Visibility.Visible;
        }

        private void LoadEmployees()
        {
            if (SelectedDepartment != null)
            {
                var employees = _employeeService.GetEmployees(SelectedDepartment.Id);
                Employees = new ObservableCollection<Employee>(employees);
                OnPropertyChanged(nameof(Employees)); // Notify the view that the Employees collection has changed
            }
        }
    }
}
