using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.Role.Request
{
    public sealed record UpdateRoleRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; init; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; init; }

        public List<string> Permissions { get; init; } = new();
    }
}
