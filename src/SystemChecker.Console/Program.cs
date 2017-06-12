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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

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
                
                var svc = new SystemCheckerRunner(repoFactory, logger);

                if (args.Any(x => x == "--service" || x == "-s"))
                {
                    var serviceHost = new Win32ServiceHost(svc);
                    serviceHost.Run();
                }
                else
                {
                    var hostCancellationToken = new CancellationTokenSource();

                    System.Console.CancelKeyPress += delegate
                    {
                        System.Console.WriteLine("Shut down requested");
                        logger.LogInformation("Shutting down http..");

                        hostCancellationToken.Cancel(false);
                    };

                    // Start the windows service

                    svc.Start();

                    // Start http service for realtime updates via web sockets
                    
                    var startup = new Startup(svc.Scheduler);

                    var host = new WebHostBuilder()
                        .UseKestrel()
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .ConfigureServices(services => services.AddSingleton<IStartup>(startup))
                        .Build();
                    host.Run(hostCancellationToken.Token);

                    /*
                     * web host process now holds the thread open
                    while (!_killSwitch)
                    {
                        Thread.Sleep(1000);
                    }
                    */

                    logger.LogInformation("Http shutdown");
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