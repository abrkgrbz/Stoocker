using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Services.Monitoring
{
    public interface IMetricsService
    {
        void IncrementCounter(string name, string[] labels);
        void RecordGauge(string name, double value, string[] labels);
        void RecordHistogram(string name, double value, string[] labels);
    }
}
