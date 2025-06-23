using Stoocker.Application.Interfaces.Services.Caching.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Services.Caching
{
    public interface ICacheStatistics
    {
        void RecordHit(string key);
        void RecordMiss(string key);
        void RecordSet(string key, TimeSpan duration);
        void RecordEviction(string key);
        CacheStatisticsSnapshot GetSnapshot();
    }
}
