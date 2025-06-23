using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using Stoocker.Application.Interfaces.Services.Monitoring;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Stoocker.Infrastructure.Monitoring
{
    public static class MonitoringConfiguration
    {
        public static IServiceCollection AddMonitoringConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Add health checks
            services.AddHealthChecks()
                .AddSqlServer(
                    configuration.GetConnectionString("SqlConnection") ?? "",
                    name: "database",
                    tags: new[] { "db", "sql", "sqlserver" })
                .AddHangfire(
                    options => options.MinimumAvailableServers = 1,
                    name: "hangfire",
                    tags: new[] { "hangfire", "jobs" })
                .AddCheck<DiskSpaceHealthCheck>(
                    "disk_space",
                    tags: new[] { "disk", "storage" })
                .AddCheck<MemoryHealthCheck>(
                    "memory",
                    tags: new[] { "memory", "ram" })
                .AddCheck<CpuHealthCheck>(
                    "cpu",
                    tags: new[] { "cpu", "processor" });

            // Redis health check ekle
            var redisConnection = configuration.GetConnectionString("RedisConnection");
            if (!string.IsNullOrEmpty(redisConnection))
            {
                services.AddHealthChecks()
                    .AddRedis(redisConnection, name: "redis", tags: new[] { "cache", "redis" });
            }

            // Add health check UI
            services.AddHealthChecksUI(options =>
            {
                options.SetEvaluationTimeInSeconds(30);
                options.MaximumHistoryEntriesPerEndpoint(50);
                options.AddHealthCheckEndpoint("Stoocker API Health", "/health");
            }).AddInMemoryStorage();

            // Add OpenTelemetry
            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource
                    .AddService(
                        serviceName: "Stoocker",
                        serviceVersion: typeof(MonitoringConfiguration).Assembly.GetName().Version?.ToString() ?? "1.0.0"))
                .WithTracing(tracing => tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation())
                .WithMetrics(metrics => metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation());

            // Add custom metrics
            services.AddSingleton<IMetricsService, MetricsService>();

            return services;
        }
        public static IApplicationBuilder UseMonitoringConfiguration(this IApplicationBuilder app)
        {
            // Use health checks
            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = WriteHealthCheckResponse
            });

            // Use health checks UI
            app.UseHealthChecksUI(options =>
            {
                options.UIPath = "/health-ui";
                options.ApiPath = "/health-ui-api";
            });

            // Use Prometheus metrics
            app.UseHttpMetrics(); // Automatic HTTP metrics
            app.UseMetricServer(); // Expose /metrics endpoint

            return app;
        }

        private static async Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            var response = new
            {
                status = report.Status.ToString(),
                duration = report.TotalDuration.TotalMilliseconds,
                info = report.Entries.Select(e => new
                {
                    key = e.Key,
                    description = e.Value.Description,
                    duration = e.Value.Duration.TotalMilliseconds,
                    status = e.Value.Status.ToString(),
                    error = e.Value.Exception?.Message,
                    data = e.Value.Data
                })
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }

}
