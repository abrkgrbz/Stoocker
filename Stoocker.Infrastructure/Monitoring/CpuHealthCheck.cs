using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Infrastructure.Monitoring
{
    public class CpuHealthCheck : IHealthCheck
    {
        private readonly double _maximumCpuThreshold = 80.0; // 80%

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var startTime = DateTime.UtcNow;
            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

            await Task.Delay(100, cancellationToken);

            var endTime = DateTime.UtcNow;
            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            var cpuUsagePercentage = cpuUsageTotal * 100;

            if (cpuUsagePercentage > _maximumCpuThreshold)
            {
                return HealthCheckResult.Degraded(
                    $"High CPU usage: {cpuUsagePercentage:F2}%");
            }

            var data = new Dictionary<string, object>
            {
                ["CpuUsage"] = $"{cpuUsagePercentage:F2}%",
                ["ProcessorCount"] = Environment.ProcessorCount
            };

            return HealthCheckResult.Healthy("CPU usage is healthy", data);
        }
    }

}
