﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Stoocker.Application.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var requestGuid = Guid.NewGuid().ToString();

            _logger.LogInformation("Handling {RequestName} ({RequestGuid})", requestName, requestGuid);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await next();

                stopwatch.Stop();

                _logger.LogInformation("Handled {RequestName} ({RequestGuid}) in {ElapsedMilliseconds}ms",
                    requestName, requestGuid, stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex, "Error handling {RequestName} ({RequestGuid}) after {ElapsedMilliseconds}ms",
                    requestName, requestGuid, stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
}
