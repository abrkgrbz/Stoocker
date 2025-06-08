using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Services
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        Guid? TenantId { get; }
        string? UserName { get; }
        string? Email { get; }
        List<string> Roles { get; }
        List<string> Permissions { get; }
        bool IsAuthenticated { get; }
        bool HasPermission(string permission);
        bool HasRole(string role);
        bool HasAnyRole(params string[] roles);
    }
}
