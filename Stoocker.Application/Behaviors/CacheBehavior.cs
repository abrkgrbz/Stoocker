using MediatR;
using Microsoft.Extensions.Logging;
using Stoocker.Application.Interfaces.Services.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Behaviors
{
    public class CacheBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, ICacheableQuery
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<CacheBehavior<TRequest, TResponse>> _logger;

        public CacheBehavior(ICacheService cacheService, ILogger<CacheBehavior<TRequest, TResponse>> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!request.BypassCache)
            {
                var cacheKey = request.CacheKey;
                var cachedResponse = await _cacheService.GetAsync<TResponse>(cacheKey, cancellationToken);

                if (cachedResponse != null)
                {
                    _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
                    return cachedResponse;
                }
            }

            _logger.LogInformation("Cache miss for key: {CacheKey}", request.CacheKey);

            var response = await next();

            if (response != null)
            {
                await _cacheService.SetAsync(request.CacheKey, response, request.CacheDuration, cancellationToken);
                _logger.LogInformation("Cached response for key: {CacheKey} with duration: {Duration}",
                    request.CacheKey, request.CacheDuration);
            }

            return response;
        }
    }
}
