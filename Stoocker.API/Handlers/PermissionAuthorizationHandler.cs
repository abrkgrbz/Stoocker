using Microsoft.AspNetCore.Authorization;
using Stoocker.Application.Interfaces.Services.Permission;

namespace Stoocker.API.Handlers
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionAuthorizationHandler.PermissionRequirement>
    {
        public class PermissionRequirement : IAuthorizationRequirement
        {
            public string[] Permissions { get; }
            public bool RequireAll { get; }

            public PermissionRequirement(string[] permissions, bool requireAll = true)
            {
                Permissions = permissions;
                RequireAll = requireAll;
            }
        }
        private readonly IPermissionService _permissionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionAuthorizationHandler(
            IPermissionService permissionService,
            IHttpContextAccessor httpContextAccessor)
        {
            _permissionService = permissionService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var user = context.User;
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Fail();
                return;
            }

            var userIdClaim = user.FindFirst("UserId")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                context.Fail();
                return;
            }

            bool hasPermission;
            if (requirement.RequireAll)
            {
                hasPermission = await _permissionService.HasPermissionsAsync(userId, requirement.Permissions);
            }
            else
            {
                hasPermission = await _permissionService.HasAnyPermissionAsync(userId, requirement.Permissions);
            }

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }

}