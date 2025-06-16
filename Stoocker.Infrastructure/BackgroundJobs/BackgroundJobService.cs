using Hangfire.States;
using Hangfire;
using Serilog;
using Stoocker.Application.Interfaces.Services.BackgrounJobs;

namespace Stoocker.Infrastructure.BackgroundJobs
{
    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly ILogger _logger;

        public BackgroundJobService(
            IBackgroundJobClient backgroundJobClient,
            IRecurringJobManager recurringJobManager)
        {
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
            _logger = Log.ForContext<BackgroundJobService>();
        }

        public string EnqueueJob<T>(System.Linq.Expressions.Expression<Action<T>> methodCall, string queue = "default")
        {
            try
            { 
                var jobId = _backgroundJobClient.Enqueue(methodCall);
                _logger.Information("Enqueued job {JobId} to queue {Queue}", jobId, queue);
                return jobId;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to enqueue job");
                throw;
            }
        }

        public string ScheduleJob<T>(System.Linq.Expressions.Expression<Action<T>> methodCall, TimeSpan delay, string queue = "default")
        {
            try
            {
                var jobId = _backgroundJobClient.Schedule(methodCall, delay);
                _logger.Information("Scheduled job {JobId} to run after {Delay} in queue {Queue}", jobId, delay, queue);
                return jobId;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to schedule job");
                throw;
            }
        }

        public void RecurringJob<T>(string recurringJobId, System.Linq.Expressions.Expression<Action<T>> methodCall, string cronExpression, string queue = "default")
        {
            try
            {
                _recurringJobManager.AddOrUpdate(recurringJobId, methodCall, cronExpression, new RecurringJobOptions
                { 
                    TimeZone = TimeZoneInfo.Local
                });
                _logger.Information("Created/Updated recurring job {JobId} with cron {CronExpression}", recurringJobId, cronExpression);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to create recurring job {JobId}", recurringJobId);
                throw;
            }
        }

        public void DeleteRecurringJob(string recurringJobId)
        {
            try
            {
                _recurringJobManager.RemoveIfExists(recurringJobId);
                _logger.Information("Deleted recurring job {JobId}", recurringJobId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete recurring job {JobId}", recurringJobId);
                throw;
            }
        }

        public bool DeleteJob(string jobId)
        {
            try
            {
                var result = _backgroundJobClient.Delete(jobId);
                _logger.Information("Deleted job {JobId} with result {Result}", jobId, result);
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete job {JobId}", jobId);
                return false;
            }
        }
    }
}
