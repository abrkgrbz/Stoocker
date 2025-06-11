using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Infrastructure.Monitoring
{
    public class MemoryHealthCheck : IHealthCheck
    {
        private readonly long _minimumFreeMemoryThreshold = 104_857_600; // 100 MB

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var gcInfo = GC.GetGCMemoryInfo();
            var availableMemory = gcInfo.TotalAvailableMemoryBytes;
            var usedMemory = GC.GetTotalMemory(false);

            if (availableMemory < _minimumFreeMemoryThreshold)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    $"Low memory: {availableMemory / 1024 / 1024} MB available"));
            }

            var data = new Dictionary<string, object>
            {
                ["TotalMemory"] = $"{gcInfo.TotalAvailableMemoryBytes / 1024 / 1024} MB",
                ["UsedMemory"] = $"{usedMemory / 1024 / 1024} MB",
                ["Gen0Collections"] = GC.CollectionCount(0),
                ["Gen1Collections"] = GC.CollectionCount(1),
                ["Gen2Collections"] = GC.CollectionCount(2)
            };

            return Task.FromResult(HealthCheckResult.Healthy("Memory usage is healthy", data));
        }
    }
}
