using Stoocker.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.User.Response
{
    public sealed class UserDetailResponse : AuditableDto
    {
        public Guid Id { get; init; }
        public Guid TenantId { get; init; }
        public string TenantName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public string? PhoneNumber { get; init; }
        public string? ProfilePicture { get; init; }
        public bool IsActive { get; init; }
        public bool EmailConfirmed { get; init; }
        public bool PhoneNumberConfirmed { get; init; }
        public DateTime? LastLoginAt { get; init; }
        public string TimeZone { get; init; } = string.Empty;
        public string Language { get; init; } = string.Empty;
        public List<RoleInfo> Roles { get; init; } = new();
        public List<string> Permissions { get; init; } = new();

        public sealed record RoleInfo
        {
            public Guid Id { get; init; }
            public string Name { get; init; } = string.Empty;
            public string? Description { get; init; }
            public DateTime AssignedAt { get; init; }
        }
    }
}
