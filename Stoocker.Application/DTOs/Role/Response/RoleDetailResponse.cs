using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.Role.Response
{
    public class RoleDetailResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? NormalizedName { get; set; }
        public Guid TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        // İlişkili veriler
        public int UserCount { get; set; }
        public List<UserRoleSummary> Users { get; set; } = new();

        // İstatistikler
        public DateTime? LastAssignedAt { get; set; }
        public bool CanBeDeleted { get; set; }
    }
}
