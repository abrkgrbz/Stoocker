using Serilog;
using Stoocker.API.Middlewares;

namespace Stoocker.API.Extensions
{
    public static class AppExtensions
    {
        public static void UseSwaggerExtension(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Stoocker API V1");
                c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
            });
        }

        public static void UseUnauthorizedResponseHandlingMiddleware(this IApplicationBuilder app)
        { 
            app.UseMiddleware<UnauthorizedResponseMiddleware>();
        }

        public static void UseErrorHandlingMiddleware(this IApplicationBuilder app)
        {

            app.UseMiddleware<ErrorHandlerMiddleware>();
        }

        public static void UseSerilogRequestLogging(this IApplicationBuilder app)
        {
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                    diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
                };
            });
        }

        public static void UseTenantHandlingMiddleware(this IApplicationBuilder app)
        {

            app.UseMiddleware<TenantMiddleware>();
        }

    }
}
