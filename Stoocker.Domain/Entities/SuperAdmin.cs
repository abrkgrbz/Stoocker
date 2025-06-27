using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Domain.Common;

namespace Stoocker.Domain.Entities
{
    public class SuperAdmin:BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; } = true;
        public bool TwoFactorEnabled { get; set; } = false;
        public DateTime? LastLoginDate { get; set; }
        public int FailedLoginCount { get; set; } = 0;
        public DateTime? LockoutEndDate { get; set; }

        // Audit fields
        public string? LastLoginIp { get; set; }
        public string? LastLoginUserAgent { get; set; }

        // Security
        public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
