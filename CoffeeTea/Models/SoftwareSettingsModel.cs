using System;

namespace CoffeeTea.Models
{
    public class SoftwareSettingsModel
    {
        public string StoreDisplayName { get; set; }

        public string StoreAddress { get; set; }

        public string Hotline { get; set; }

        public string OpenTime { get; set; }

        public string CloseTime { get; set; }

        public bool EnableLowStockAlert { get; set; }

        public decimal LowStockThreshold { get; set; }

        public bool EnableAutoLock { get; set; }

        public int AutoLockMinutes { get; set; }

        public bool ConfirmBeforeExit { get; set; }

        public bool EnableDailySummaryNotification { get; set; }

        public bool IsDarkTheme { get; set; }

        public string LastUpdatedBy { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public static SoftwareSettingsModel CreateDefault()
        {
            return new SoftwareSettingsModel
            {
                StoreDisplayName = "CoffeeTea",
                StoreAddress = "Chi nhánh mặc định - vui lòng cập nhật địa chỉ thực tế.",
                Hotline = "0901 000 001",
                OpenTime = "06:30",
                CloseTime = "22:30",
                EnableLowStockAlert = true,
                LowStockThreshold = 10,
                EnableAutoLock = false,
                AutoLockMinutes = 30,
                ConfirmBeforeExit = true,
                EnableDailySummaryNotification = true,
                IsDarkTheme = false,
                LastUpdatedBy = "Hệ thống",
                LastUpdatedAt = DateTime.Now
            };
        }
    }
}
