using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.Tenant.Request
{
    public class SearchTenantRequest
    {
        public string? SearchTerm { get; set; }
        public TenantStatus? Status { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public bool? IsActive { get; set; }
        public string? Domain { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Sayfa numarası 1'den büyük olmalıdır")]
        public int Page { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Sayfa boyutu 1-100 arasında olmalıdır")]
        public int PageSize { get; set; } = 10;

        public string SortBy { get; set; } = "CreatedAt";
        public bool SortDescending { get; set; } = true;
    }

    public enum TenantStatus
    {
        Active = 1,
        Inactive = 2,
        Suspended = 3,
        PendingActivation = 4
    }
}
