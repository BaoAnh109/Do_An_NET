using CoffeeTea.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CoffeeTea.ViewModels
{
    public class CategoryViewModel : BaseViewModel
    {
        private QL_CoffeeTeaEntities db = new QL_CoffeeTeaEntities();
        private ObservableCollection<DanhMucMon> _categories;
        public ObservableCollection<DanhMucMon> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged("Categories"); 
            }
        }

        private DanhMucMon _selectedCategory;
        public DanhMucMon SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged("SelectedCategory"); 
                if (_selectedCategory != null)
                {
                    TenDanhMuc = _selectedCategory.TenDanhMuc;
                    MoTa = _selectedCategory.MoTa;
                }
            }
        }

        private string _tenDanhMuc;
        public string TenDanhMuc
        {
            get => _tenDanhMuc;
            set
            {
                _tenDanhMuc = value;
                OnPropertyChanged("TenDanhMuc"); 
            }
        }

        private string _moTa;
        public string MoTa
        {
            get => _moTa;
            set
            {
                _moTa = value;
                OnPropertyChanged("MoTa"); 
            }
        }

        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public CategoryViewModel()
        {
            LoadData();
            AddCommand = new RelayCommand(
                (p) => {
                    string autoId = GenerateNewId();
                    var item = new DanhMucMon
                     {
                         MaDanhMuc = autoId,
                         TenDanhMuc = TenDanhMuc,
                         MoTa = MoTa
                     };
                         db.DanhMucMons.Add(item);
                         db.SaveChanges(); 
                         LoadData(); 
                         ClearInput(); 
                     },
                 (p) => !string.IsNullOrEmpty(TenDanhMuc) 
                 );
            UpdateCommand = new RelayCommand(
                (p) => {
                    var item = db.DanhMucMons.Find(SelectedCategory.MaDanhMuc);
                    if (item != null)
                    {
                        item.TenDanhMuc = TenDanhMuc;
                        item.MoTa = MoTa;
                        db.SaveChanges();
                        LoadData();
                    }
                },
                (p) => SelectedCategory != null
            );

         
            DeleteCommand = new RelayCommand(
                (p) => {
                    var item = db.DanhMucMons.Find(SelectedCategory.MaDanhMuc);
                    if (item != null)
                    {
                        db.DanhMucMons.Remove(item);
                        db.SaveChanges();
                        LoadData();
                        ClearInput();
                    }
                },
                (p) => SelectedCategory != null
            );
        }
        private string GenerateNewId()
        {
                        var lastCategory = db.DanhMucMons
                                 .OrderByDescending(x => x.MaDanhMuc)
                                 .FirstOrDefault();
            if (lastCategory == null) return "DM01";

            string lastId = lastCategory.MaDanhMuc; 
            try
            {
                
                int number = int.Parse(lastId.Substring(2)) + 1;
                return "DM" + number.ToString("D2"); 
            }
            catch
            {
                return "DM" + (db.DanhMucMons.Count() + 1).ToString("D2");
            }
        }

        void LoadData() => Categories = new ObservableCollection<DanhMucMon>(db.DanhMucMons.ToList());
        void ClearInput()
        {
            TenDanhMuc = "";
            MoTa = "";
            SelectedCategory = null;
        }
    }
}