using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Stoocker.Application.Interfaces.Services.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Infrastructure.Caching
{
    public static class CacheConfiguration
    {
        public static IServiceCollection AddCachingServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Cache statistics
            services.AddSingleton<ICacheStatistics, CacheStatistics>();

            // In-Memory Cache
            services.AddMemoryCache(options =>
            {
                var sizeLimitMB = configuration.GetValue<int>("CacheSettings:MemoryCacheSizeLimitMB", 1024);
                options.SizeLimit = sizeLimitMB * 1024 * 1024; // MB to bytes
                options.CompactionPercentage = 0.25;
            });

            // Redis Cache
            var redisConnection = configuration.GetConnectionString("RedisConnection");
            var enableRedis = configuration.GetValue<bool>("CacheSettings:EnableRedisCache", true);

            if (!string.IsNullOrEmpty(redisConnection) && enableRedis)
            {
                services.AddSingleton<IConnectionMultiplexer>(sp =>
                {
                    var configurationOptions = ConfigurationOptions.Parse(redisConnection);
                    configurationOptions.AbortOnConnectFail = false;
                    configurationOptions.ConnectTimeout = 5000;
                    configurationOptions.SyncTimeout = 5000;
                    configurationOptions.AsyncTimeout = 5000;
                    configurationOptions.ConnectRetry = 3;
                    configurationOptions.ReconnectRetryPolicy = new LinearRetry(5000);

                    return ConnectionMultiplexer.Connect(configurationOptions);
                });

                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnection;
                    options.InstanceName = "Stoocker:";
                });

                services.AddScoped<IDistributedCacheService, RedisCacheService>();
            }

            services.AddScoped<IMemoryCacheService, MemoryCacheService>();

            // Default cache service with statistics tracking
            services.AddScoped<ICacheService>(provider =>
            {
                ICacheService baseCache;

                if (!string.IsNullOrEmpty(redisConnection) && enableRedis)
                {
                    baseCache = provider.GetRequiredService<IDistributedCacheService>();
                }
                else
                {
                    baseCache = provider.GetRequiredService<IMemoryCacheService>();
                }

                var statistics = provider.GetRequiredService<ICacheStatistics>();
                return new StatisticsTrackingCacheService(baseCache, statistics);
            });

            return services;
        }
    }
}
