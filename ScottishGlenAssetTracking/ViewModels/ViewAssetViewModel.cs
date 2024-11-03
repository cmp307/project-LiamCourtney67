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
    public partial class ViewAssetViewModel : ObservableObject
    {
        private readonly DepartmentService _departmentService;
        private readonly EmployeeService _employeeService;
        private readonly AssetService _assetService;

        public ViewAssetViewModel(DepartmentService departmentService, EmployeeService employeeService, AssetService assetService)
        {
            // Initialize services
            _departmentService = departmentService;
            _employeeService = employeeService;
            _assetService = assetService;

            // Load departments
            Departments = new ObservableCollection<Department>(_departmentService.GetDepartments());

            // Initialize collections
            Employees = new ObservableCollection<Employee>();

            // Initialize commands
            SelectDepartmentCommand = new RelayCommand(LoadEmployees);
            SelectEmployeeCommand = new RelayCommand(LoadAssets);
            SelectAssetCommand = new RelayCommand(PopulateAssetDetails);

            DeleteAssetCommand = new RelayCommand(DeleteAsset);
            UpdateAssetCommand = new RelayCommand(UpdateAsset);

            ChangeViewToEditCommand = new RelayCommand(ChangeViewToEdit);
            ChangeViewToViewCommand = new RelayCommand(ChangeViewToView);
        }

        // Collections
        public ObservableCollection<Department> Departments { get; }
        public ObservableCollection<Employee> Employees { get; private set; }

        public ObservableCollection<Asset> Assets { get; private set; }

        // Properties
        [ObservableProperty]
        private Department selectedDepartment;

        [ObservableProperty]
        private Employee selectedEmployee;

        [ObservableProperty]
        private Asset selectedAsset;

        [ObservableProperty]
        private DateTimeOffset purchaseDate;

        [ObservableProperty]
        private string statusMessage;

        // Visibility properties
        [ObservableProperty]
        private Visibility statusVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility viewAssetViewVisibility = Visibility.Collapsed;

        [ObservableProperty]
        private Visibility editAssetViewVisibility = Visibility.Collapsed;

        // Commands
        public RelayCommand SelectDepartmentCommand { get; }
        public RelayCommand SelectEmployeeCommand { get; }
        public RelayCommand SelectAssetCommand { get; }
        public RelayCommand DeleteAssetCommand { get; }
        public RelayCommand UpdateAssetCommand { get; }
        public RelayCommand ChangeViewToEditCommand { get; }
        public RelayCommand ChangeViewToViewCommand { get; }

        private void LoadEmployees()
        {
            if (SelectedDepartment != null)
            {
                var employees = _employeeService.GetEmployees(SelectedDepartment.Id);
                Employees = new ObservableCollection<Employee>(employees);
                OnPropertyChanged(nameof(Employees));
            }
        }

        private void LoadAssets()
        {
            if (SelectedEmployee != null)
            {
                var assets = _assetService.GetAssets(SelectedEmployee.Id);
                Assets = new ObservableCollection<Asset>(assets);
                OnPropertyChanged(nameof(Assets));
            }
        }

        private void PopulateAssetDetails()
        {
            if (SelectedAsset != null)
            {
                PurchaseDate = (DateTimeOffset)SelectedAsset.PurchaseDate;

                SelectedAsset.Employee = Employees.FirstOrDefault(e => e.Id == SelectedAsset.Employee.Id);
                SelectedAsset.Employee.Department = Departments.FirstOrDefault(d => d.Id == SelectedAsset.Employee.Department.Id);
                OnPropertyChanged(nameof(SelectedAsset));

                StatusVisibility = Visibility.Collapsed;
                StatusMessage = string.Empty;
                ChangeViewToView();
            }
        }

        private void DeleteAsset()
        {
            if (SelectedAsset != null)
            {
                _assetService.DeleteAsset(SelectedAsset.Id);
                StatusVisibility = Visibility.Visible;
                StatusMessage = "Asset Deleted";
                LoadAssets();
                ChangeViewToView();
            }
            else
            {
                StatusMessage = "No Asset Selected";
            }
        }

        private void UpdateAsset()
        {
            if (SelectedAsset != null)
            {
                SelectedAsset.PurchaseDate = PurchaseDate.DateTime;
                _assetService.UpdateAsset(SelectedAsset);
                OnPropertyChanged(nameof(SelectedAsset));

                //int selectedDepartmentId = SelectedAsset.Employee.Department.Id;
                //int selectedEmployeeId = SelectedAsset.Employee.Id;
                //int selectedAssetId = SelectedAsset.Id;

                //SelectedDepartment = Departments.FirstOrDefault(d => d.Id == selectedDepartmentId);
                //LoadEmployees();
                //SelectedEmployee = Employees.FirstOrDefault(e => e.Id == selectedEmployeeId);
                //LoadAssets();
                //SelectedAsset = Assets.FirstOrDefault(a => a.Id == selectedAssetId);

                StatusVisibility = Visibility.Visible;
                StatusMessage = "Asset Updated";

                ChangeViewToView();
            }
        }

        private void ChangeViewToEdit()
        {
            if (SelectedAsset != null)
            {
                ViewAssetViewVisibility = Visibility.Collapsed;
                EditAssetViewVisibility = Visibility.Visible;
            }
        }

        private void ChangeViewToView()
        {
            EditAssetViewVisibility = Visibility.Collapsed;
            ViewAssetViewVisibility = Visibility.Visible;
        }
    }
}
