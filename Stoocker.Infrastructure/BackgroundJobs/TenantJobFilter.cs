using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Stoocker.Infrastructure.BackgroundJobs
{
    public class TenantJobFilter : JobFilterAttribute, IClientFilter, IServerFilter
    {
        private const string TenantIdKey = "TenantId";
        private const string UserIdKey = "UserId";

        public void OnCreating(CreatingContext filterContext)
        {
            // Retrieve HttpContext from Items dictionary if available
            if (filterContext.Items.TryGetValue("HttpContext", out var context) && context is HttpContext httpContext)
            {
                if (httpContext?.User?.Identity?.IsAuthenticated == true)
                {
                    var tenantId = httpContext.User.FindFirst("TenantId")?.Value;
                    var userId = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                    if (!string.IsNullOrEmpty(tenantId))
                        filterContext.SetJobParameter(TenantIdKey, tenantId);

                    if (!string.IsNullOrEmpty(userId))
                        filterContext.SetJobParameter(UserIdKey, userId);
                }
            }
        }

        public void OnCreated(CreatedContext filterContext)
        {
            // Nothing to do
        }

        public void OnPerforming(PerformingContext filterContext)
        {
            var tenantId = filterContext.GetJobParameter<string>(TenantIdKey);
            var userId = filterContext.GetJobParameter<string>(UserIdKey);

            if (!string.IsNullOrEmpty(tenantId))
                Serilog.Context.LogContext.PushProperty("TenantId", tenantId);

            if (!string.IsNullOrEmpty(userId))
                Serilog.Context.LogContext.PushProperty("UserId", userId);
        }

        public void OnPerformed(PerformedContext filterContext)
        {
            
        }
    }
}
