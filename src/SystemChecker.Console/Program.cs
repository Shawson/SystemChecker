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
                    var hostCancellationToken = new CancellationTokenSource();

                    System.Console.CancelKeyPress += delegate
                    {
                        System.Console.WriteLine("Shut down requested");
                        // call methods to clean up
                        _killSwitch = true;
                        hostCancellationToken.Cancel(false);
                    };

                    svc.Start();

                    // at this point do we also add a tcp listener which allows us to trigger tests immediately?
                    // pass a reference of the actual service runner?
                    
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

                    logger.LogInformation("Shutting down..");
                    
                    svc.Stop();
                }
            }
            catch (Exception ex)
            {
                logger.LogCritical($"{DateTime.Now} : {ex.ToString()}");
            }

            System.Console.WriteLine("Shut down");
        }
    }
}