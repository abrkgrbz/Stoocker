using Stoocker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Domain.Entities
{
    public class Unit : BaseEntity, ITenantEntity
    {
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty; 
        public string ShortName { get; set; } = string.Empty;  
        public bool AllowDecimals { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public bool IsSystemUnit { get; set; } = false;

        public virtual Tenant Tenant { get; set; } = null!;
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }

}
