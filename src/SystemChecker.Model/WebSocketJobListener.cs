using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Quartz;

namespace SystemChecker.Model
{
    public class WebSocketJobListener : Quartz.IJobListener
    {
        public WebSocketJobListener() {
            WebSockets = new List<WebSocket>();
        }

        public List<WebSocket> WebSockets { get; set; }

        private async Task Broadcast(string message)
        {
            foreach (var skt in WebSockets)
            {
                var encoded = Encoding.UTF8.GetBytes(message);
                var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
                CancellationToken token = new CancellationToken();
                await skt.SendAsync(buffer, WebSocketMessageType.Text, true, token);
            }
        }

        public virtual string Name
        {
            get { return "WebSocket Listener"; }
        }

        public Task JobToBeExecuted(IJobExecutionContext context)
        {
            return Task.CompletedTask;
        }

        public Task JobExecutionVetoed(IJobExecutionContext context)
        {
            return Task.CompletedTask;
        }

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            if (jobException != null)
            {
                // Log/handle error here
                //await Broadcast($"Job Errored : {context.JobDetail.Description} - {jobException.ToString()}");
                string json = JsonConvert.SerializeObject(new
                {
                    State = -999, // do we need this?
                    CheckId = context.JobDetail.Key,
                    FireTime = context.FireTimeUtc,
                    NextFireTime = context.NextFireTimeUtc,
                    Result = context.Result
                }, Formatting.None);
                await Broadcast(json);
            }
            else
            {
                string json = JsonConvert.SerializeObject(new
                {
                    State = 0, // do we need this?
                    CheckId = context.JobDetail.Key,
                    FireTime = context.FireTimeUtc,
                    NextFireTime = context.NextFireTimeUtc,
                    Result = context.Result
                }, Formatting.None);
                await Broadcast(json);
            }
        }
    }
}
