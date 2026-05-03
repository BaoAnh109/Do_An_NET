using CoffeeTea.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Data.Entity;

namespace CoffeeTea.ViewModels
{
    public class DrinkViewModel : BaseViewModel
    {
        private QL_CoffeeTeaEntities db = new QL_CoffeeTeaEntities();

        private ObservableCollection<Mon> _drinks;
        public ObservableCollection<Mon> Drinks
        {
            get { return _drinks; }
            set
            {
                _drinks = value;
                OnPropertyChanged("Drinks");
            }
        }

        private ObservableCollection<DanhMucMon> _categories;
        public ObservableCollection<DanhMucMon> Categories
        {
            get { return _categories; }
            set
            {
                _categories = value;
                OnPropertyChanged("Categories");
            }
        }

        private string _tenMon;
        public string TenMon
        {
            get { return _tenMon; }
            set
            {
                _tenMon = value;
                OnPropertyChanged("TenMon");
            }
        }
        private string _donViTinh;
        public string DonViTinh
        {
            get { return _donViTinh; }
            set { _donViTinh = value; OnPropertyChanged("DonViTinh"); }
        }

        private decimal? _donGia;
        public decimal? DonGia
        {
            get { return _donGia; }
            set
            {
                _donGia = value;
                OnPropertyChanged("DonGia");
            }
        }

        private DanhMucMon _selectedCategoryInForm;
        public DanhMucMon SelectedCategoryInForm
        {
            get { return _selectedCategoryInForm; }
            set
            {
                _selectedCategoryInForm = value;
                OnPropertyChanged("SelectedCategoryInForm");
            }
        }

        private Mon _selectedDrink;
        public Mon SelectedDrink
        {
            get { return _selectedDrink; }
            set
            {
                _selectedDrink = value;
                OnPropertyChanged("SelectedDrink");
                if (SelectedDrink != null)
                {
                    TenMon = SelectedDrink.TenMon;
                    DonGia = SelectedDrink.DonGia;
                    DonViTinh = SelectedDrink.DonViTinh; 
                    SelectedCategoryInForm = Categories.FirstOrDefault(x => x.MaDanhMuc == SelectedDrink.MaDanhMuc);
                }
            }
        }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public DrinkViewModel()
        {
            LoadData();

            AddCommand = new RelayCommand(
                 (p) => {
                     var newDrink = new Mon()
                     {
                         MaMon = GenerateNewId(),
                         TenMon = TenMon,
                         DonGia = DonGia ?? 0,
                         MaDanhMuc = SelectedCategoryInForm.MaDanhMuc,
                         DonViTinh = string.IsNullOrEmpty(DonViTinh) ? "Ly" : DonViTinh,
                         TrangThai = "Đang bán"
                      };
                         db.Mons.Add(newDrink);
                         db.SaveChanges();
                         LoadData();
                         ClearInputs();
                         (AddCommand as RelayCommand)?.RaiseCanExecuteChanged();
                      },
             (p) => !string.IsNullOrEmpty(TenMon) && !string.IsNullOrEmpty(DonViTinh) && SelectedCategoryInForm != null
             );

            EditCommand = new RelayCommand(
                (p) => {
                    var drink = db.Mons.FirstOrDefault(x => x.MaMon == SelectedDrink.MaMon);
                    if (drink != null)
                    {
                        drink.TenMon = TenMon;
                        drink.DonGia = DonGia ?? 0;
                        drink.MaDanhMuc = SelectedCategoryInForm.MaDanhMuc;
                        drink.DonViTinh = DonViTinh; 

                        db.SaveChanges();
                        LoadData();
                    }
                },
                (p) => SelectedDrink != null
            );

            DeleteCommand = new RelayCommand(
                (p) => {
                    var drink = db.Mons.FirstOrDefault(x => x.MaMon == SelectedDrink.MaMon);
                    if (drink != null)
                    {
                        db.Mons.Remove(drink);
                        db.SaveChanges();
                        LoadData();
                        ClearInputs();
                        (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
                        (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    }
                },
                (p) => SelectedDrink != null
            );
        }

        public void LoadData()
        {
            Drinks = new ObservableCollection<Mon>(db.Mons.Include(m => m.DanhMucMon).ToList());
            Categories = new ObservableCollection<DanhMucMon>(db.DanhMucMons.ToList());
        }

        private void ClearInputs()
        {
            TenMon = string.Empty;
            DonGia = null;
            SelectedCategoryInForm = null;
            SelectedDrink = null;
        }

        private string GenerateNewId()
        {
            var lastDrink = db.Mons.OrderByDescending(x => x.MaMon).FirstOrDefault();
            if (lastDrink == null) return "M01";

            try
            {

                int number = int.Parse(lastDrink.MaMon.Substring(1)) + 1;
                return "M" + number.ToString("D2");
            }
            catch
            {
                return "M" + (db.Mons.Count() + 1).ToString("D2");
            }
        }
    }
}