using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using CoffeeTea.ViewModels;

namespace CoffeeTea.Views
{
    public partial class StoreStatisticsReportWindow : Window
    {
        private readonly StatisticsReportData _reportData;
        private readonly CultureInfo _culture = CultureInfo.GetCultureInfo("vi-VN");

        public StoreStatisticsReportWindow(StatisticsReportData reportData)
        {
            InitializeComponent();
            _reportData = reportData ?? throw new ArgumentNullException(nameof(reportData));
            PeriodTextBlock.Text = $"Từ ngày {_reportData.FromDate:dd/MM/yyyy} đến ngày {_reportData.ToDate:dd/MM/yyyy}";
            ReportViewer.Document = CreateReportDocument();
        }

        private FlowDocument CreateReportDocument()
        {
            var document = new FlowDocument
            {
                PagePadding = new Thickness(48),
                ColumnWidth = 700,
                FontFamily = new FontFamily("Segoe UI"),
                FontSize = 12,
                Foreground = Brushes.Black
            };

            document.Blocks.Add(CreateTitle("BÁO CÁO THỐNG KÊ CỬA HÀNG"));
            document.Blocks.Add(CreateInfoParagraph($"Thời gian: {_reportData.FromDate:dd/MM/yyyy} - {_reportData.ToDate:dd/MM/yyyy}"));
            document.Blocks.Add(CreateInfoParagraph($"Ngày lập report: {_reportData.GeneratedAt:dd/MM/yyyy HH:mm}"));
            document.Blocks.Add(CreateSummaryTable());
            document.Blocks.Add(CreateSectionTitle("Danh sách hóa đơn đã thanh toán"));
            document.Blocks.Add(CreateInvoiceTable());

            return document;
        }

        private Paragraph CreateTitle(string text)
        {
            return new Paragraph(new Run(text))
            {
                FontSize = 22,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 18)
            };
        }

        private Paragraph CreateInfoParagraph(string text)
        {
            return new Paragraph(new Run(text))
            {
                FontSize = 12,
                TextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 0, 0, 4)
            };
        }

        private Paragraph CreateSectionTitle(string text)
        {
            return new Paragraph(new Run(text))
            {
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 18, 0, 8)
            };
        }

        private Table CreateSummaryTable()
        {
            var table = new Table
            {
                CellSpacing = 0,
                Margin = new Thickness(0, 18, 0, 0)
            };

            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            table.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });

            var group = new TableRowGroup();
            var header = new TableRow();
            header.Cells.Add(CreateCell("Tổng doanh thu", true, TextAlignment.Center, true));
            header.Cells.Add(CreateCell("Số hóa đơn", true, TextAlignment.Center, true));
            header.Cells.Add(CreateCell("TB / hóa đơn", true, TextAlignment.Center, true));

            var values = new TableRow();
            values.Cells.Add(CreateCell(FormatMoney(_reportData.TotalRevenue), true, TextAlignment.Center));
            values.Cells.Add(CreateCell(_reportData.TotalInvoices.ToString("N0", _culture), true, TextAlignment.Center));
            values.Cells.Add(CreateCell(FormatMoney(_reportData.AveragePerInvoice), true, TextAlignment.Center));

            group.Rows.Add(header);
            group.Rows.Add(values);
            table.RowGroups.Add(group);
            return table;
        }

        private Table CreateInvoiceTable()
        {
            var table = new Table
            {
                CellSpacing = 0
            };

            table.Columns.Add(new TableColumn { Width = new GridLength(38) });
            table.Columns.Add(new TableColumn { Width = new GridLength(72) });
            table.Columns.Add(new TableColumn { Width = new GridLength(104) });
            table.Columns.Add(new TableColumn { Width = new GridLength(70) });
            table.Columns.Add(new TableColumn { Width = new GridLength(118) });
            table.Columns.Add(new TableColumn { Width = new GridLength(95) });
            table.Columns.Add(new TableColumn { Width = new GridLength(96) });

            var group = new TableRowGroup();
            var header = new TableRow();
            header.Cells.Add(CreateCell("STT", true, TextAlignment.Center, true));
            header.Cells.Add(CreateCell("Mã HĐ", true, TextAlignment.Center, true));
            header.Cells.Add(CreateCell("Ngày lập", true, TextAlignment.Center, true));
            header.Cells.Add(CreateCell("Bàn", true, TextAlignment.Center, true));
            header.Cells.Add(CreateCell("Nhân viên", true, TextAlignment.Center, true));
            header.Cells.Add(CreateCell("PTTT", true, TextAlignment.Center, true));
            header.Cells.Add(CreateCell("Tổng tiền", true, TextAlignment.Right, true));
            group.Rows.Add(header);

            foreach (var item in _reportData.Items)
            {
                var row = new TableRow();
                row.Cells.Add(CreateCell(item.No.ToString(), false, TextAlignment.Center));
                row.Cells.Add(CreateCell(item.InvoiceId, false, TextAlignment.Left));
                row.Cells.Add(CreateCell(item.CreatedAt, false, TextAlignment.Left));
                row.Cells.Add(CreateCell(item.TableName, false, TextAlignment.Left));
                row.Cells.Add(CreateCell(item.StaffName, false, TextAlignment.Left));
                row.Cells.Add(CreateCell(item.PaymentMethod, false, TextAlignment.Left));
                row.Cells.Add(CreateCell(FormatMoney(item.TotalAmount), false, TextAlignment.Right));
                group.Rows.Add(row);
            }

            var totalRow = new TableRow();
            totalRow.Cells.Add(CreateCell("Tổng cộng", true, TextAlignment.Right));
            totalRow.Cells[0].ColumnSpan = 6;
            totalRow.Cells.Add(CreateCell(FormatMoney(_reportData.TotalRevenue), true, TextAlignment.Right));
            group.Rows.Add(totalRow);

            table.RowGroups.Add(group);
            return table;
        }

        private TableCell CreateCell(string text, bool isBold, TextAlignment alignment, bool isHeader = false)
        {
            var paragraph = new Paragraph(new Run(text ?? ""))
            {
                Margin = new Thickness(0),
                TextAlignment = alignment,
                FontSize = isHeader ? 11 : 10.5
            };

            if (isBold)
            {
                paragraph.FontWeight = FontWeights.Bold;
            }

            return new TableCell(paragraph)
            {
                Padding = new Thickness(6, 5, 6, 5),
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(0.5),
                Background = isHeader ? new SolidColorBrush(Color.FromRgb(234, 248, 253)) : Brushes.White
            };
        }

        private string FormatMoney(decimal value)
        {
            return value.ToString("N0", _culture) + " đ";
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                var paginator = ((IDocumentPaginatorSource)ReportViewer.Document).DocumentPaginator;
                printDialog.PrintDocument(paginator, "Báo cáo thống kê cửa hàng");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
