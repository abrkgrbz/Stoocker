using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.Tenant.Request
{
    public class UpdateTenantRequest
    {
        [Required(ErrorMessage = "Tenant ID zorunludur")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Tenant adı zorunludur")]
        [StringLength(200, ErrorMessage = "Tenant adı maksimum 200 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        [StringLength(200, ErrorMessage = "Email maksimum 200 karakter olabilir")]
        public string? ContactEmail { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        [StringLength(20, ErrorMessage = "Telefon numarası maksimum 20 karakter olabilir")]
        public string? ContactPhone { get; set; }

        [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Geçerli bir hex color kodu giriniz")]
        public string PrimaryColor { get; set; } = "#3498db";

        public bool IsActive { get; set; }

        // Güncelleme sebebi
        public string? UpdateReason { get; set; }
    }
}
