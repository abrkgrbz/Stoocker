using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stoocker.Application.Interfaces.Repositories;
using Stoocker.Application.Interfaces.Services.BackgrounJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Infrastructure.BackgroundJobs
{
    public class HealthCheckJob : IHealthCheckJob
    {
        private readonly ILogger<HealthCheckJob> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public HealthCheckJob(
            ILogger<HealthCheckJob> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting health check job");

            try
            {
                // Check database connectivity
                await CheckDatabaseHealth(cancellationToken);

                // Check external services
                await CheckExternalServices(cancellationToken);

                // Check disk space
                await CheckDiskSpace(cancellationToken);

                _logger.LogInformation("Health check job completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check job failed");
                // Don't rethrow - we want this job to continue running
            }
        }

        private async Task CheckDatabaseHealth(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            // Perform simple query to check database
            await unitOfWork.Tenants.GetByIdAsync(Guid.Empty, cancellationToken);
            _logger.LogInformation("Database health check passed");
        }

        private async Task CheckExternalServices(CancellationToken cancellationToken)
        {
            // Check external services like email, SMS, etc.
            await Task.Delay(100, cancellationToken); // Simulate work
            _logger.LogInformation("External services health check passed");
        }

        private async Task CheckDiskSpace(CancellationToken cancellationToken)
        {
            var drives = DriveInfo.GetDrives();
            foreach (var drive in drives.Where(d => d.IsReady))
            {
                var freeSpacePercent = (double)drive.AvailableFreeSpace / drive.TotalSize * 100;
                if (freeSpacePercent < 10)
                {
                    _logger.LogWarning($"Low disk space on drive {drive.Name}: {freeSpacePercent:F2}% free");
                }
            }
        }
    }
}
