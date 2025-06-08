using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.Auth.Response
{
    public sealed class LoginResponse
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
        public UserInfo User { get; init; } = null!;

        public sealed record UserInfo
        {
            public Guid Id { get; init; }
            public string Email { get; init; } = string.Empty;
            public string FullName { get; init; } = string.Empty;
            public Guid TenantId { get; init; }
            public string TenantName { get; init; } = string.Empty;
            public List<string> Roles { get; init; } = new();
            public List<string> Permissions { get; init; } = new();
        }
    }
}
