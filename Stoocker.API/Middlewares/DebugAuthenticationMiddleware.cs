namespace Stoocker.API.Middlewares
{
    public class DebugAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DebugAuthenticationMiddleware> _logger;

        public DebugAuthenticationMiddleware(RequestDelegate next, ILogger<DebugAuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/admin"))
            {
                _logger.LogInformation("=== Admin Area Access Debug ===");
                _logger.LogInformation($"Path: {context.Request.Path}");
                _logger.LogInformation($"Is Authenticated: {context.User.Identity?.IsAuthenticated}");

                if (context.User.Identity?.IsAuthenticated == true)
                {
                    _logger.LogInformation($"User: {context.User.Identity.Name}");
                    _logger.LogInformation("Claims:");
                    foreach (var claim in context.User.Claims)
                    {
                        _logger.LogInformation($"  {claim.Type}: {claim.Value}");
                    }

                    _logger.LogInformation("Roles:");
                    var roles = context.User.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role);
                    foreach (var role in roles)
                    {
                        _logger.LogInformation($"  Role: {role.Value}");
                    }

                    _logger.LogInformation($"Is in SuperAdmin role: {context.User.IsInRole("SuperAdmin")}");
                }
            }

            await _next(context);
        }
    }
}
