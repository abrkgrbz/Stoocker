using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.Tenant.Request
{
    public class SuspendTenantRequest
    {
        [Required(ErrorMessage = "Tenant ID zorunludur")]
        public Guid TenantId { get; set; }

        [Required(ErrorMessage = "Askıya alma sebebi zorunludur")]
        [StringLength(500, ErrorMessage = "Sebep maksimum 500 karakter olabilir")]
        public string Reason { get; set; } = string.Empty;

        public DateTime? SuspendUntil { get; set; }

        public bool NotifyUsers { get; set; } = true;

        public string? AdditionalNotes { get; set; }
    }
}
