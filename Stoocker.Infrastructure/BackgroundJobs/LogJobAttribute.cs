using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Serilog;

namespace Stoocker.Infrastructure.BackgroundJobs
{
    public class LogJobAttribute : JobFilterAttribute, IClientFilter, IServerFilter, IElectStateFilter
    {
        private static readonly ILogger Logger = Log.ForContext<LogJobAttribute>();

        public void OnCreating(CreatingContext filterContext)
        {
            Logger.Information("Creating job {JobType}.{Method} with parameters {@Arguments}",
                filterContext.Job.Type.Name,
                filterContext.Job.Method.Name,
                filterContext.Job.Args);
        }

        public void OnCreated(CreatedContext filterContext)
        {
            Logger.Information("Job created with ID {JobId}", filterContext.BackgroundJob?.Id);
        }

        public void OnPerforming(PerformingContext filterContext)
        {
            Logger.Information("Starting job {JobId} - {JobType}.{Method}",
                filterContext.BackgroundJob.Id,
                filterContext.BackgroundJob.Job.Type.Name,
                filterContext.BackgroundJob.Job.Method.Name);
        }

        public void OnPerformed(PerformedContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                Logger.Error(filterContext.Exception, "Job {JobId} failed", filterContext.BackgroundJob.Id);
            }
            else
            {
                Logger.Information("Job {JobId} completed successfully", filterContext.BackgroundJob.Id);
            }
        }

        public void OnStateElection(ElectStateContext context)
        {
            var failedState = context.CandidateState as FailedState;
            if (failedState != null)
            {
                Logger.Error(failedState.Exception, "Job {JobId} is moving to failed state", context.BackgroundJob.Id);
            }
        }
    }
}
