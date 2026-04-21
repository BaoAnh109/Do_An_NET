using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Coffee_Tee.Views;

namespace Coffee_Tee.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
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
        private ICommand _changePasswordCommand;
        private ICommand _logoutCommand;

        public MainViewModel()
        {
            _dashboardCommand = new RelayCommand(delegate { CurrentView = new UCDashboardView(); });
            _drinkCommand = new RelayCommand(delegate { CurrentView = new UCDrinkManagement(); });
            _categoryCommand = new RelayCommand(delegate { CurrentView = new UCCategoryManagement(); });
            _staffCommand = new RelayCommand(delegate { CurrentView = new UCStaffManagement(); });
            _supplierCommand = new RelayCommand(delegate { CurrentView = new UCSupplierManagement(); });
            _orderCommand = new RelayCommand(delegate { CurrentView = new UCOrder(); });
            _paymentCommand = new RelayCommand(delegate { CurrentView = new UCPayment(); });
            _tableStatusCommand = new RelayCommand(delegate { CurrentView = new UCTableStatus(); });
            _importCommand = new RelayCommand(delegate { CurrentView = new UCImportReceipt(); });
            _inventoryCommand = new RelayCommand(delegate { CurrentView = new UCInventory(); });
            _revenueReportCommand = new RelayCommand(delegate { CurrentView = new UCStoreStatistics(); });
            _profileCommand = new RelayCommand(delegate { CurrentView = new UCProfile(); });
            _logoutCommand = new RelayCommand(delegate { CurrentView = null; });

            CurrentView = new UCDashboardView();
        }

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

        public ICommand ChangePasswordCommand
        {
            get { return _changePasswordCommand; }
        }

        public ICommand LogoutCommand
        {
            get { return _logoutCommand; }
        }
    }
}
