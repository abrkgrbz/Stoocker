using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.Tenant.Request
{
    public class UpdateTenantSettingsRequest
    {
        [Required(ErrorMessage = "Tenant ID zorunludur")]
        public Guid TenantId { get; set; }

        // Genel ayarlar
        [Required(ErrorMessage = "Varsayılan timezone zorunludur")]
        public string DefaultTimeZone { get; set; } = "Europe/Istanbul";

        [Required(ErrorMessage = "Varsayılan dil zorunludur")]
        public string DefaultLanguage { get; set; } = "tr-TR";

        public string DateFormat { get; set; } = "dd/MM/yyyy";
        public string TimeFormat { get; set; } = "HH:mm";

        // Güvenlik ayarları
        [Range(6, 20, ErrorMessage = "Parola minimum uzunluğu 6-20 arasında olmalıdır")]
        public int PasswordMinLength { get; set; } = 8;

        public bool RequireUppercase { get; set; } = true;
        public bool RequireLowercase { get; set; } = true;
        public bool RequireNumbers { get; set; } = true;
        public bool RequireSpecialCharacters { get; set; } = false;

        [Range(0, 365, ErrorMessage = "Parola geçerlilik süresi 0-365 gün arasında olmalıdır")]
        public int PasswordExpirationDays { get; set; } = 90;

        [Range(3, 10, ErrorMessage = "Maksimum başarısız giriş denemesi 3-10 arasında olmalıdır")]
        public int MaxFailedLoginAttempts { get; set; } = 5;

        [Range(5, 60, ErrorMessage = "Hesap kilitleme süresi 5-60 dakika arasında olmalıdır")]
        public int LockoutDurationMinutes { get; set; } = 30;

        // Kullanıcı ayarları
        public bool AllowUserRegistration { get; set; } = false;
        public bool RequireEmailConfirmation { get; set; } = true;
        public bool AllowPasswordReset { get; set; } = true;

        // Bildirim ayarları
        public bool EnableEmailNotifications { get; set; } = true;
        public bool EnableSmsNotifications { get; set; } = false;

        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string? NotificationEmailFrom { get; set; }

        // UI ayarları
        [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Geçerli bir hex color kodu giriniz")]
        public string PrimaryColor { get; set; } = "#3498db";

        [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Geçerli bir hex color kodu giriniz")]
        public string SecondaryColor { get; set; } = "#2c3e50";

        [Url(ErrorMessage = "Geçerli bir URL giriniz")]
        public string LogoUrl { get; set; } = string.Empty;

        [Url(ErrorMessage = "Geçerli bir URL giriniz")]
        public string FaviconUrl { get; set; } = string.Empty;

        // İş ayarları
        [Required(ErrorMessage = "Para birimi zorunludur")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "Para birimi 3 karakter olmalıdır")]
        public string Currency { get; set; } = "TRY";

        [Required(ErrorMessage = "Para birimi sembolü zorunludur")]
        public string CurrencySymbol { get; set; } = "₺";

        [Range(0, 1, ErrorMessage = "Vergi oranı 0-1 arasında olmalıdır")]
        public decimal TaxRate { get; set; } = 0.20m;

        // Entegrasyon ayarları
        public bool EnableApiAccess { get; set; } = true;

        [Range(100, 10000, ErrorMessage = "API rate limit 100-10000 arasında olmalıdır")]
        public int ApiRateLimit { get; set; } = 1000;

        public bool EnableWebhooks { get; set; } = false;

        // Güncelleme sebebi
        public string? UpdateReason { get; set; }
    }
}
