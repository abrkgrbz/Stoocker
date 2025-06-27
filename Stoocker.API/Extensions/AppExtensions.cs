using Serilog;
using Stoocker.API.Middlewares;

namespace Stoocker.API.Extensions
{
    public static class AppExtensions
    {
        public static void UseSwaggerExtension(this IApplicationBuilder app)
        {
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(options =>
            { 
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Stoocker API v1"); 
                options.SwaggerEndpoint("/swagger/admin/swagger.json", "Stoocker Admin API"); 
                options.DocumentTitle = "Stoocker API Documentation";
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                options.DefaultModelExpandDepth(2);
                options.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
                options.DisplayRequestDuration();
                options.EnableFilter();
                options.EnableTryItOutByDefault(); 
                options.RoutePrefix = "swagger";    
            });
             
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/admin/swagger.json", "Admin API");
                options.RoutePrefix = "admin/swagger";
                options.DocumentTitle = "Stoocker Admin API"; 
                options.InjectStylesheet("/swagger-ui/admin-theme.css");
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

        public static void UseAdminAreaHandlingMiddleware(this IApplicationBuilder app)
        {
            app.UseAdminAreaMiddleware();
        }



    }
}
