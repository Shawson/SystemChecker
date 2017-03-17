using System;
using System.Threading;
using SystemChecker.Model;
using SystemChecker.Model.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SystemChecker.Console
{
    public class Program
    {
        private static bool _killSwitch = false;
        public Program() { }

        public static void Main(string[] args)
        {
            System.Console.CancelKeyPress += delegate
            {
                System.Console.WriteLine("Shut down requested");
                // call methods to clean up
                _killSwitch = true;
            };

            var config = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .Build();

            ILoggerFactory factory = new LoggerFactory().AddConsole(config.GetSection("Logging"));
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
                svc.Start();

                // at this point do we also add a tcp listener which allows us to trigger tests immediately?

                while (!_killSwitch)
                {
                    Thread.Sleep(1000);
                }

                logger.LogInformation("Shutting down..");
                svc.Stop();

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