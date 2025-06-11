using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Stoocker.Application.Interfaces.Services;
using Stoocker.Application.Interfaces.Services.Logger;

namespace Stoocker.Infrastructure.Logging
{
    public class TenantAwareLogger : ITenantAwareLogger
    {
        private readonly ILogger<TenantAwareLogger> _logger;
        private readonly ICurrentUserService _currentUserService;

        public TenantAwareLogger(ILogger<TenantAwareLogger> logger, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
        }

        private void EnrichWithTenantInfo(Action<ILogger> logAction)
        {
            using (_logger.BeginScope(new Dictionary<string, object>
                   {
                       ["TenantId"] = _currentUserService.TenantId?.ToString() ?? "System",
                       ["UserId"] = _currentUserService.UserId?.ToString() ?? "Anonymous",
                       ["UserEmail"] = _currentUserService.Email ?? "Unknown"
                   }))
            {
                logAction(_logger);
            }
        }

        public void LogInformation(string message, params object[] args)
        {
            EnrichWithTenantInfo(logger => logger.LogInformation(message, args));
        }

        public void LogWarning(string message, params object[] args)
        {
            EnrichWithTenantInfo(logger => logger.LogWarning(message, args));
        }

        public void LogError(Exception exception, string message, params object[] args)
        {
            EnrichWithTenantInfo(logger => logger.LogError(exception, message, args));
        }

        public void LogDebug(string message, params object[] args)
        {
            EnrichWithTenantInfo(logger => logger.LogDebug(message, args));
        }

        public void LogTrace(string message, params object[] args)
        {
            EnrichWithTenantInfo(logger => logger.LogTrace(message, args));
        }
    }

}
