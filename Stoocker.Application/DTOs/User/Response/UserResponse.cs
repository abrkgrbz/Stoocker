using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.DTOs.User.Response
{
    public sealed record UserResponse
    {
        public Guid Id { get; init; }
        public string Email { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public string? PhoneNumber { get; init; }
        public bool IsActive { get; init; }
        public DateTime? LastLoginAt { get; init; }
        public List<string> Roles { get; init; } = new();
    }
}
