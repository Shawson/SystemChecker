using MicroOrm.Pocos.SqlGenerator;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Data.SqlClient;
using SystemChecker.Model.Data;
using SystemChecker.Model.Data.Repositories;

namespace SystemChecker.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IHostingEnvironment env, Microsoft.Extensions.PlatformAbstractions.IApplicationEnvironment appEnv)
        {
            // Setup configuration sources.
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile($"{appEnv.ApplicationBasePath}\\config.json")
                .AddEnvironmentVariables();

            Configuration = configurationBuilder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var connectionString = Configuration[$"Data:DefaultConnection:ConnectionString-{Configuration["COMPUTERNAME"]}"];

            services.AddScoped<IDbConnection>((_) => new SqlConnection(connectionString));

            services.AddScoped<ISqlGenerator<CheckToPerform>, SqlGenerator<CheckToPerform>>();
            services.AddScoped<ISqlGenerator<CheckTrigger>, SqlGenerator<CheckTrigger>>();
            services.AddScoped<ISqlGenerator<CheckType>, SqlGenerator<CheckType>>();

            services.AddScoped<ICheckToPerformRepository, CheckToPerformRepository>();
            services.AddScoped<ICheckTriggerRepository, CheckTriggerRepository>();
            services.AddScoped<ICheckTypeRepository, CheckTypeRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseIISPlatformHandler();
            app.UseMvc();

            var options = new Microsoft.AspNet.StaticFiles.DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(options);

            app.UseStaticFiles();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
