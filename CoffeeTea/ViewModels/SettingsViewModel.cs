using System;
using System.Globalization;
using System.Windows.Input;
using CoffeeTea.Models;
using CoffeeTea.Services;

namespace CoffeeTea.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly SettingsStorageService _settingsStorageService;
        private readonly NhanVien _authenticatedUser;

        private string _storeDisplayName;
        private string _storeAddress;
        private string _hotline;
        private string _openTime;
        private string _closeTime;
        private bool _enableLowStockAlert;
        private decimal _lowStockThreshold;
        private bool _enableAutoLock;
        private int _autoLockMinutes;
        private bool _confirmBeforeExit;
        private bool _enableDailySummaryNotification;
        private bool _isDarkTheme;
        private string _lastUpdatedInformation;
        private string _successMessage;
        private string _errorMessage;

        public SettingsViewModel(NhanVien authenticatedUser)
        {
            _authenticatedUser = authenticatedUser;
            _settingsStorageService = new SettingsStorageService();

            SaveSettingsCommand = new RelayCommand(_ => SaveSettings());
            RestoreDefaultsCommand = new RelayCommand(_ => RestoreDefaults());
            BackupSettingsCommand = new RelayCommand(_ => CreateBackup());

            LoadSettings();
        }

        public ICommand SaveSettingsCommand { get; private set; }

        public ICommand RestoreDefaultsCommand { get; private set; }

        public ICommand BackupSettingsCommand { get; private set; }

        public string StoreDisplayName
        {
            get { return _storeDisplayName; }
            set
            {
                if (_storeDisplayName == value)
                {
                    return;
                }

                _storeDisplayName = value;
                OnPropertyChanged(nameof(StoreDisplayName));
            }
        }

        public string StoreAddress
        {
            get { return _storeAddress; }
            set
            {
                if (_storeAddress == value)
                {
                    return;
                }

                _storeAddress = value;
                OnPropertyChanged(nameof(StoreAddress));
            }
        }

        public string Hotline
        {
            get { return _hotline; }
            set
            {
                if (_hotline == value)
                {
                    return;
                }

                _hotline = value;
                OnPropertyChanged(nameof(Hotline));
            }
        }

        public string OpenTime
        {
            get { return _openTime; }
            set
            {
                if (_openTime == value)
                {
                    return;
                }

                _openTime = value;
                OnPropertyChanged(nameof(OpenTime));
            }
        }

        public string CloseTime
        {
            get { return _closeTime; }
            set
            {
                if (_closeTime == value)
                {
                    return;
                }

                _closeTime = value;
                OnPropertyChanged(nameof(CloseTime));
            }
        }

        public bool EnableLowStockAlert
        {
            get { return _enableLowStockAlert; }
            set
            {
                if (_enableLowStockAlert == value)
                {
                    return;
                }

                _enableLowStockAlert = value;
                OnPropertyChanged(nameof(EnableLowStockAlert));
            }
        }

        public decimal LowStockThreshold
        {
            get { return _lowStockThreshold; }
            set
            {
                if (_lowStockThreshold == value)
                {
                    return;
                }

                _lowStockThreshold = value;
                OnPropertyChanged(nameof(LowStockThreshold));
            }
        }

        public bool EnableAutoLock
        {
            get { return _enableAutoLock; }
            set
            {
                if (_enableAutoLock == value)
                {
                    return;
                }

                _enableAutoLock = value;
                OnPropertyChanged(nameof(EnableAutoLock));
            }
        }

        public int AutoLockMinutes
        {
            get { return _autoLockMinutes; }
            set
            {
                if (_autoLockMinutes == value)
                {
                    return;
                }

                _autoLockMinutes = value;
                OnPropertyChanged(nameof(AutoLockMinutes));
            }
        }

        public bool ConfirmBeforeExit
        {
            get { return _confirmBeforeExit; }
            set
            {
                if (_confirmBeforeExit == value)
                {
                    return;
                }

                _confirmBeforeExit = value;
                OnPropertyChanged(nameof(ConfirmBeforeExit));
            }
        }

        public bool EnableDailySummaryNotification
        {
            get { return _enableDailySummaryNotification; }
            set
            {
                if (_enableDailySummaryNotification == value)
                {
                    return;
                }

                _enableDailySummaryNotification = value;
                OnPropertyChanged(nameof(EnableDailySummaryNotification));
            }
        }

        public bool IsDarkTheme
        {
            get { return _isDarkTheme; }
            set
            {
                if (_isDarkTheme == value)
                {
                    return;
                }

                _isDarkTheme = value;
                OnPropertyChanged(nameof(IsDarkTheme));
                ThemeManager.ApplyTheme(_isDarkTheme);
            }
        }

        public string LastUpdatedInformation
        {
            get { return _lastUpdatedInformation; }
            set
            {
                if (_lastUpdatedInformation == value)
                {
                    return;
                }

                _lastUpdatedInformation = value;
                OnPropertyChanged(nameof(LastUpdatedInformation));
            }
        }

        public string SuccessMessage
        {
            get { return _successMessage; }
            set
            {
                if (_successMessage == value)
                {
                    return;
                }

                _successMessage = value;
                OnPropertyChanged(nameof(SuccessMessage));
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (_errorMessage == value)
                {
                    return;
                }

                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        private void LoadSettings()
        {
            SoftwareSettingsModel settings = _settingsStorageService.Load();
            ApplySettings(settings);
            LastUpdatedInformation = BuildLastUpdatedText(settings);
        }

        private void SaveSettings()
        {
            ClearStatus();

            if (!ValidateInput())
            {
                return;
            }

            try
            {
                SoftwareSettingsModel settings = BuildCurrentSettings();
                settings.LastUpdatedBy = ResolveUpdatedBy();
                settings.LastUpdatedAt = DateTime.Now;

                _settingsStorageService.Save(settings);

                LastUpdatedInformation = BuildLastUpdatedText(settings);
                SuccessMessage = "Đã lưu cài đặt hệ thống thành công.";
            }
            catch (Exception)
            {
                ErrorMessage = "Không thể lưu cài đặt hệ thống. Vui lòng thử lại.";
            }
        }

        private void RestoreDefaults()
        {
            ClearStatus();

            try
            {
                SoftwareSettingsModel defaults = SoftwareSettingsModel.CreateDefault();
                defaults.LastUpdatedBy = ResolveUpdatedBy();
                defaults.LastUpdatedAt = DateTime.Now;

                ApplySettings(defaults);
                _settingsStorageService.Save(defaults);

                LastUpdatedInformation = BuildLastUpdatedText(defaults);
                SuccessMessage = "Đã khôi phục cài đặt mặc định.";
            }
            catch (Exception)
            {
                ErrorMessage = "Không thể khôi phục cài đặt mặc định.";
            }
        }

        private void CreateBackup()
        {
            ClearStatus();

            try
            {
                SoftwareSettingsModel settings = BuildCurrentSettings();
                string backupPath = _settingsStorageService.CreateBackup(settings);
                SuccessMessage = string.Format("Đã tạo bản sao lưu cấu hình tại: {0}", backupPath);
            }
            catch (Exception)
            {
                ErrorMessage = "Không thể tạo bản sao lưu cấu hình.";
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(StoreDisplayName))
            {
                ErrorMessage = "Tên hiển thị cửa hàng không được để trống.";
                return false;
            }

            if (!TimeSpan.TryParseExact(OpenTime, "hh\\:mm", CultureInfo.InvariantCulture, out TimeSpan openTime))
            {
                ErrorMessage = "Giờ mở cửa không hợp lệ. Định dạng đúng là HH:mm.";
                return false;
            }

            if (!TimeSpan.TryParseExact(CloseTime, "hh\\:mm", CultureInfo.InvariantCulture, out TimeSpan closeTime))
            {
                ErrorMessage = "Giờ đóng cửa không hợp lệ. Định dạng đúng là HH:mm.";
                return false;
            }

            if (openTime >= closeTime)
            {
                ErrorMessage = "Giờ mở cửa phải nhỏ hơn giờ đóng cửa.";
                return false;
            }

            if (LowStockThreshold < 0)
            {
                ErrorMessage = "Ngưỡng cảnh báo tồn kho không được âm.";
                return false;
            }

            if (AutoLockMinutes < 1 || AutoLockMinutes > 180)
            {
                ErrorMessage = "Thời gian tự động khóa phải từ 1 đến 180 phút.";
                return false;
            }

            return true;
        }

        private SoftwareSettingsModel BuildCurrentSettings()
        {
            return new SoftwareSettingsModel
            {
                StoreDisplayName = Normalize(StoreDisplayName),
                StoreAddress = Normalize(StoreAddress),
                Hotline = Normalize(Hotline),
                OpenTime = Normalize(OpenTime),
                CloseTime = Normalize(CloseTime),
                EnableLowStockAlert = EnableLowStockAlert,
                LowStockThreshold = LowStockThreshold,
                EnableAutoLock = EnableAutoLock,
                AutoLockMinutes = AutoLockMinutes,
                ConfirmBeforeExit = ConfirmBeforeExit,
                EnableDailySummaryNotification = EnableDailySummaryNotification,
                IsDarkTheme = IsDarkTheme,
                LastUpdatedBy = ResolveUpdatedBy(),
                LastUpdatedAt = DateTime.Now
            };
        }

        private void ApplySettings(SoftwareSettingsModel settings)
        {
            StoreDisplayName = settings.StoreDisplayName;
            StoreAddress = settings.StoreAddress;
            Hotline = settings.Hotline;
            OpenTime = settings.OpenTime;
            CloseTime = settings.CloseTime;
            EnableLowStockAlert = settings.EnableLowStockAlert;
            LowStockThreshold = settings.LowStockThreshold;
            EnableAutoLock = settings.EnableAutoLock;
            AutoLockMinutes = settings.AutoLockMinutes;
            ConfirmBeforeExit = settings.ConfirmBeforeExit;
            EnableDailySummaryNotification = settings.EnableDailySummaryNotification;
            IsDarkTheme = settings.IsDarkTheme;
        }

        private string ResolveUpdatedBy()
        {
            if (!string.IsNullOrWhiteSpace(_authenticatedUser?.HoTen))
            {
                return _authenticatedUser.HoTen.Trim();
            }

            if (!string.IsNullOrWhiteSpace(_authenticatedUser?.TenDangNhap))
            {
                return _authenticatedUser.TenDangNhap.Trim();
            }

            return "Người dùng hệ thống";
        }

        private static string BuildLastUpdatedText(SoftwareSettingsModel settings)
        {
            return string.Format(
                "Cập nhật lần cuối: {0:dd/MM/yyyy HH:mm} bởi {1}",
                settings.LastUpdatedAt,
                string.IsNullOrWhiteSpace(settings.LastUpdatedBy) ? "Hệ thống" : settings.LastUpdatedBy);
        }

        private void ClearStatus()
        {
            SuccessMessage = string.Empty;
            ErrorMessage = string.Empty;
        }

        private static string Normalize(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }
    }
}

