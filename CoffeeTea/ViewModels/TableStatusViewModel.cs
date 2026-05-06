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
    public class TableStatusViewModel : BaseViewModel
    {
        private QL_CoffeeTeaEntities _context = new QL_CoffeeTeaEntities();
        public ObservableCollection<string> Areas { get; set; }

        private ObservableCollection<Ban> _tableList;
        public ObservableCollection<Ban> TableList
        {
            get => _tableList;
            set { _tableList = value; OnPropertyChanged(nameof(TableList)); }
        }

        private string _selectedArea;
        public string SelectedArea
        {
            get => _selectedArea;
            set { _selectedArea = value; OnPropertyChanged(nameof(SelectedArea)); FilterTables(); }
        }

        public ICommand TableClickCommand { get; }

        public TableStatusViewModel()
        {
            LoadData();
            TableClickCommand = new RelayCommand(param => HandleTableClick(param as Ban));
        }

        private void LoadData()
        {
            var areas = _context.Bans.Select(b => b.KhuVuc).Distinct().ToList();
            areas.Insert(0, "Tất cả khu vực");
            Areas = new ObservableCollection<string>(areas);

            SelectedArea = "Tất cả khu vực";
        }

        private void FilterTables()
        {
            if (SelectedArea == "Tất cả khu vực" || string.IsNullOrEmpty(SelectedArea))
                TableList = new ObservableCollection<Ban>(_context.Bans.ToList());
            else
                TableList = new ObservableCollection<Ban>(_context.Bans.Where(b => b.KhuVuc == SelectedArea).ToList());
        }

        private void HandleTableClick(Ban table)
        {
            if (table == null) return;
            MessageBox.Show($"Bạn đã chọn bàn: {table.TenBan} - Trạng thái: {table.TrangThai}");
        }
    }
}
