using Stoocker.Application.Interfaces.Services.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prometheus;

namespace Stoocker.Infrastructure.Monitoring
{

    public class MetricsService : IMetricsService
    {
        private readonly Dictionary<string, Counter> _counters = new();
        private readonly Dictionary<string, Gauge> _gauges = new();
        private readonly Dictionary<string, Histogram> _histograms = new();

        public MetricsService()
        {
            // Initialize common metrics
            _counters["http_requests_total"] = Metrics.CreateCounter(
                "stoocker_http_requests_total",
                "Total HTTP requests",
                new[] { "method", "endpoint", "status" });

            _counters["background_jobs_total"] = Metrics.CreateCounter(
                "stoocker_background_jobs_total",
                "Total background jobs",
                new[] { "job_type", "status" });

            _gauges["active_users"] = Metrics.CreateGauge(
                "stoocker_active_users",
                "Number of active users",
                new[] { "tenant" });

            _histograms["request_duration_seconds"] = Metrics.CreateHistogram(
                "stoocker_request_duration_seconds",
                "HTTP request duration in seconds",
                new[] { "method", "endpoint" });
        }

        public void IncrementCounter(string name, string[] labels)
        {
            if (_counters.TryGetValue(name, out var counter))
            {
                counter.WithLabels(labels).Inc();
            }
        }

        public void RecordGauge(string name, double value, string[] labels)
        {
            if (_gauges.TryGetValue(name, out var gauge))
            {
                gauge.WithLabels(labels).Set(value);
            }
        }

        public void RecordHistogram(string name, double value, string[] labels)
        {
            if (_histograms.TryGetValue(name, out var histogram))
            {
                histogram.WithLabels(labels).Observe(value);
            }
        }
    }
}
