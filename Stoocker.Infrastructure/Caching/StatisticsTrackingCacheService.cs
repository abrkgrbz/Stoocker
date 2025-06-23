using Stoocker.Application.Interfaces.Services.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Infrastructure.Caching
{
    public class StatisticsTrackingCacheService : ICacheService
    {
        private readonly ICacheService _innerCache;
        private readonly ICacheStatistics _statistics;

        public StatisticsTrackingCacheService(ICacheService innerCache, ICacheStatistics statistics)
        {
            _innerCache = innerCache;
            _statistics = statistics;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var result = await _innerCache.GetAsync<T>(key, cancellationToken);

            if (result != null)
                _statistics.RecordHit(key);
            else
                _statistics.RecordMiss(key);

            return result;
        }

        public async Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            var result = await _innerCache.GetAsync(key, cancellationToken);

            if (result != null)
                _statistics.RecordHit(key);
            else
                _statistics.RecordMiss(key);

            return result;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            await _innerCache.SetAsync(key, value, expiration, cancellationToken);
            _statistics.RecordSet(key, expiration ?? TimeSpan.FromMinutes(30));
        }

        public async Task SetAsync(string key, string value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            await _innerCache.SetAsync(key, value, expiration, cancellationToken);
            _statistics.RecordSet(key, expiration ?? TimeSpan.FromMinutes(30));
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            return _innerCache.RemoveAsync(key, cancellationToken);
        }

        public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            return _innerCache.RemoveByPatternAsync(pattern, cancellationToken);
        }

        public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            return _innerCache.ExistsAsync(key, cancellationToken);
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            var cached = await GetAsync<T>(key, cancellationToken);
            if (cached != null)
            {
                return cached;
            }

            var value = await factory();
            await SetAsync(key, value, expiration, cancellationToken);
            return value;
        }
    }
}
