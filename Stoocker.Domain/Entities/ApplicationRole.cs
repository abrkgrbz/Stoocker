using Microsoft.AspNetCore.Identity;
using Stoocker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Domain.Entities
{
    public class ApplicationRole : IdentityRole<Guid>, ITenantEntity
    {
        public Guid TenantId { get; set; }
        public string? Description { get; set; }
        public bool IsSystemRole { get; set; } = false; // Admin, SuperAdmin gibi sistem rolleri
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? Permissions { get; set; } // JSON string olarak permission'ları sakla

        // Navigation Properties
        public virtual Tenant Tenant { get; set; } = null!;
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
