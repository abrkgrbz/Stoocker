using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stoocker.Infrastructure.BackgroundJobs;
using Stoocker.Infrastructure.Logging;
using Stoocker.Infrastructure.Monitoring;

namespace Stoocker.Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add logging services
            services.AddLoggingServices();

            // Add Hangfire
            services.AddHangfireConfiguration(configuration);

            // Add monitoring and health checks
            services.AddMonitoringConfiguration(configuration);

            return services;
        }

        public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
        {
            // Add Serilog
            builder.AddSerilogConfiguration();

            // Add infrastructure services
            builder.Services.AddInfrastructureServices(builder.Configuration);

            return builder;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app, IConfiguration configuration)
        {
            // Use performance logging middleware
            app.UsePerformanceLogging();

            // Use Hangfire
            app.UseHangfireConfiguration(configuration);

            // Use monitoring
            app.UseMonitoringConfiguration();

            return app;
        }
    }
}
