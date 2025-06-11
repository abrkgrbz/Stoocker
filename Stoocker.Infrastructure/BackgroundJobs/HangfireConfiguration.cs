using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Stoocker.Application.Interfaces.Services;
using Stoocker.Application.Interfaces.Services.BackgrounJobs;

namespace Stoocker.Infrastructure.BackgroundJobs
{
    public static class HangfireConfiguration
    {
        public static void AddHangfireConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.ConnectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true,
                    SchemaName = "Hangfire"
                })
                .UseFilter(new AutomaticRetryAttribute { Attempts = 3 })
                .UseFilter(new LogJobAttribute())
                .UseFilter(new TenantJobFilter())
                .UseSerilogLogProvider());

            services.AddHangfireServer(options =>
            {
                options.ServerName = $"Stoocker-{Environment.MachineName}";
                options.WorkerCount = Environment.ProcessorCount * 2;
                options.Queues = new[] { "critical", "default", "low", "tenant-specific" };
                options.HeartbeatInterval = TimeSpan.FromMinutes(1);
                options.ServerCheckInterval = TimeSpan.FromMinutes(1);
                options.SchedulePollingInterval = TimeSpan.FromSeconds(15);
            });

            services.AddScoped<IBackgroundJobService, BackgroundJobService>();

            services.AddScoped<IDataCleanupJob, DataCleanupJob>();

            services.AddScoped<IHealthCheckJob, HealthCheckJob>();

            services.AddScoped<IStockAlertJob, StockAlertJob>();

            services.AddScoped<ISystemMaintenanceJob, SystemMaintenanceJob>();
              
        }

        public static void UseHangfireConfiguration(this IApplicationBuilder app,IConfiguration configuration)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() },
                DashboardTitle = "Stoocker - Background Jobs",
                DisplayStorageConnectionString = false,
                DarkModeEnabled = true
            });

            // Register recurring jobs
            RecurringJob.AddOrUpdate<ISystemMaintenanceJob>(
                "system-maintenance",
                job => job.ExecuteAsync(CancellationToken.None),
                Cron.Daily(3)); // 3 AM every day

            RecurringJob.AddOrUpdate<IDataCleanupJob>(
                "data-cleanup",
                job => job.ExecuteAsync(CancellationToken.None),
                Cron.Weekly(DayOfWeek.Sunday, 2)); // 2 AM every Sunday

            RecurringJob.AddOrUpdate<IHealthCheckJob>(
                "health-check",
                job => job.ExecuteAsync(CancellationToken.None),
                Cron.MinuteInterval(5)); // Every 5 minutes

        }
    }
}
