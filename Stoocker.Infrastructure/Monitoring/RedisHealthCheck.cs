using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Infrastructure.Monitoring
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisHealthCheck(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var database = _connectionMultiplexer.GetDatabase();
                await database.PingAsync();

                var info = await database.ExecuteAsync("INFO", "server");

                return HealthCheckResult.Healthy("Redis is healthy", new Dictionary<string, object>
                {
                    ["Status"] = _connectionMultiplexer.IsConnected ? "Connected" : "Disconnected",
                    ["Endpoints"] = string.Join(", ", _connectionMultiplexer.GetEndPoints().Select(e => e.ToString())),
                    ["Response"] = info.ToString().Split('\n').FirstOrDefault(l => l.StartsWith("redis_version")) ?? "Unknown"
                });
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Redis health check failed", ex, new Dictionary<string, object>
                {
                    ["Status"] = "Failed",
                    ["Error"] = ex.Message
                });
            }
        }
    }
}
