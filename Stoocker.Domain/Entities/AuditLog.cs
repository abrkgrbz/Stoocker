using Stoocker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Domain.Entities
{
    public class AuditLog : BaseEntity, ITenantEntity
    {
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
        public string Action { get; set; } = string.Empty; // CREATE, UPDATE, DELETE
        public string EntityName { get; set; } = string.Empty; // Product, Customer, etc.
        public string EntityId { get; set; } = string.Empty;
        public string? OldValues { get; set; } // JSON
        public string? NewValues { get; set; } // JSON
        public string? Changes { get; set; } // JSON of changed fields
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;

        // Navigation Properties
        public virtual Tenant Tenant { get; set; } = null!;
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
