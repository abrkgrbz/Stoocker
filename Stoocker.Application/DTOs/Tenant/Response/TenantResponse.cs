using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.Tenant.Response
{
    public class TenantResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Domain { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string PrimaryColor { get; set; } = "#3498db";
        public bool IsActive { get; set; }
        public TenantStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public enum TenantStatus
    {
        Active = 1,
        Inactive = 2,
        Suspended = 3,
        PendingActivation = 4
    }
}
