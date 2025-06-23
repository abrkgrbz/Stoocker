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
    public class CacheInvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, ICacheInvalidatorCommand
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<CacheInvalidationBehavior<TRequest, TResponse>> _logger;

        public CacheInvalidationBehavior(ICacheService cacheService, ILogger<CacheInvalidationBehavior<TRequest, TResponse>> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next();

            if (request.CacheKeysToInvalidate != null)
            {
                foreach (var key in request.CacheKeysToInvalidate)
                {
                    await _cacheService.RemoveAsync(key, cancellationToken);
                    _logger.LogInformation("Invalidated cache key: {CacheKey}", key);
                }
            }

            if (request.CachePatternsToInvalidate != null)
            {
                foreach (var pattern in request.CachePatternsToInvalidate)
                {
                    await _cacheService.RemoveByPatternAsync(pattern, cancellationToken);
                    _logger.LogInformation("Invalidated cache pattern: {Pattern}", pattern);
                }
            }

            return response;
        }
    }
}
