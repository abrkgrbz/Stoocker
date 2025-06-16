using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Services.BackgrounJobs
{
    public interface IBackgroundJobService
    {
        string EnqueueJob<T>(System.Linq.Expressions.Expression<Action<T>> methodCall, string queue = "default");
        string ScheduleJob<T>(System.Linq.Expressions.Expression<Action<T>> methodCall, TimeSpan delay, string queue = "default");
        void RecurringJob<T>(string recurringJobId, System.Linq.Expressions.Expression<Action<T>> methodCall, string cronExpression, string queue = "default");
        void DeleteRecurringJob(string recurringJobId);
        bool DeleteJob(string jobId);
    }
}
