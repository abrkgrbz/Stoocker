using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Infrastructure.Monitoring
{
    public class DiskSpaceHealthCheck : IHealthCheck
    {
        private readonly long _minimumFreeBytesThreshold = 1_073_741_824; // 1 GB

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var drives = DriveInfo.GetDrives();
            var unhealthyDrives = new List<string>();

            foreach (var drive in drives.Where(d => d.IsReady))
            {
                if (drive.AvailableFreeSpace < _minimumFreeBytesThreshold)
                {
                    unhealthyDrives.Add($"{drive.Name} ({drive.AvailableFreeSpace / 1024 / 1024} MB free)");
                }
            }

            if (unhealthyDrives.Any())
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    $"Low disk space on drives: {string.Join(", ", unhealthyDrives)}"));
            }

            return Task.FromResult(HealthCheckResult.Healthy("Sufficient disk space available"));
        }
    }
}
