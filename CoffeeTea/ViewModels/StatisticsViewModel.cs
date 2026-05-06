using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CoffeeTea.Models;

namespace CoffeeTea.ViewModels
{
    public class StatisticsViewModel : BaseViewModel
    {
        private readonly Action<HoaDon> _openInvoiceAction;
        private QL_CoffeeTeaEntities _context = new QL_CoffeeTeaEntities();
        private DateTime _fromDate = DateTime.Now.Date.AddDays(-30); 
        public DateTime FromDate
        {
            get => _fromDate;
            set
            {
                DateTime newDate = value.Date;
                bool wasClamped = false;
                if (newDate > ToDate)
                {
                    newDate = ToDate;
                    wasClamped = true;
                }

                if (_fromDate == newDate)
                {
                    if (wasClamped)
                    {
                        OnPropertyChanged(nameof(FromDate));
                    }

                    return;
                }

                _fromDate = newDate;
                OnPropertyChanged(nameof(FromDate));
            }
        }

        private DateTime _toDate = DateTime.Now.Date;
        public DateTime ToDate
        {
            get => _toDate;
            set
            {
                DateTime newDate = value.Date;
                if (_toDate == newDate) return;

                _toDate = newDate;
                OnPropertyChanged(nameof(ToDate));

                if (FromDate > _toDate)
                {
                    _fromDate = _toDate;
                    OnPropertyChanged(nameof(FromDate));
                }
            }
        }

        private ObservableCollection<HoaDon> _invoices;
        public ObservableCollection<HoaDon> Invoices
        {
            get => _invoices;
            set { _invoices = value; OnPropertyChanged(nameof(Invoices)); }
        }

        public decimal TotalRevenue => Invoices?.Sum(x => x.TongTien) ?? 0;
        public int TotalInvoices => Invoices?.Count ?? 0;
        public decimal AveragePerInvoice => TotalInvoices > 0 ? TotalRevenue / TotalInvoices : 0;

        public ICommand FilterCommand { get; }
        public ICommand ExportReportCommand { get; }
        public ICommand ViewInvoiceCommand { get; }

        public StatisticsViewModel(Action<HoaDon> openInvoiceAction = null)
        {
            _openInvoiceAction = openInvoiceAction;
            FilterCommand = new RelayCommand(_ => LoadStatistics());
            ExportReportCommand = new RelayCommand(_ => ExportToExcel());
            ViewInvoiceCommand = new RelayCommand(invoice => ViewInvoice(invoice as HoaDon), invoice => invoice is HoaDon);
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            var endOfDay = ToDate.AddDays(1).AddTicks(-1);

            var result = _context.HoaDons
                .Include(h => h.Ban)
                .Include(h => h.NhanVien)
                .Where(h => h.NgayLap >= FromDate && h.NgayLap <= endOfDay && h.TrangThai == "Đã thanh toán")
                .OrderByDescending(h => h.NgayLap)
                .ToList();

            Invoices = new ObservableCollection<HoaDon>(result);
            OnPropertyChanged(nameof(TotalRevenue));
            OnPropertyChanged(nameof(TotalInvoices));
            OnPropertyChanged(nameof(AveragePerInvoice));
        }

        private void ExportToExcel()
        {

            MessageBox.Show("Chức năng xuất Report nâng cao đang được xử lý...");
        }

        private void ViewInvoice(HoaDon invoice)
        {
            if (invoice == null)
            {
                MessageBox.Show("Vui lòng chọn hóa đơn cần xem.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _openInvoiceAction?.Invoke(invoice);
        }
    }
}
