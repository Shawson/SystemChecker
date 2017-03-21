using Quartz;
using System;
using SystemChecker.Model.Data.Interfaces;
using SystemChecker.Model.Checkers.Serialisation;
using System.Diagnostics;
using System.Threading.Tasks;
using SystemChecker.Model.Interfaces;
using SystemChecker.Model.Data.Repositories;
using Microsoft.Extensions.Logging;
using SystemChecker.Model.Data;
using Newtonsoft.Json;

namespace SystemChecker.Model.Scheduling
{
    public class ScheduledCheckRunner : IJob
    {
        public int CheckToPerformId { get; set; }

        private bool _firstRun = true;
        private ISystemCheck _checker;
        private ICheckResultRepository _checkResultRepo;

        public Task Execute(IJobExecutionContext context)
        {
            try {

                if (_firstRun)
                {
                    var repoFactory = context.Scheduler.Context["RepositoryFactory"] as IRepositoryFactory;
                    var checkToPerformRepo = repoFactory.GetCheckToPerformRepository();
                    _checkResultRepo = repoFactory.GetCheckResultRepository();

                    var check = checkToPerformRepo.GetById(CheckToPerformId);

                    _checker = CheckUnpacker.Unpack(check, repoFactory);
                }

                var logger = context.Scheduler.Context["Logger"] as ILogger;

                var stopWatch = new Stopwatch();

                var startTime = DateTime.Now;
                stopWatch.Start();
                CheckResult result = null;
                Exception exception = null;
                try
                {
                    result = _checker.PerformCheck(_checkResultRepo, logger);
                }
                catch (Exception e)
                {
                    exception = e;
                    result = new CheckResult
                    {
                        FailureDetail = e.ToString(),
                        Result = -99,
                        RunData = JsonConvert.SerializeObject(new
                        {
                            Exception = e.Message,
                            StackTrace = e.StackTrace
                        })
                    };
                }
                stopWatch.Stop();
                
                result.DurationMS = (int)stopWatch.ElapsedMilliseconds;
                result.CheckId = CheckToPerformId; // check.CheckId;
                result.CheckDTS = startTime;
                
                _checkResultRepo.Insert(result);

                stopWatch = null;
                result = null;
                GC.Collect();

                if (exception != null)
                {
                    throw exception;
                }

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                JobExecutionException e2 =
                    new JobExecutionException(e);

                throw e2;
            }
        }
    }
}