using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Stoocker.Application.Interfaces.Services.BackgrounJobs
{
    public interface IRecurringJobService
    {
        void AddOrUpdate<T>(string recurringJobId, Expression<Func<T, Task>> methodCall, string cronExpression) where T : class;
        void AddOrUpdate<T>(string recurringJobId, Expression<Func<T, Task>> methodCall, string cronExpression, TimeZoneInfo timeZone) where T : class;
        void RemoveIfExists(string recurringJobId);
        void Trigger(string recurringJobId);
    }
}
