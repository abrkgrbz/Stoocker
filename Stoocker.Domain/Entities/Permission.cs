using Stoocker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Domain.Entities
{
    public class Permission: BaseEntity 
    {
        public string Name { get; set; } = string.Empty; // product.create
        public string DisplayName { get; set; } = string.Empty; // Ürün Oluştur
        public string Category { get; set; } = string.Empty; // Product Management
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;

        // Navigation Properties
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
