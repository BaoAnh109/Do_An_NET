using System;
using System.Windows.Input;
using CoffeeTea.Models;
using CoffeeTea.Views;

namespace CoffeeTea.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly NhanVien _authenticatedUser;
        private readonly Action _logoutAction;

        private object _currentView;
        private ICommand _dashboardCommand;
        private ICommand _drinkCommand;
        private ICommand _categoryCommand;
        private ICommand _staffCommand;
        private ICommand _supplierCommand;
        private ICommand _orderCommand;
        private ICommand _paymentCommand;
        private ICommand _tableStatusCommand;
        private ICommand _importCommand;
        private ICommand _inventoryCommand;
        private ICommand _revenueReportCommand;
        private ICommand _profileCommand;
        private ICommand _settingsCommand;
        private ICommand _logoutCommand;

        public MainViewModel(NhanVien authenticatedUser, Action logoutAction)
        {
            _authenticatedUser = authenticatedUser;
            _logoutAction = logoutAction;

            CurrentAccountName = ResolveAccountName();
            CurrentAccountRole = ResolveRoleDisplay();
            BuildPermissionSet();

            _dashboardCommand = new RelayCommand(_ => CurrentView = new UCDashboardView(_authenticatedUser), _ => CanAccessSalesMenu);
            _drinkCommand = new RelayCommand(_ => CurrentView = new UCDrinkManagement(), _ => CanAccessCatalogMenu);
            _categoryCommand = new RelayCommand(_ => CurrentView = new UCCategoryManagement(), _ => CanAccessCatalogMenu);
            _staffCommand = new RelayCommand(_ => CurrentView = new UCStaffManagement(), _ => CanAccessCatalogMenu);
            _supplierCommand = new RelayCommand(_ => CurrentView = new UCSupplierManagement(), _ => CanAccessCatalogMenu);
            _orderCommand = new RelayCommand(_ => CurrentView = new UCOrder(), _ => CanAccessSalesMenu);
            _paymentCommand = new RelayCommand(_ => CurrentView = new UCPayment(), _ => CanAccessSalesMenu);
            _tableStatusCommand = new RelayCommand(_ => CurrentView = new UCTableStatus(), _ => CanAccessSalesMenu);
            _importCommand = new RelayCommand(_ => CurrentView = new UCImportReceipt(), _ => CanAccessWarehouseMenu);
            _inventoryCommand = new RelayCommand(_ => CurrentView = new UCInventory(), _ => CanAccessWarehouseMenu);
            _revenueReportCommand = new RelayCommand(_ => CurrentView = new UCStoreStatistics(), _ => CanAccessStatisticsMenu);
            _profileCommand = new RelayCommand(_ => CurrentView = new UCProfile(_authenticatedUser), _ => CanAccessSystemMenu);
            _settingsCommand = new RelayCommand(_ => CurrentView = new UCSettings(_authenticatedUser), _ => CanAccessSettings);
            _logoutCommand = new RelayCommand(_ => _logoutAction?.Invoke());

            CurrentView = new UCDashboardView(_authenticatedUser);
        }

        public string CurrentAccountName { get; private set; }

        public string CurrentAccountRole { get; private set; }

        public bool CanAccessCatalogMenu { get; private set; }

        public bool CanAccessSalesMenu { get; private set; }

        public bool CanAccessWarehouseMenu { get; private set; }

        public bool CanAccessStatisticsMenu { get; private set; }

        public bool CanAccessSystemMenu { get; private set; }

        public bool CanAccessSettings { get; private set; }

        public object CurrentView
        {
            get
            {
                return _currentView;
            }
            set
            {
                if (_currentView == value)
                {
                    return;
                }

                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public NhanVien AuthenticatedUser
        {
            get { return _authenticatedUser; }
        }

        public ICommand DashboardCommand
        {
            get { return _dashboardCommand; }
        }

        public ICommand DrinkCommand
        {
            get { return _drinkCommand; }
        }

        public ICommand CategoryCommand
        {
            get { return _categoryCommand; }
        }

        public ICommand StaffCommand
        {
            get { return _staffCommand; }
        }

        public ICommand SupplierCommand
        {
            get { return _supplierCommand; }
        }

        public ICommand OrderCommand
        {
            get { return _orderCommand; }
        }

        public ICommand PaymentCommand
        {
            get { return _paymentCommand; }
        }

        public ICommand TableStatusCommand
        {
            get { return _tableStatusCommand; }
        }

        public ICommand ImportCommand
        {
            get { return _importCommand; }
        }

        public ICommand InventoryCommand
        {
            get { return _inventoryCommand; }
        }

        public ICommand RevenueReportCommand
        {
            get { return _revenueReportCommand; }
        }

        public ICommand ProfileCommand
        {
            get { return _profileCommand; }
        }

        public ICommand SettingsCommand
        {
            get { return _settingsCommand; }
        }

        public ICommand LogoutCommand
        {
            get { return _logoutCommand; }
        }

        private string ResolveAccountName()
        {
            if (_authenticatedUser == null || string.IsNullOrWhiteSpace(_authenticatedUser.HoTen))
            {
                return "Tài khoản không xác định";
            }

            return _authenticatedUser.HoTen.Trim();
        }

        private string ResolveRoleDisplay()
        {
            string roleName = _authenticatedUser?.VaiTro != null ? _authenticatedUser.VaiTro.TenVaiTro : null;
            string roleCode = _authenticatedUser != null ? _authenticatedUser.MaVaiTro : null;

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                return roleName.Trim();
            }

            if (string.Equals(roleCode, "VT01", StringComparison.OrdinalIgnoreCase))
            {
                return "Admin";
            }

            if (string.Equals(roleCode, "VT02", StringComparison.OrdinalIgnoreCase))
            {
                return "Quản lý";
            }

            if (string.Equals(roleCode, "VT03", StringComparison.OrdinalIgnoreCase))
            {
                return "Nhân viên";
            }

            return "Không xác định";
        }

        private void BuildPermissionSet()
        {
            bool isAdmin = string.Equals(_authenticatedUser?.MaVaiTro, "VT01", StringComparison.OrdinalIgnoreCase);
            bool isManager = string.Equals(_authenticatedUser?.MaVaiTro, "VT02", StringComparison.OrdinalIgnoreCase);
            bool isStaff = string.Equals(_authenticatedUser?.MaVaiTro, "VT03", StringComparison.OrdinalIgnoreCase);

            CanAccessCatalogMenu = isAdmin || isManager;
            CanAccessSalesMenu = true;
            CanAccessWarehouseMenu = isAdmin || isManager || isStaff;
            CanAccessStatisticsMenu = isAdmin || isManager;
            CanAccessSystemMenu = true;
            CanAccessSettings = isAdmin || isManager;
        }
    }
}
