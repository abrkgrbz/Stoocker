using Stoocker.Application.Interfaces.Services.Caching;
using Stoocker.Application.Interfaces.Services.Caching.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Infrastructure.Caching
{
    public class CacheStatistics : ICacheStatistics
    {
        private long _hits;
        private long _misses;
        private long _sets;
        private long _evictions;
        private readonly ConcurrentDictionary<string, long> _keyHits;
        private readonly ConcurrentDictionary<string, long> _keyMisses;

        public CacheStatistics()
        {
            _keyHits = new ConcurrentDictionary<string, long>();
            _keyMisses = new ConcurrentDictionary<string, long>();
        }

        public void RecordHit(string key)
        {
            Interlocked.Increment(ref _hits);
            _keyHits.AddOrUpdate(key, 1, (k, v) => v + 1);
        }

        public void RecordMiss(string key)
        {
            Interlocked.Increment(ref _misses);
            _keyMisses.AddOrUpdate(key, 1, (k, v) => v + 1);
        }

        public void RecordSet(string key, TimeSpan duration)
        {
            Interlocked.Increment(ref _sets);
        }

        public void RecordEviction(string key)
        {
            Interlocked.Increment(ref _evictions);
        }

        public CacheStatisticsSnapshot GetSnapshot()
        {
            var totalRequests = _hits + _misses;
            var hitRate = totalRequests > 0 ? (double)_hits / totalRequests * 100 : 0;

            return new CacheStatisticsSnapshot
            {
                TotalHits = _hits,
                TotalMisses = _misses,
                TotalSets = _sets,
                TotalEvictions = _evictions,
                HitRate = hitRate,
                TopHitKeys = _keyHits.OrderByDescending(x => x.Value).Take(10).ToList(),
                TopMissKeys = _keyMisses.OrderByDescending(x => x.Value).Take(10).ToList()
            };
        }
    }
}