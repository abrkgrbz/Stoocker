using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Stoocker.API.Middlewares
{
    public class AdminAuthenticationHandler : AuthenticationHandler<AdminAuthenticationOptions>
    {
        public AdminAuthenticationHandler(
            IOptionsMonitor<AdminAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Cookie'den token al
            if (!Request.Cookies.TryGetValue("admin-auth-token", out var token))
            {
                // Header'dan da kontrol et
                if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    return AuthenticateResult.NoResult();
                }

                token = authHeader.ToString().Replace("Bearer ", "");
            }

            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.NoResult();
            }

            try
            {
                // Token'ı doğrula
                var principal = ValidateToken(token);
                if (principal == null)
                {
                    return AuthenticateResult.Fail("Invalid token");
                }

                // Admin claim kontrolü
                if (!principal.HasClaim("AdminAccess", "true"))
                {
                    return AuthenticateResult.Fail("Not an admin token");
                }

                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Admin authentication failed");
                return AuthenticateResult.Fail("Authentication failed");
            }
        }

        private ClaimsPrincipal? ValidateToken(string token)
        {
            // JWT token validation logic here
            // Use JwtSecurityTokenHandler to validate
            return null; // Implement actual validation
        }
    }

    public class AdminAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string SecretKey { get; set; } = string.Empty;
    }

    public static class AdminAuthenticationExtensions
    {
        public static AuthenticationBuilder AddAdminAuthentication(
            this AuthenticationBuilder builder,
            Action<AdminAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<AdminAuthenticationOptions, AdminAuthenticationHandler>(
                "AdminAuth", configureOptions);
        }
    }
}