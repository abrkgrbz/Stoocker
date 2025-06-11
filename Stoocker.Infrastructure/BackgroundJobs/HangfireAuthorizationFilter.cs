using Hangfire.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Infrastructure.BackgroundJobs
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Check if user is authenticated
            if (!httpContext.User.Identity?.IsAuthenticated ?? true)
                return false;

            // Check if user has admin role
            return httpContext.User.IsInRole("Admin") || httpContext.User.IsInRole("SuperAdmin");
        }
    }
}
