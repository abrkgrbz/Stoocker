using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.Tenant.Request
{
    public class CreateTenantRequest
    {
        [Required(ErrorMessage = "Tenant adı zorunludur")]
        [StringLength(200, ErrorMessage = "Tenant adı maksimum 200 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Domain maksimum 100 karakter olabilir")]
        [RegularExpression(@"^[a-zA-Z0-9][a-zA-Z0-9-]{0,61}[a-zA-Z0-9](\.[a-zA-Z0-9][a-zA-Z0-9-]{0,61}[a-zA-Z0-9])*$",
            ErrorMessage = "Geçerli bir domain formatı giriniz")]
        public string? Domain { get; set; }

        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        [StringLength(200, ErrorMessage = "Email maksimum 200 karakter olabilir")]
        public string? ContactEmail { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        [StringLength(20, ErrorMessage = "Telefon numarası maksimum 20 karakter olabilir")]
        public string? ContactPhone { get; set; }

        [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Geçerli bir hex color kodu giriniz")]
        public string PrimaryColor { get; set; } = "#3498db";

        public bool IsActive { get; set; } = true;

        // İlk kurulum için admin kullanıcı bilgileri
        public AdminUserSetup? AdminUser { get; set; }

        // Başlangıç ayarları
        public InitialTenantSettings? InitialSettings { get; set; }
    }

    public class AdminUserSetup
    {
        [Required(ErrorMessage = "Admin email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Admin adı zorunludur")]
        [StringLength(100, ErrorMessage = "Ad maksimum 100 karakter olabilir")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Admin soyadı zorunludur")]
        [StringLength(100, ErrorMessage = "Soyad maksimum 100 karakter olabilir")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Telefon numarası maksimum 20 karakter olabilir")]
        public string? PhoneNumber { get; set; }

        public bool SendWelcomeEmail { get; set; } = true;
    }

    public class InitialTenantSettings
    {
        public string TimeZone { get; set; } = "Europe/Istanbul";
        public string Language { get; set; } = "tr-TR";
        public string Currency { get; set; } = "TRY";
        public bool CreateDefaultRoles { get; set; } = true;
        public List<string> DefaultRoleNames { get; set; } = new() { "Admin", "User", "Manager" };
    }
}
