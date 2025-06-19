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
using Stoocker.API.Extensions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();

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
    });
    builder.Services.AddAuthorization();

    var app = builder.Build();
     
    app.UseSwaggerExtension();
    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();
    app.UseCors("AllowAll");
    app.UseInfrastructure(app.Configuration);
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseUnauthorizedResponseHandlingMiddleware();
    app.UseErrorHandlingMiddleware();
    app.UseTenantHandlingMiddleware();
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