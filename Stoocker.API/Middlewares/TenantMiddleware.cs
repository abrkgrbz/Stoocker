using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Application.Interfaces.Services;

namespace Stoocker.API.Middlewares
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork,ICurrentUserService currentUserService)
        {
            var token = context.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                var tenantId = currentUserService.TenantId;
                if (tenantId.HasValue)
                {
                    unitOfWork.SetCurrentTenant(tenantId.Value);
                }
            }

            await _next(context);
        }
    }
}
