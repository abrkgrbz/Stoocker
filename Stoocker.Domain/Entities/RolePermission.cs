using Stoocker.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Domain.Entities
{
    public class RolePermission : BaseEntity 
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public bool IsGranted { get; set; } = true;
        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
        public Guid? GrantedBy { get; set; }

        // Navigation Properties
        public virtual ApplicationRole Role { get; set; } = null!;
        public virtual Permission Permission { get; set; } = null!;
    }
}
