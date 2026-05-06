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
    public class InvoiceDetailModel
    {
        public string TenBan { get; set; }
        public DateTime NgayLap { get; set; }
        public decimal TongTien { get; set; }
        public ObservableCollection<CartItemModel> Items { get; set; }
        public string MaBan { get; set; }     
        public string MaNhanVien { get; set; }  
    }
    public class PaymentViewModel : BaseViewModel
    {
        private readonly Action _onCancelAction;
        public InvoiceDetailModel InvoiceDetails { get; set; }
        public ObservableCollection<string> PaymentMethods { get; set; }

        private string _selectedPaymentMethod;
        public string SelectedPaymentMethod
        {
            get => _selectedPaymentMethod;
            set { _selectedPaymentMethod = value; OnPropertyChanged(nameof(SelectedPaymentMethod)); }
        }

        private string _customerGivenAmount = "0";
        public string CustomerGivenAmount
        {
            get => _customerGivenAmount;
            set
            {
                _customerGivenAmount = value;
                OnPropertyChanged(nameof(CustomerGivenAmount));
                OnPropertyChanged(nameof(ChangeAmount));
            }
        }
        public decimal ChangeAmount
        {
            get
            {
                if (decimal.TryParse(CustomerGivenAmount, out decimal given))
                    return given - InvoiceDetails.TongTien > 0 ? given - InvoiceDetails.TongTien : 0;
                return 0;
            }
        }
        public ICommand CancelCommand { get; }
        public ICommand ConfirmPaymentCommand { get; }

        public PaymentViewModel(InvoiceDetailModel invoice, Action onCancelAction = null)
        {
            InvoiceDetails = invoice;
            _onCancelAction = onCancelAction;

            PaymentMethods = new ObservableCollection<string> { "Tiền mặt", "Chuyển khoản", "Thẻ tín dụng" };
            SelectedPaymentMethod = "Tiền mặt";

            CancelCommand = new RelayCommand(_ => CancelPayment());
            ConfirmPaymentCommand = new RelayCommand(_ => ConfirmPayment());
        }

        private void ConfirmPayment()
        {
            if (!decimal.TryParse(CustomerGivenAmount, out decimal givenAmount))
            {
                MessageBox.Show("Thanh toán không thành công! Vui lòng nhập số tiền hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (givenAmount < InvoiceDetails.TongTien)
            {
                MessageBox.Show("Thanh toán không thành công! Số tiền khách đưa không đủ.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                using (var context = new QL_CoffeeTeaEntities())
                {
                    int hdCount = context.HoaDons.Count() + 1;
                    string maHD = "HD" + hdCount.ToString("D4");
                    var hoaDonMoi = new HoaDon
                    {
                        MaHoaDon = maHD,
                        NgayLap = DateTime.Now,
                        MaBan = InvoiceDetails.MaBan,
                        MaNhanVien = InvoiceDetails.MaNhanVien,
                        TongTien = InvoiceDetails.TongTien,
                        PhuongThucTT = SelectedPaymentMethod,
                        TrangThai = "Đã thanh toán",
                        GhiChu = ""
                    };
                    context.HoaDons.Add(hoaDonMoi);
                    int cthdCount = context.ChiTietHoaDons.Count() + 1;
                    foreach (var item in InvoiceDetails.Items)
                    {
                        var chiTiet = new ChiTietHoaDon
                        {
                            MaCTHD = "CT" + cthdCount.ToString("D5"),
                            MaHoaDon = maHD,
                            MaMon = item.MonInfo.MaMon,
                            SoLuong = item.SoLuong,
                            DonGia = item.DonGia,
                            GhiChu = ""
                        };
                        context.ChiTietHoaDons.Add(chiTiet);
                        cthdCount++;
                    }
                    var ban = context.Bans.FirstOrDefault(b => b.MaBan == InvoiceDetails.MaBan);
                    if (ban != null)
                    {
                        ban.TrangThai = "Trống";
                    }

        
                    context.SaveChanges();
                }

                MessageBox.Show($"Thanh toán thành công {InvoiceDetails.TongTien:N0} VNĐ qua {SelectedPaymentMethod}!",
                                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

            
                _onCancelAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu CSDL: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CancelPayment()
        {
            MessageBox.Show("Đã hủy quá trình thanh toán. Quay về màn hình Lập hóa đơn.",
                            "Thông báo",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

            _onCancelAction?.Invoke();
        }
    }
}
