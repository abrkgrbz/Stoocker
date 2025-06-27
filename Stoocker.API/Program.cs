using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Stoocker.API.Middlewares;
using Stoocker.API.Services;
using Stoocker.Application;
using Stoocker.Application.Interfaces.Services;
using Stoocker.Domain.Entities;
using Stoocker.Infrastructure;
using Stoocker.Persistence;
using Stoocker.Persistence.Contexts;
using Stoocker.Persistence.Seeds;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Stoocker.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using static Stoocker.API.Handlers.PermissionAuthorizationHandler;
using Stoocker.API.Handlers;
using Stoocker.API.Providers;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddMemoryCache();

    builder.Services.AddHttpContextAccessor();

    builder.Configuration
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddJsonFile("appsettings.Logging.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();

    builder.Services.AddOpenApi();

    builder.Services.AddSwaggerExtension();

    builder.AddInfrastructure();

    builder.Services.AddPersistenceServices();

    builder.Services.AddApplicationServices();

    builder.Services.AddIdentityCoreExtension();

    builder.Services.AddAuthenticationExtension(builder.Configuration);

    builder.Services.AddPermissionAuthorizationHandlerExtension();

 

    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });

        options.AddPolicy("AdminPanel", policy =>
        {
            policy.WithOrigins(
                    "https://admin.stoocker.com",
                    "http://localhost:3001")  
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();  
        });
    });

    builder.Services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
    builder.Services.AddTransient<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

    builder.Services.AddAuthorization(options =>
    {
        
        options.DefaultPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
        options.AddPolicy("AdminOnly",policy=>
            policy.RequireRole("Admin","SuperAdmin"));

        options.AddPolicy("SuperAdmin", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole("SuperAdmin"); 
        });

        options.AddPolicy("WarehouseManager", policy =>
            policy.RequireAssertion(context =>
                context.User.IsInRole("WarehouseManager") ||
                context.User.IsInRole("Admin")));
    });
 

    var app = builder.Build();
    app.UseStaticFiles();
    app.UseSwaggerExtension();
    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.UseCors("AllowAll");
    app.UseInfrastructure(app.Configuration);
    app.UseAuthentication();
    app.UseAuthorization();

    if (app.Environment.IsDevelopment())
    {
        app.MapGet("/", () => Results.Redirect("/swagger"))
            .ExcludeFromDescription(); // Ana sayfayý swagger'a yönlendir
        app.UseMiddleware<DebugAuthenticationMiddleware>();
        //app.MapGet("/routes", (IEnumerable<EndpointDataSource> endpointSources) =>
        //{
        //    var endpoints = endpointSources.SelectMany(source => source.Endpoints);
        //    var output = endpoints.Select(endpoint =>
        //    {
        //        var controller = endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();
        //        var area = controller?.ControllerTypeInfo.GetCustomAttributes(typeof(AreaAttribute), true)
        //            .Cast<AreaAttribute>()
        //            .FirstOrDefault()?.RouteValue;

        //        var httpMethod = endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods.FirstOrDefault();

        //        return new
        //        {
        //            Method = httpMethod,
        //            Route = endpoint.DisplayName,
        //            Area = area
        //        };
        //    });
        //    return output;
        //});
    }

    app.UseAdminAreaHandlingMiddleware();
    app.UseUnauthorizedResponseHandlingMiddleware();
    app.UseErrorHandlingMiddleware();
    app.UseTenantHandlingMiddleware();
    app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
    app.MapControllers();
 

    using (var scope = app.Services.CreateScope())
    {
        await DatabaseInitializer.InitializeAsync(scope.ServiceProvider); 
          
    }

    Log.Information("Stoocker API started successfully");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}