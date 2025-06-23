using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using Stoocker.Application.Interfaces.Services.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Stoocker.Infrastructure.Caching
{
    public class RedisCacheService : IDistributedCacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IDatabase _database;

        public RedisCacheService(IDistributedCache distributedCache, IConnectionMultiplexer connectionMultiplexer)
        {
            _distributedCache = distributedCache;
            _connectionMultiplexer = connectionMultiplexer;
            _database = _connectionMultiplexer.GetDatabase();
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var value = await _distributedCache.GetStringAsync(key, cancellationToken);
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(value);
        }

        public async Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _distributedCache.GetStringAsync(key, cancellationToken);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null,
            CancellationToken cancellationToken = default)
        {
            var options = new DistributedCacheEntryOptions();

            if (expiration.HasValue)
            {
                options.SetAbsoluteExpiration(expiration.Value);
            }
            else
            {
                options.SetSlidingExpiration(TimeSpan.FromMinutes(30)); // Default sliding expiration
            }

            var serialized = JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(key, serialized, options, cancellationToken);
        }

        public async Task SetAsync(string key, string value, TimeSpan? expiration = null,
            CancellationToken cancellationToken = default)
        {
            var options = new DistributedCacheEntryOptions();

            if (expiration.HasValue)
            {
                options.SetAbsoluteExpiration(expiration.Value);
            }
            else
            {
                options.SetSlidingExpiration(TimeSpan.FromMinutes(30));
            }

            await _distributedCache.SetStringAsync(key, value, options, cancellationToken);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }

        public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: $"*{pattern}*").ToArray();

            if (keys.Any())
            {
                await _database.KeyDeleteAsync(keys);
            }
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _database.KeyExistsAsync(key);
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null,
            CancellationToken cancellationToken = default)
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

        public async Task<long> IncrementAsync(string key, long value = 1, TimeSpan? expiration = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _database.StringIncrementAsync(key, value);

            if (expiration.HasValue)
            {
                await _database.KeyExpireAsync(key, expiration.Value);
            }

            return result;
        }

        public async Task<bool> LockAsync(string key, TimeSpan expiration,
            CancellationToken cancellationToken = default)
        {
            return await _database.StringSetAsync($"lock:{key}", "1", expiration, When.NotExists);
        }

        public async Task<bool> UnlockAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _database.KeyDeleteAsync($"lock:{key}");
        }
    }
}