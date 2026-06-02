using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Windows.Forms;
using CoffeeTea.Report;
using ReportInputData = CoffeeTea.ViewModels.StatisticsReportData;
using StatisticsCrystalReport = CoffeeTea.Report.StatisticsReportData;

namespace CoffeeTea.Views
{
    public partial class StoreStatisticsReportWindow : Window
    {
        private static readonly CultureInfo VietnameseCulture = CultureInfo.GetCultureInfo("vi-VN");

        private readonly ReportInputData _reportData;
        private readonly CrystalReportViewer _viewer;
        private ReportDocument _reportDocument;

        public StoreStatisticsReportWindow(ReportInputData reportData)
        {
            if (reportData == null)
            {
                throw new ArgumentNullException(nameof(reportData));
            }

            InitializeComponent();

            _reportData = reportData;
            _viewer = new CrystalReportViewer
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ToolPanelView = ToolPanelViewType.None,
                ShowLogo = false,
                ShowGroupTreeButton = false,
                ReuseParameterValuesOnRefresh = true
            };

            ReportHost.Child = _viewer;
            PeriodTextBlock.Text = string.Format(
                VietnameseCulture,
                "Từ ngày {0:dd/MM/yyyy} đến ngày {1:dd/MM/yyyy} - {2:N0} hóa đơn",
                _reportData.FromDate.Date,
                _reportData.ToDate.Date,
                _reportData.TotalInvoices);

            Loaded += StoreStatisticsReportWindow_Loaded;
            Closed += StoreStatisticsReportWindow_Closed;
        }

        private void StoreStatisticsReportWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCrystalReport();
        }

        private void LoadCrystalReport()
        {
            var oldCulture = Thread.CurrentThread.CurrentCulture;
            var oldUICulture = Thread.CurrentThread.CurrentUICulture;

            try
            {
                Thread.CurrentThread.CurrentCulture = VietnameseCulture;
                Thread.CurrentThread.CurrentUICulture = VietnameseCulture;

                _reportDocument = new StatisticsCrystalReport();

                dsThongKe dataSet = CreateReportDataSet();
                _reportDocument.SetDataSource(dataSet);
                _reportDocument.Database.Tables["HoaDon"].SetDataSource((DataTable)dataSet.HoaDon);

                SetParameterIfExists("pTongDoanhThu", _reportData.TotalRevenue);
                SetParameterIfExists("pSoHoaDon", _reportData.TotalInvoices);
                SetParameterIfExists("pTBHoaDon", _reportData.AveragePerInvoice);
                SetParameterIfExists("pTuNgay", _reportData.FromDate.Date);
                SetParameterIfExists("pDenNgay", _reportData.ToDate.Date);

                _viewer.ReportSource = _reportDocument;
                _viewer.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải báo cáo thống kê.\n\nChi tiết lỗi:\n" + ex.Message,
                    "Lỗi Crystal Report",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = oldCulture;
                Thread.CurrentThread.CurrentUICulture = oldUICulture;
            }
        }

        private dsThongKe CreateReportDataSet()
        {
            var dataSet = new dsThongKe();

            foreach (var item in _reportData.Items ?? Enumerable.Empty<CoffeeTea.ViewModels.StatisticsReportInvoiceItem>())
            {
                dataSet.HoaDon.AddHoaDonRow(
                    item.No,
                    Limit(item.InvoiceId, 10),
                    ParseCreatedAt(item.CreatedAt),
                    Limit(item.StaffName, 100),
                    Limit(item.TableName, 50),
                    Limit(item.PaymentMethod, 50),
                    item.TotalAmount,
                    Limit(item.Status, 30));
            }

            return dataSet;
        }

        private static DateTime ParseCreatedAt(string value)
        {
            DateTime result;
            string[] formats =
            {
                "dd/MM/yyyy HH:mm",
                "dd/MM/yyyy",
                "d/M/yyyy HH:mm",
                "d/M/yyyy"
            };

            if (DateTime.TryParseExact(value, formats, VietnameseCulture, DateTimeStyles.None, out result))
            {
                return result;
            }

            if (DateTime.TryParse(value, VietnameseCulture, DateTimeStyles.None, out result))
            {
                return result;
            }

            return DateTime.Today;
        }

        private static string Limit(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        private void SetParameterIfExists(string parameterName, object value)
        {
            bool exists = _reportDocument.DataDefinition.ParameterFields
                .Cast<ParameterFieldDefinition>()
                .Any(parameter => parameter.Name == parameterName);

            if (exists)
            {
                _reportDocument.SetParameterValue(parameterName, value);
            }
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewer.ReportSource != null)
            {
                _viewer.PrintReport();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void StoreStatisticsReportWindow_Closed(object sender, EventArgs e)
        {
            _viewer.ReportSource = null;
            ReportHost.Child = null;

            if (_reportDocument != null)
            {
                _reportDocument.Close();
                _reportDocument.Dispose();
                _reportDocument = null;
            }
        }
    }
}
