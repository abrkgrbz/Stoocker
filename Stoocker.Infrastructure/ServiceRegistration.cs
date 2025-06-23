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
using Stoocker.Infrastructure.Caching;

namespace Stoocker.Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        { 
            services.AddLoggingServices();
             
            services.AddHangfireConfiguration(configuration);
             
            services.AddMonitoringConfiguration(configuration);
             
            services.AddCachingServices(configuration);

            return services;
        }

        public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
        { 
            builder.AddSerilogConfiguration();
             
            builder.Services.AddInfrastructureServices(builder.Configuration);

            return builder;
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app, IConfiguration configuration)
        { 
            app.UsePerformanceLogging();
             
            app.UseHangfireConfiguration(configuration);
             
            app.UseMonitoringConfiguration();

            return app;
        }
    }
}
