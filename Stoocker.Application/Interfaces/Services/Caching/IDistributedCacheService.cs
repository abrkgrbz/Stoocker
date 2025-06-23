using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Services.Caching
{
    public interface IDistributedCacheService : ICacheService
    {
        Task<long> IncrementAsync(string key, long value = 1, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
        Task<bool> LockAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default);
        Task<bool> UnlockAsync(string key, CancellationToken cancellationToken = default);
    }
}
