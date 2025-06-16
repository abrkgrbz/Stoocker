using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.Tenant.Response
{
    public class TenantSettingsResponse
    {
        public Guid TenantId { get; set; }
        public TenantSettings Settings { get; set; } = new();
        public DateTime LastUpdated { get; set; }
        public string? LastUpdatedBy { get; set; }
    }

    public class TenantSettings
    {
        // Genel ayarlar
        public string DefaultTimeZone { get; set; } = "Europe/Istanbul";
        public string DefaultLanguage { get; set; } = "tr-TR";
        public string DateFormat { get; set; } = "dd/MM/yyyy";
        public string TimeFormat { get; set; } = "HH:mm";

        // Güvenlik ayarları
        public int PasswordMinLength { get; set; } = 8;
        public bool RequireUppercase { get; set; } = true;
        public bool RequireLowercase { get; set; } = true;
        public bool RequireNumbers { get; set; } = true;
        public bool RequireSpecialCharacters { get; set; } = false;
        public int PasswordExpirationDays { get; set; } = 90;
        public int MaxFailedLoginAttempts { get; set; } = 5;
        public int LockoutDurationMinutes { get; set; } = 30;

        // Kullanıcı ayarları
        public bool AllowUserRegistration { get; set; } = false;
        public bool RequireEmailConfirmation { get; set; } = true;
        public bool AllowPasswordReset { get; set; } = true;

        // Bildirim ayarları
        public bool EnableEmailNotifications { get; set; } = true;
        public bool EnableSmsNotifications { get; set; } = false;
        public string? NotificationEmailFrom { get; set; }

        // UI ayarları
        public string PrimaryColor { get; set; } = "#3498db";
        public string SecondaryColor { get; set; } = "#2c3e50";
        public string LogoUrl { get; set; } = string.Empty;
        public string FaviconUrl { get; set; } = string.Empty;

        // İş ayarları
        public string Currency { get; set; } = "TRY";
        public string CurrencySymbol { get; set; } = "₺";
        public decimal TaxRate { get; set; } = 0.20m;

        // Entegrasyon ayarları
        public bool EnableApiAccess { get; set; } = true;
        public int ApiRateLimit { get; set; } = 1000;
        public bool EnableWebhooks { get; set; } = false;
    }
}
