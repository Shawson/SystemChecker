using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DasMulli.Win32.ServiceUtils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using SystemChecker.Model.Data.Interfaces;
using SystemChecker.Model.Scheduling;

namespace SystemChecker.Model
{
    public class SystemCheckerRunner : IWin32Service
    {
        private static IScheduler _scheduler;
        private static IRepositoryFactory _repoFactory;
        private ILogger _logger;
        private CancellationToken _killToken;

        public SystemCheckerRunner(IRepositoryFactory repositoryFactory, ILogger logger, CancellationToken killToken)
        {
            _repoFactory = repositoryFactory;
            _logger = logger;
            _killToken = killToken;
        }

        public string ServiceName => "SystemChecker";
        public IScheduler Scheduler => _scheduler;

        public async void Start()
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            _scheduler = await schedFact.GetScheduler();

            // handle job failures
            _scheduler.ListenerManager.AddJobListener(new GlobalJobListener(_logger), GroupMatcher<JobKey>.AnyGroup());
            _scheduler.ListenerManager.AddSchedulerListener(new GlobalSchedulerListener(_logger));

            // Kids: don't share repositories!
            // Instead use the factory so each thread gets its own instance created with it's own connection etc
            _scheduler.Context.Add("RepositoryFactory", _repoFactory);
            _scheduler.Context.Add("Logger", _logger);

            // todo: add events for raising these kind of updates - log4net?
            _logger.LogInformation("Starting Scheduler");

            // check for changes to the job or schedules
            IJobDetail updatejob = JobBuilder.Create<ScheduleUpdater>()
                    .WithDescription("New Job Checker")
                    .WithIdentity("ScheduleUpdater", $"Updater")
                    .Build();

            ITrigger updateTrigger = TriggerBuilder.Create()
                    .WithIdentity($"TriggerScheduleUpdater", $"Updater")
                    .StartNow()
                    .WithCronSchedule("* 0/10 * * * ? *")  // check every 10 seconds for changes to the work list
                    .Build();

            await _scheduler.ScheduleJob(updatejob, updateTrigger);
            await _scheduler.Start();
            _logger.LogInformation($"Scheduler started");

            // trigger the schedule updater now to do the initial load of the work we're going to be running
            await _scheduler.TriggerJob(updatejob.Key);

            // todo: Add a signalR server which the web ui can connect to to request immediate re-runs of tests/ be notified of recent results
            //http://stackoverflow.com/questions/11140164/signalr-console-app-example
            var startup = new Startup(_scheduler);
            var host = new WebHostBuilder()
                        .UseKestrel()
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .ConfigureServices(services => services.AddSingleton<IStartup>(startup))
                        .Build();
            host.Run(_killToken);
        }

        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback)
        {
            Start();
        }

        public void Stop()
        {
            _scheduler.Shutdown(true);
        }
    }

    internal class GlobalSchedulerListener : ISchedulerListener
    {
        private ILogger _logger;

        public GlobalSchedulerListener(ILogger logger)
        {
            _logger = logger;
        }

        public Task JobAdded(IJobDetail jobDetail)
        {
            _logger.LogDebug($"Job added: {jobDetail.Key.Group} - {jobDetail.Key.Name}");
            return Task.CompletedTask;
        }

        public Task JobDeleted(JobKey jobKey)
        {
            _logger.LogDebug($"Job deleted: {jobKey.Group} - {jobKey.Name}");
            return Task.CompletedTask;
        }

        public Task JobPaused(JobKey jobKey)
        {
            _logger.LogDebug($"Job paused: {jobKey.Group} - {jobKey.Name}");
            return Task.CompletedTask;
        }

        public Task JobResumed(JobKey jobKey)
        {
            _logger.LogDebug($"Job resumed: {jobKey.Group} - {jobKey.Name}");
            return Task.CompletedTask;
        }

        public Task JobScheduled(ITrigger trigger)
        {
            _logger.LogDebug($"Job scheduled: {trigger.Key.Group} - {trigger.Key.Name}");
            return Task.CompletedTask;
        }

        public Task JobsPaused(string jobGroup)
        {
            _logger.LogDebug($"Jobs paused: {jobGroup}");
            return Task.CompletedTask;
        }

        public Task JobsResumed(string jobGroup)
        {
            _logger.LogDebug($"Jobs resumed: {jobGroup}");
            return Task.CompletedTask;
        }

        public Task JobUnscheduled(TriggerKey triggerKey)
        {
            _logger.LogDebug($"Job unscheduled: {triggerKey.Group} - {triggerKey.Name}");
            return Task.CompletedTask;
        }

        public Task SchedulerError(string msg, SchedulerException cause)
        {
            _logger.LogError($"Scheduler error: {msg}: {cause.Message}");
            return Task.CompletedTask;
        }

        public Task SchedulerInStandbyMode()
        {
            _logger.LogDebug("Scheduler standby");
            return Task.CompletedTask;
        }

        public Task SchedulerShutdown()
        {
            _logger.LogDebug("Scheduler shutdown");
            return Task.CompletedTask;
        }

        public Task SchedulerShuttingdown()
        {
            _logger.LogDebug("Scheduler shutting down");
            return Task.CompletedTask;
        }

        public Task SchedulerStarted()
        {
            _logger.LogDebug("Scheduler started");
            return Task.CompletedTask;
        }

        public Task SchedulerStarting()
        {
            _logger.LogDebug("Scheduler starting");
            return Task.CompletedTask;
        }

        public Task SchedulingDataCleared()
        {
            _logger.LogDebug("Scheduling data cleared");
            return Task.CompletedTask;
        }

        public Task TriggerFinalized(ITrigger trigger)
        {
            _logger.LogDebug($"Trigger finalized: {trigger.Key.Group} - {trigger.Key.Name}");
            return Task.CompletedTask;
        }

        public Task TriggerPaused(TriggerKey triggerKey)
        {
            _logger.LogDebug($"Trigger paused: {triggerKey.Group} - {triggerKey.Name}");
            return Task.CompletedTask;
        }

        public Task TriggerResumed(TriggerKey triggerKey)
        {
            _logger.LogDebug($"Trigger resumed: {triggerKey.Group} - {triggerKey.Name}");
            return Task.CompletedTask;
        }

        public Task TriggersPaused(string triggerGroup)
        {
            _logger.LogDebug($"Triggers paused: {triggerGroup}");
            return Task.CompletedTask;
        }

        public Task TriggersResumed(string triggerGroup)
        {
            _logger.LogDebug($"Triggers resumed: {triggerGroup}");
            return Task.CompletedTask;
        }
    }

    public class GlobalJobListener : Quartz.IJobListener
    {
        private ILogger _logger;
        public GlobalJobListener(ILogger logger)
        {
            _logger = logger;
        }

        public virtual string Name
        {
            get { return "MainJobListener"; }
        }

        public Task JobToBeExecuted(IJobExecutionContext context)
        {
            return Task.CompletedTask;
        }

        public Task JobExecutionVetoed(IJobExecutionContext context)
        {
            return Task.CompletedTask;
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            if (jobException != null)
            {
                // Log/handle error here
                _logger.LogError($"Job Errored : {context.JobDetail.Description} - {jobException.ToString()}");
            }
            else
            {
                _logger.LogInformation($"Job Executed : {context.JobDetail.Description} ({context.JobDetail.Key}) Result ({context.Result??"null"}) Next run at {context.NextFireTimeUtc}");
                
                
            }

            return Task.CompletedTask;
        }
    }
}
