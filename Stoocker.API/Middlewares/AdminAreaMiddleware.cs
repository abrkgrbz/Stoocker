using System.Security.Claims;

namespace Stoocker.API.Middlewares
{
    public class AdminAreaMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AdminAreaMiddleware> _logger;

        public AdminAreaMiddleware(RequestDelegate next, ILogger<AdminAreaMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/admin"))
            {
                // Auth controller'a erişime izin ver
                if (context.Request.Path.StartsWithSegments("/api/admin/auth"))
                {
                    await _next(context);
                    return;
                }

                // Kullanıcı giriş yapmış mı?
                if (!context.User.Identity?.IsAuthenticated ?? true)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized: Authentication required");
                    return;
                }

                // Admin area'ya giriş logla
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userEmail = context.User.FindFirst(ClaimTypes.Email)?.Value;

                _logger.LogInformation(
                    "Admin area access: User {UserId} ({Email}) accessing {Path}",
                    userId,
                    userEmail,
                    context.Request.Path);

                // SuperAdmin claim kontrolü (Role yerine claim kontrolü)
                var isSuperAdminClaim = context.User.HasClaim("IsSuperAdmin", "true") ||
                                       context.User.HasClaim("AdminType", "System");

                // Role kontrolü
                var isSuperAdminRole = context.User.IsInRole("SuperAdmin");

                if (!isSuperAdminClaim && !isSuperAdminRole)
                {
                    _logger.LogWarning(
                        "Unauthorized admin area access attempt by user {UserId}. Claims: {Claims}",
                        userId,
                        string.Join(", ", context.User.Claims.Select(c => $"{c.Type}={c.Value}")));

                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Forbidden: Super Admin access required");
                    return;
                }

                // Admin işlemleri için özel tenant context'i kaldır
                context.Items["IgnoreTenantContext"] = true;
            }

            await _next(context);
        }
    } 

    public static class AdminAreaMiddlewareExtensions
    {
        public static IApplicationBuilder UseAdminAreaMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AdminAreaMiddleware>();
        }
    }
}
