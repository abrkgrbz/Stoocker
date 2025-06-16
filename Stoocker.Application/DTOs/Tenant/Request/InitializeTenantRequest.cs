using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.Tenant.Request
{
    public class InitializeTenantRequest
    {
        [Required(ErrorMessage = "Tenant ID zorunludur")]
        public Guid TenantId { get; set; }

        // Admin kullanıcı bilgileri
        [Required(ErrorMessage = "Admin kullanıcı bilgileri zorunludur")]
        public AdminUserSetup AdminUser { get; set; } = new();

        // Başlangıç ayarları
        public InitialTenantSettings Settings { get; set; } = new();

        // Varsayılan veriler
        public DefaultDataSetup DefaultData { get; set; } = new();

        // Bildirim ayarları
        public bool SendWelcomeEmails { get; set; } = true;
        public bool CreateSampleData { get; set; } = false;
    }

    public class DefaultDataSetup
    {
        public bool CreateDefaultRoles { get; set; } = true;
        public List<DefaultRoleSetup> Roles { get; set; } = new();

        public bool CreateDefaultDepartments { get; set; } = false;
        public List<string> DepartmentNames { get; set; } = new();
    }

    public class DefaultRoleSetup
    {
        [Required(ErrorMessage = "Rol adı zorunludur")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public bool IsDefault { get; set; } = false;
        public List<string> Permissions { get; set; } = new();
    }
}
