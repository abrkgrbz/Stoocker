using Microsoft.Extensions.Logging;
using Stoocker.Application.Interfaces.Services.Logger;
using Stoocker.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Infrastructure.Logging
{
    public class AuditLogger : IAuditLogger
    {
        private readonly ILogger<AuditLogger> _logger;
        private readonly ICurrentUserService _currentUserService;

        public AuditLogger(ILogger<AuditLogger> logger, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task LogAuditAsync(string action, string entityName, object entityId, object? oldValues = null, object? newValues = null)
        {
            var auditLog = new
            {
                Timestamp = DateTime.UtcNow,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                UserId = _currentUserService.UserId,
                UserEmail = _currentUserService.Email,
                TenantId = _currentUserService.TenantId,
                OldValues = oldValues,
                NewValues = newValues,
                IpAddress = GetClientIpAddress(),
                UserAgent = GetUserAgent()
            };

            _logger.LogInformation("Audit: {Action} on {EntityName} by {UserEmail}", action, entityName, _currentUserService.Email ?? "Unknown");

            await Task.CompletedTask;
        }

        private string GetClientIpAddress()
        {
            // Implementation would get IP from HttpContext
            return "0.0.0.0";
        }

        private string GetUserAgent()
        {
            // Implementation would get User-Agent from HttpContext
            return "Unknown";
        }
    }
}
