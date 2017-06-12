using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace SystemChecker.Console
{
    public class Startup : IStartup
    {
        private IScheduler _scheduler;
        private WebSocketJobListener _scheduleListener;

        public Startup(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public void Configure(IApplicationBuilder app)
        {
            //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/websockets
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            app.UseWebSockets(webSocketOptions);
            _scheduleListener = new WebSocketJobListener();
            app.Use(async (context, next) =>
            {
                // add a listener?  would be better if this can be somehow passed in?
                //SystemCheckerRunner.sched.ListenerManager.AddSchedulerListener() 

                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        _scheduleListener.WebSockets.Add(webSocket);
                        await ListenForSocketClosureRequest(context, webSocket);
                    }
                    else
                    {
                        //context.Response.StatusCode = 400;
                        var jobGroups = await _scheduler.GetJobGroupNames();

                        foreach(var s in jobGroups)
                        {
                            await context.Response.WriteAsync($"{s}\r\n");
                        }
                        
                    }
                }
                else
                {
                    await next();
                }

            });

            _scheduler.ListenerManager.AddJobListener(_scheduleListener);

            var applicationLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            applicationLifetime.ApplicationStopping.Register(OnShutdown);
        }

        private void OnShutdown()
        {
            // close off the sockets
            foreach (var skt in _scheduleListener.WebSockets)
            {
                skt.CloseAsync(WebSocketCloseStatus.NormalClosure, "Application Shutdown", CancellationToken.None)
                    .RunSynchronously();
            }
            
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider();
        }

        private async Task ListenForSocketClosureRequest(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            _scheduleListener.WebSockets.Remove(webSocket);
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
