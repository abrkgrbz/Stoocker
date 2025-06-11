 
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events; 
using Serilog.Formatting.Elasticsearch;
using Stoocker.Application.Interfaces.Services.Logger;

namespace Stoocker.Infrastructure.Logging
{
    public static class LoggingConfiguration
    {
        public static WebApplicationBuilder AddSerilogConfiguration(this WebApplicationBuilder builder)
        {
            var configuration = builder.Configuration;
            var environment = builder.Environment;

            // Create logger configuration  
            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithProcessId() 
                .Enrich.WithProperty("ApplicationName", "Stoocker")
                .Enrich.WithProperty("Version", typeof(LoggingConfiguration).Assembly.GetName().Version?.ToString() ?? "1.0.0");

            // Console sink  
            loggerConfiguration.WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
                theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code);

            // File sink  
            var logPath = configuration["Logging:FilePath"] ?? "logs/stoocker-.txt";
            loggerConfiguration.WriteTo.File(
                path: logPath,
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: 10 * 1024 * 1024, // 10MB  
                retainedFileCountLimit: 30,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");

            // Seq sink (for development)  
            var seqUrl = configuration["Logging:SeqUrl"];
            if (!string.IsNullOrEmpty(seqUrl))
            {
                loggerConfiguration.WriteTo.Seq(seqUrl);
            }

            // Elasticsearch sink (for production)  
            var elasticsearchUrl = configuration["Logging:ElasticsearchUrl"];
            if (!string.IsNullOrEmpty(elasticsearchUrl))
            {
                loggerConfiguration.WriteTo.Elasticsearch(
                    new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri(elasticsearchUrl))
                    {
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = Serilog.Sinks.Elasticsearch.AutoRegisterTemplateVersion.ESv7,
                        IndexFormat = $"stoocker-logs-{environment.EnvironmentName.ToLower()}-{DateTime.UtcNow:yyyy-MM}",
                        NumberOfShards = 2,
                        NumberOfReplicas = 1,
                        CustomFormatter = new ElasticsearchJsonFormatter()
                    });
            }

            // Set minimum level based on environment  
            if (environment.IsDevelopment())
            {
                loggerConfiguration.MinimumLevel.Debug();
            }
            else
            {
                loggerConfiguration.MinimumLevel.Information();
            }

            // Override minimum level for specific namespaces  
            loggerConfiguration
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .MinimumLevel.Override("Hangfire", LogEventLevel.Information);

            // Create logger  
            Log.Logger = loggerConfiguration.CreateLogger();

            // Use Serilog  
            builder.Host.UseSerilog();

            return builder;
        }

        public static void AddLoggingServices(this IServiceCollection services)
        { 
            services.AddScoped<ITenantAwareLogger, TenantAwareLogger>();
             
            services.AddScoped<PerformanceLoggingMiddleware>();
             
            services.AddScoped<IAuditLogger, AuditLogger>();

           
        }
    }
}
