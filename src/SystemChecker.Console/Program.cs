using System;
using System.Threading;
using SystemChecker.Model;
using SystemChecker.Model.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DasMulli.Win32.ServiceUtils;
using System.Linq;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace SystemChecker.Console
{
    public class Program
    {
        private static bool _killSwitch = false;
        public Program() { }

        public static void Main(string[] args)
        {
            if (args.Any(x => x == "--directory" || x == "-d"))
            {
                Directory.SetCurrentDirectory(PlatformServices.Default.Application.ApplicationBasePath); // otherwise ends up in system32 for windows service
            }

            var config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();

            var logSection = config.GetSection("Logging");

            ILoggerFactory factory = new LoggerFactory().AddConsole(logSection);
            if (args.Any(x => x == "--log" || x == "-l"))
            {
                factory.AddFile(logSection);
            }

            var logger = factory.CreateLogger("SystemCheckerRunner");

            logger.LogInformation("Starting");

            try
            {
#if (DEBUG)
                var connectionString = config[$"Data:DefaultConnection:ConnectionString-{config["COMPUTERNAME"]}"];
#else
                var connectionString = config[$"Data:DefaultConnection:ConnectionString"];
#endif

                var repoFactory = new DapperRepositoryFactory(connectionString);
                

                var svc = new SystemCheckerRunner(repoFactory, logger);

                if (args.Any(x => x == "--service" || x == "-s"))
                {
                    var serviceHost = new Win32ServiceHost(svc);
                    serviceHost.Run();
                }
                else
                {
                    System.Console.CancelKeyPress += delegate
                    {
                        System.Console.WriteLine("Shut down requested");
                        // call methods to clean up
                        _killSwitch = true;
                    };

                    svc.Start();

                    //SystemCheckerRunner.sched // access to the scheduler

                    // at this point do we also add a tcp listener which allows us to trigger tests immediately?

                    while (!_killSwitch)
                    {
                        Thread.Sleep(1000);
                    }

                    logger.LogInformation("Shutting down..");
                    svc.Stop();
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical($"{DateTime.Now} : {ex.ToString()}");
            }

            System.Console.WriteLine("Shut down- Press return");
            System.Console.ReadLine();
        }
    }
}