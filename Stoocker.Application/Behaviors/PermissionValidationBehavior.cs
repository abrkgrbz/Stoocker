using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Stoocker.Application.Exceptions;
using Stoocker.Application.Interfaces.Services;
using Stoocker.Application.Interfaces.Services.Permission;

namespace Stoocker.Application.Behaviors
{
    public class PermissionValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IPermissionService _permissionService;
        private readonly ICurrentUserService _currentUserService;

        public PermissionValidationBehavior(
            IPermissionService permissionService,
            ICurrentUserService currentUserService)
        {
            _permissionService = permissionService;
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        { 
            if (request is IRequirePermission permissionRequest)
            {
                if (!_currentUserService.IsAuthenticated || !_currentUserService.UserId.HasValue)
                {
                    throw new UnauthorizedException("User is not authenticated");
                }

                var hasPermission = await _permissionService.HasPermissionsAsync(
                    _currentUserService.UserId.Value,
                    permissionRequest.RequiredPermissions);

                if (!hasPermission)
                {
                    throw new ForbiddenException($"User does not have required permissions: {string.Join(", ", permissionRequest.RequiredPermissions)}");
                }
            }

            return await next();
        }
    }
}
