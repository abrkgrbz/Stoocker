using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.Role.Response
{
    public sealed class RoleResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public bool IsSystemRole { get; init; }
        public bool IsActive { get; init; }
        public int UserCount { get; init; }
        public List<string> Permissions { get; init; } = new();
    }
}
