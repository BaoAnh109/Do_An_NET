using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Coffee_Tee.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        public DashboardViewModel()
        {
            CultureInfo vietnameseCulture = CultureInfo.GetCultureInfo("vi-VN");
            DateTime now = DateTime.Now;

            StoreName = "Coffee & Tea";
            WelcomeMessage = "Xin chào, Quản lý quán";
            StoreTagline = "Theo dõi nhanh vận hành cửa hàng, ca làm việc và tài khoản đang sử dụng ứng dụng.";
            OpenHours = "06:30 - 22:30";
            StoreAddress = "Chi nhánh mặc định - cập nhật địa chỉ thực tế khi kết nối dữ liệu cửa hàng.";
            BusinessDate = now.ToString("dddd, dd/MM/yyyy", vietnameseCulture);
            CurrentDateText = now.ToString("dddd, dd/MM/yyyy", vietnameseCulture);
            CurrentShift = ResolveShift(now);
            ShiftText = CurrentShift + " - 07:00 đến 12:00";
            StoreStatus = IsStoreOpen(now) ? "Đang mở cửa" : "Ngoài giờ phục vụ";
            StoreStatusDetail = IsStoreOpen(now)
                ? "Các màn hình nghiệp vụ đã sẵn sàng cho ca làm việc hiện tại."
                : "Hệ thống đang nằm ngoài khung giờ phục vụ mặc định của cửa hàng.";

            CurrentAccountName = BuildDisplayName(Environment.UserName, vietnameseCulture);
            CurrentAccountInitial = string.IsNullOrWhiteSpace(CurrentAccountName)
                ? "U"
                : CurrentAccountName.Substring(0, 1).ToUpperInvariant();
            CurrentAccountRole = "Tài khoản hệ thống hiện tại";
            SignInSource = string.Format("{0}\\{1}", Environment.UserDomainName, Environment.UserName);
            DeviceName = Environment.MachineName;
            SessionStartedAt = now.ToString("HH:mm 'ngày' dd/MM/yyyy", vietnameseCulture);

            DashboardScreenCount = "12";
            FunctionalGroupCount = "4";
            ActiveSessionCount = "1";
            ServiceReadiness = "Sẵn sàng";
            TodayRevenue = "12.500.000 VNĐ";
            InvoiceCount = "86";
            ActiveTables = "14 / 25";
            BestSellerName = "Trà đào cam sả";

            FocusItems = new ObservableCollection<string>
            {
                "Theo dõi hóa đơn và bàn đang phục vụ ở nhóm Bán hàng.",
                "Rà soát tồn kho và phiếu nhập trước khi đóng ca.",
                "Kiểm tra thông tin nhân viên, nhà cung cấp và danh mục món khi cần cập nhật."
            };
            LoadRevenueChartMockData();
        }


        public Geometry RevenueLineGeometry { get; private set; }

        public Geometry RevenueAreaGeometry { get; private set; }

        public ObservableCollection<string> RevenueHourLabels { get; private set; }

        private void LoadRevenueChartMockData()
        {
            double[] values = { 2.0, 12.0, 4.0, 8.0, 6.5, 3.5 };
            string[] labels = { "07:00", "08:00", "09:00", "10:00", "11:00", "12:00" };
            const double maxValue = 12.0;
            const double chartWidth = 540.0;
            const double chartHeight = 120.0;
            double stepX = chartWidth / (values.Length - 1);

            RevenueHourLabels = new ObservableCollection<string>(labels);
            List<Point> points = new List<Point>();

            for (int i = 0; i < values.Length; i++)
            {
                double value = values[i];
                double x = i * stepX;
                double y = chartHeight - ((value / maxValue) * chartHeight);
                points.Add(new Point(x, y));
            }

            RevenueLineGeometry = CreateSmoothLineGeometry(points);
            RevenueAreaGeometry = CreateSmoothAreaGeometry(points, chartHeight, chartWidth);
        }


        public string WelcomeMessage { get; private set; }

        public string StoreName { get; private set; }

        public string StoreTagline { get; private set; }

        public string OpenHours { get; private set; }

        public string StoreAddress { get; private set; }

        public string BusinessDate { get; private set; }

        public string CurrentDateText { get; private set; }

        public string StoreStatus { get; private set; }

        public string StoreStatusDetail { get; private set; }

        public string CurrentShift { get; private set; }

        public string ShiftText { get; private set; }

        public string CurrentAccountName { get; private set; }

        public string CurrentAccountInitial { get; private set; }

        public string CurrentAccountRole { get; private set; }

        public string SignInSource { get; private set; }

        public string DeviceName { get; private set; }

        public string SessionStartedAt { get; private set; }

        public string DashboardScreenCount { get; private set; }

        public string FunctionalGroupCount { get; private set; }

        public string ActiveSessionCount { get; private set; }

        public string ServiceReadiness { get; private set; }

        public string TodayRevenue { get; private set; }

        public string InvoiceCount { get; private set; }

        public string ActiveTables { get; private set; }

        public string BestSellerName { get; private set; }

        public ObservableCollection<string> FocusItems { get; private set; }

        private static bool IsStoreOpen(DateTime now)
        {
            TimeSpan currentTime = now.TimeOfDay;
            TimeSpan openTime = new TimeSpan(6, 30, 0);
            TimeSpan closeTime = new TimeSpan(22, 30, 0);
            return currentTime >= openTime && currentTime <= closeTime;
        }

        private static string ResolveShift(DateTime now)
        {
            if (now.Hour < 12)
            {
                return "Ca sáng";
            }

            if (now.Hour < 18)
            {
                return "Ca chiều";
            }

            return "Ca tối";
        }

        private static string BuildDisplayName(string rawUserName, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(rawUserName))
            {
                return "Người dùng hệ thống";
            }

            string normalized = rawUserName
                .Replace(".", " ")
                .Replace("_", " ")
                .Replace("-", " ")
                .Trim();

            if (string.IsNullOrWhiteSpace(normalized))
            {
                return "Người dùng hệ thống";
            }

            return culture.TextInfo.ToTitleCase(normalized.ToLowerInvariant());
        }

        private static Geometry CreateSmoothLineGeometry(IList<Point> points)
        {
            StreamGeometry geometry = new StreamGeometry();

            using (StreamGeometryContext context = geometry.Open())
            {
                context.BeginFigure(points[0], false, false);

                for (int i = 1; i < points.Count; i++)
                {
                    Point previous = points[i - 1];
                    Point current = points[i];
                    double midX = (previous.X + current.X) / 2;

                    context.BezierTo(
                        new Point(midX, previous.Y),
                        new Point(midX, current.Y),
                        current,
                        true,
                        true);
                }
            }

            geometry.Freeze();
            return geometry;
        }

        private static Geometry CreateSmoothAreaGeometry(IList<Point> points, double chartHeight, double chartWidth)
        {
            StreamGeometry geometry = new StreamGeometry();

            using (StreamGeometryContext context = geometry.Open())
            {
                context.BeginFigure(new Point(0, chartHeight), true, true);
                context.LineTo(points[0], true, true);

                for (int i = 1; i < points.Count; i++)
                {
                    Point previous = points[i - 1];
                    Point current = points[i];
                    double midX = (previous.X + current.X) / 2;

                    context.BezierTo(
                        new Point(midX, previous.Y),
                        new Point(midX, current.Y),
                        current,
                        true,
                        true);
                }

                context.LineTo(new Point(chartWidth, chartHeight), true, true);
            }

            geometry.Freeze();
            return geometry;
        }
    }

}
