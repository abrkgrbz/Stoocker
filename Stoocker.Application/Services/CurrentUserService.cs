using Microsoft.AspNetCore.Http;
using Stoocker.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public Guid? UserId
        {
            get
            {
                var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
            }
        }

        public Guid? TenantId
        {
            get
            {
                var tenantIdClaim = User?.FindFirst("TenantId")?.Value;
                return Guid.TryParse(tenantIdClaim, out var tenantId) ? tenantId : null;
            }
        }

        public string? UserName => User?.Identity?.Name;

        public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;

        public List<string> Roles => User?.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList() ?? new List<string>();

        public List<string> Permissions => User?.FindAll("Permission")
            .Select(c => c.Value)
            .ToList() ?? new List<string>();

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        public bool HasPermission(string permission)
        {
            return Permissions.Contains(permission) || Permissions.Contains("*");
        }

        public bool HasRole(string role)
        {
            return Roles.Contains(role);
        }

        public bool HasAnyRole(params string[] roles)
        {
            return roles.Any(role => Roles.Contains(role));
        }
    }
}
