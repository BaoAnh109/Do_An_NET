using CoffeeTea.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CoffeeTea.ViewModels
{
    public class CartItemModel : BaseViewModel
    {
        public Mon MonInfo { get; set; }
        public string TenMon => MonInfo.TenMon;
        public decimal DonGia => MonInfo.DonGia;

        private int _soLuong;
        public int SoLuong
        {
            get => _soLuong;
            set { _soLuong = value; OnPropertyChanged(nameof(SoLuong)); OnPropertyChanged(nameof(ThanhTien)); }
        }

        public decimal ThanhTien => SoLuong * DonGia;
    }

    public class OrderViewModel : BaseViewModel
    {
        private readonly Action<InvoiceDetailModel> _goToPaymentAction;
        private readonly NhanVien _currentUser; 
        private QL_CoffeeTeaEntities _context = new QL_CoffeeTeaEntities();

        public ObservableCollection<DanhMucMon> Categories { get; set; }
        public ObservableCollection<Mon> MenuItems { get; set; }
        public ObservableCollection<Ban> AvailableTables { get; set; }
        public ObservableCollection<CartItemModel> CartItems { get; set; }

        private Ban _selectedTable;
        public Ban SelectedTable
        {
            get => _selectedTable;
            set { _selectedTable = value; OnPropertyChanged(nameof(SelectedTable)); UpdateCommandStates(); }
        }

        private CartItemModel _selectedCartItem;
        public CartItemModel SelectedCartItem
        {
            get => _selectedCartItem;
            set { _selectedCartItem = value; OnPropertyChanged(nameof(SelectedCartItem)); UpdateCommandStates(); }
        }

        public decimal TotalAmount => CartItems.Sum(x => x.ThanhTien);

        public ICommand FilterByCategoryCommand { get; }
        public ICommand AddToCartCommand { get; }
        public ICommand IncreaseQuantityCommand { get; }
        public ICommand DecreaseQuantityCommand { get; }
        public ICommand RemoveSelectedItemCommand { get; }
        public ICommand ClearOrderCommand { get; }
        public ICommand ProceedToPaymentCommand { get; }

        public OrderViewModel(Action<InvoiceDetailModel> goToPaymentAction, NhanVien currentUser = null)
        {
            _goToPaymentAction = goToPaymentAction;
            _currentUser = currentUser; 
            CartItems = new ObservableCollection<CartItemModel>();
            CartItems.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(TotalAmount));
                UpdateCommandStates();
            };

            LoadData();

            FilterByCategoryCommand = new RelayCommand(param => FilterMenu(param as string));
            AddToCartCommand = new RelayCommand(param => AddToCart(param as Mon));
            IncreaseQuantityCommand = new RelayCommand(param => ChangeQuantity(param as CartItemModel, 1), param => param is CartItemModel);
            DecreaseQuantityCommand = new RelayCommand(param => ChangeQuantity(param as CartItemModel, -1), param => CanDecreaseQuantity(param as CartItemModel));
            RemoveSelectedItemCommand = new RelayCommand(_ => RemoveSelectedItem(), _ => SelectedCartItem != null);
            ClearOrderCommand = new RelayCommand(_ => ClearOrder(), _ => CartItems.Any() || SelectedTable != null);
            ProceedToPaymentCommand = new RelayCommand(_ => ProceedToPayment(), _ => SelectedTable != null && CartItems.Any());
        }

        private void LoadData()
        {
            var categories = _context.DanhMucMons.ToList();
            categories.Insert(0, new DanhMucMon { MaDanhMuc = "ALL", TenDanhMuc = "Tất cả" });
            Categories = new ObservableCollection<DanhMucMon>(categories);
            MenuItems = new ObservableCollection<Mon>(_context.Mons.Where(m => m.TrangThai == "Đang bán").ToList());
            AvailableTables = new ObservableCollection<Ban>(_context.Bans.ToList());
        }

        private void FilterMenu(string categoryId)
        {
            if (categoryId == "ALL" || string.IsNullOrEmpty(categoryId))
                MenuItems = new ObservableCollection<Mon>(_context.Mons.Where(m => m.TrangThai == "Đang bán").ToList());
            else
                MenuItems = new ObservableCollection<Mon>(_context.Mons.Where(m => m.MaDanhMuc == categoryId && m.TrangThai == "Đang bán").ToList());

            OnPropertyChanged(nameof(MenuItems));
        }

        private void AddToCart(Mon item)
        {
            if (item == null) return;
            var existingItem = CartItems.FirstOrDefault(c => c.MonInfo.MaMon == item.MaMon);
            if (existingItem != null)
            {
                existingItem.SoLuong++;
            }
            else
            {
                CartItems.Add(new CartItemModel { MonInfo = item, SoLuong = 1 });
            }
            OnPropertyChanged(nameof(TotalAmount));
            UpdateCommandStates();
        }

        private void ChangeQuantity(CartItemModel item, int amount)
        {
            if (item == null) return;

            int newQuantity = item.SoLuong + amount;
            if (newQuantity < 1) return;

            item.SoLuong = newQuantity;
            OnPropertyChanged(nameof(TotalAmount));
            UpdateCommandStates();
        }

        private bool CanDecreaseQuantity(CartItemModel item)
        {
            return item != null && item.SoLuong > 1;
        }

        private void RemoveSelectedItem()
        {
            if (SelectedCartItem == null) return;

            CartItems.Remove(SelectedCartItem);
            SelectedCartItem = null;
            OnPropertyChanged(nameof(TotalAmount));
            UpdateCommandStates();
        }

        private void ClearOrder()
        {
            CartItems.Clear();
            SelectedTable = null;
            SelectedCartItem = null;
            UpdateCommandStates();
        }
        private void ProceedToPayment()
        {
            var invoiceData = new InvoiceDetailModel
            {
                TenBan = SelectedTable.TenBan,
                MaBan = SelectedTable.MaBan, 
                MaNhanVien = _currentUser != null ? _currentUser.MaNhanVien : "NV01", 
                NgayLap = DateTime.Now,
                TongTien = TotalAmount,
                Items = new ObservableCollection<CartItemModel>(CartItems)
            };

            _goToPaymentAction?.Invoke(invoiceData);
        }
        private void UpdateCommandStates()
        {
            (IncreaseQuantityCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (DecreaseQuantityCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (RemoveSelectedItemCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ClearOrderCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (ProceedToPaymentCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

    }
}
