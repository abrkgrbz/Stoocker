using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.Role.Request
{
    public sealed record CreateRoleRequest
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; init; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; init; }

        public List<string> Permissions { get; init; } = new();

        public bool IsSystemRole { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public Guid TenantId { get; init; } = Guid.Empty;
    }
}
