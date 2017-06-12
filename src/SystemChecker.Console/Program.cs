using System;
using System.IO;
using System.Linq;
using System.Threading;
using DasMulli.Win32.ServiceUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using SystemChecker.Model;
using SystemChecker.Model.Data;

namespace SystemChecker.Console
{
    public class Program
    {
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
                var hostCancellationToken = new CancellationTokenSource();
                var svc = new SystemCheckerRunner(repoFactory, logger, hostCancellationToken.Token);

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
                        logger.LogInformation("Shutting down http..");

                        hostCancellationToken.Cancel(false);
                    };

                    svc.Start();

                    // thread kept open here by web host

                    logger.LogInformation("Shutting down service..");
                    
                    svc.Stop();

                    logger.LogInformation("Service shutdown");
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical($"{DateTime.Now} : {ex.ToString()}");
            }

            System.Console.WriteLine("Shut down complete");
        }
    }
}