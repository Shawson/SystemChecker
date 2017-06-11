using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using SystemChecker.Model;

namespace SystemChecker.Console
{
    public class Startup 
    {
        public void Configure(IApplicationBuilder app)
        {
            //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/websockets
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };
            app.UseWebSockets(webSocketOptions);
            app.Use(async (context, next) =>
            {
                // add a listener?  would be better if this can be somehow passed in?
                //SystemCheckerRunner.sched.ListenerManager.AddSchedulerListener() 

                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await Echo(context, webSocket);
                    }
                    else
                    {
                        //context.Response.StatusCode = 400;
                        var jobGroups = await SystemCheckerRunner.sched.GetJobGroupNames();

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
            /*
            app.Run(context => {
                var jobs = SystemCheckerRunner.sched.GetCurrentlyExecutingJobs();
                
                return context.Response.WriteAsync(jobs.Result.Count.ToString());
            });
            */
        }

        private async Task Echo(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
