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
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using OpenTelemetry.Metrics;
    using OpenTelemetry.Resources;
    using OpenTelemetry.Trace;
    using Prometheus; 
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using HealthChecks.UI.Client;
    using global::Stoocker.Infrastructure.BackgroundJobs;
    using OpenTelemetry.Exporter;

    namespace Stoocker.Infrastructure.Monitoring
    {
        public static class MonitoringConfiguration
        {
            public static IServiceCollection AddMonitoringConfiguration(this IServiceCollection services, IConfiguration configuration)
            {
                var monitoringConfig = configuration.GetSection("Monitoring");

                // Health Checks
                if (monitoringConfig.GetValue<bool>("HealthChecks:Enabled", true))
                {
                    AddHealthChecks(services, configuration);

                    // Health Checks UI
                    if (monitoringConfig.GetValue<bool>("HealthChecks:UI:Enabled", true))
                    {
                        AddHealthChecksUI(services, configuration);
                    }
                }

                // OpenTelemetry
                if (monitoringConfig.GetValue<bool>("OpenTelemetry:Enabled", true))
                {
                    AddOpenTelemetry(services, configuration);
                }

                // Prometheus
                if (monitoringConfig.GetValue<bool>("Prometheus:Enabled", true))
                {
                    // Prometheus metrics are handled by OpenTelemetry or standalone
                }

                // Custom metrics service
                services.AddSingleton<IMetricsService, MetricsService>();

                return services;
            }

            private static void AddHealthChecks(IServiceCollection services, IConfiguration configuration)
            {
                var sqlConnectionString = configuration.GetConnectionString("SqlConnection");
                var redisConnectionString = configuration.GetConnectionString("RedisConnection");

                var healthChecksBuilder = services.AddHealthChecks();

                // SQL Server
                if (!string.IsNullOrEmpty(sqlConnectionString))
                {
                    healthChecksBuilder.AddSqlServer(
                        sqlConnectionString,
                        name: "database",
                        tags: new[] { "db", "sql", "sqlserver" });
                }

                // Redis
                if (!string.IsNullOrEmpty(redisConnectionString))
                {
                    healthChecksBuilder.AddRedis(
                        redisConnectionString,
                        name: "redis",
                        tags: new[] { "cache", "redis" });
                }

                // Custom health checks
                healthChecksBuilder
                    .AddCheck<DiskSpaceHealthCheck>("disk_space", tags: new[] { "disk", "storage" })
                    .AddCheck<MemoryHealthCheck>("memory", tags: new[] { "memory", "ram" })
                    .AddCheck<CpuHealthCheck>("cpu", tags: new[] { "cpu", "processor" });

                // Hangfire (if configured)
                if (services.Any(x => x.ServiceType.Name.Contains("Hangfire")))
                {
                    healthChecksBuilder.AddHangfire(
                        options => options.MinimumAvailableServers = 1,
                        name: "hangfire",
                        tags: new[] { "hangfire", "jobs" });
                }
            }

            private static void AddHealthChecksUI(IServiceCollection services, IConfiguration configuration)
            {
                var uiConfig = configuration.GetSection("Monitoring:HealthChecks:UI");

                services.AddHealthChecksUI(options =>
                {
                    options.SetEvaluationTimeInSeconds(
                        uiConfig.GetValue<int>("EvaluationTimeInSeconds", 30));

                    options.MaximumHistoryEntriesPerEndpoint(50);

                    // Add configured endpoints
                    var endpoints = configuration.GetSection("Monitoring:HealthChecks:Endpoints").GetChildren();
                    foreach (var endpoint in endpoints)
                    {
                        var name = endpoint.GetValue<string>("Name");
                        var uri = endpoint.GetValue<string>("Uri");
                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(uri))
                        {
                            options.AddHealthCheckEndpoint(name, uri);
                        }
                    }
                })
                .AddInMemoryStorage();
            }

            private static void AddOpenTelemetry(IServiceCollection services, IConfiguration configuration)
            {
                var otConfig = configuration.GetSection("Monitoring:OpenTelemetry");
                var serviceName = otConfig.GetValue<string>("ServiceName") ?? "Stoocker";
                var serviceVersion = otConfig.GetValue<string>("ServiceVersion") ?? "1.0.0";

                services.AddOpenTelemetry()
                    .ConfigureResource(resource => resource
                        .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
                        .AddAttributes(new Dictionary<string, object>
                        {
                            ["deployment.environment"] = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development"
                        }))
                    .WithTracing(tracing =>
                    {
                        tracing.AddAspNetCoreInstrumentation(options =>
                        {
                            options.RecordException = otConfig.GetValue<bool>("Instrumentation:AspNetCore:RecordException", true);
                        })
                        .AddHttpClientInstrumentation(options =>
                        {
                            options.RecordException = otConfig.GetValue<bool>("Instrumentation:Http:RecordException", true);
                        });

                        // OTLP Exporter
                        if (otConfig.GetValue<bool>("Exporters:Otlp:Enabled", true))
                        {
                            tracing.AddOtlpExporter(options =>
                            {
                                options.Endpoint = new Uri(otConfig.GetValue<string>("Exporters:Otlp:Endpoint") ?? "http://localhost:4317");
                                options.Protocol = OtlpExportProtocol.Grpc;
                            });
                        }

                        // Console Exporter (for debugging)
                        if (otConfig.GetValue<bool>("Exporters:Console:Enabled", false))
                        {
                            tracing.AddConsoleExporter();
                        }

                        // Sampling
                        var samplingProbability = otConfig.GetValue<double>("Sampling:Probability", 1.0);
                        if (samplingProbability < 1.0)
                        {
                            tracing.SetSampler(new TraceIdRatioBasedSampler(samplingProbability));
                        }
                    })
                    .WithMetrics(metrics =>
                    {
                        metrics.AddAspNetCoreInstrumentation()
                               .AddHttpClientInstrumentation()
                               .AddRuntimeInstrumentation()
                               .AddProcessInstrumentation();

                        // Prometheus Exporter
                        if (otConfig.GetValue<bool>("Exporters:Prometheus:Enabled", true))
                        {
                            metrics.AddPrometheusExporter();
                        }

                        // OTLP Exporter for metrics
                        if (otConfig.GetValue<bool>("Exporters:Otlp:Enabled", true))
                        {
                            metrics.AddOtlpExporter(options =>
                            {
                                options.Endpoint = new Uri(otConfig.GetValue<string>("Exporters:Otlp:Endpoint") ?? "http://localhost:4317");
                                options.Protocol = OtlpExportProtocol.Grpc;
                            });
                        }
                    });
            }

            public static IApplicationBuilder UseMonitoringConfiguration(this IApplicationBuilder app, IConfiguration configuration)
            {
                var monitoringConfig = configuration.GetSection("Monitoring");

                // Health checks
                if (monitoringConfig.GetValue<bool>("HealthChecks:Enabled", true))
                {
                    app.UseHealthChecks("/health", new HealthCheckOptions
                    {
                        Predicate = _ => true,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    });

                    // Health checks UI
                    if (monitoringConfig.GetValue<bool>("HealthChecks:UI:Enabled", true))
                    {
                        var uiPath = monitoringConfig.GetValue<string>("HealthChecks:UI:Path") ?? "/health-ui";
                        var apiPath = monitoringConfig.GetValue<string>("HealthChecks:UI:ApiPath") ?? "/health-ui-api";

                        app.UseHealthChecksUI(options =>
                        {
                            options.UIPath = uiPath;
                            options.ApiPath = apiPath;
                        });
                    }
                }

                // Prometheus metrics
                if (monitoringConfig.GetValue<bool>("Prometheus:Enabled", true))
                {
                    app.UseHttpMetrics();

                    var metricsPath = monitoringConfig.GetValue<string>("Prometheus:MetricsPath") ?? "/metrics";
                    app.UseMetricServer(metricsPath);
                }

                return app;
            }
        }
    }

}
