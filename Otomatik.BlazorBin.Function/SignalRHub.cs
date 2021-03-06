using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Otomatik.BlazorBin.Common;

namespace Otomatik.BlazorBin.Function
{
    public static class SignalRHub
    {
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo GetSignalRInfo(
            [HttpTrigger(AuthorizationLevel.Admin,
                "post",
                Route = "api/negotiate")]
            HttpRequest req,
            ILogger log,
            [SignalRConnectionInfo(HubName = "component")]
            SignalRConnectionInfo connectionInfo)
        {
            log.LogInformation("HTTP trigger function processed a request to negotiate.");

            return connectionInfo;
        }

        [FunctionName("sendToGroup")]
        public static async Task<IActionResult> SendToGroup(
            [HttpTrigger(AuthorizationLevel.Anonymous,
                "get", "post", "put", "delete", "options", "patch", "head", "connect",
                Route = "{group}")]
            HttpRequest req,
            string group,
            ILogger log,
            [SignalR(HubName = "component")] IAsyncCollector<SignalRMessage> signalRMessages,
            CancellationToken cancellationToken)
        {
            log.LogInformation($"HTTP trigger function processed a request for group {group}.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var dangerousHeaders = new List<string>
            {
                "Max-Forwards", "X-WAWS-Unencoded-URL", "CLIENT-IP", "X-ARR-LOG-ID", "DISGUISED-HOST",
                "X-SITE-DEPLOYMENT-ID", "WAS-DEFAULT-HOSTNAME", "X-Original-URL", "X-ARR-SSL", "X-AppService-Proto"
            };

            var headers = req.Headers
                .Where(h => !dangerousHeaders.Contains(h.Key))
                .Select(h => new KeyValuePair<string, string>(h.Key, h.Value.ToString()))
                .ToList();

            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    GroupName = group,
                    Target = "ReceiveMessage",
                    Arguments = new object[]
                    {
                        new Request
                        {
                            Method = req.Method,
                            Headers = headers,
                            Body = requestBody,
                            ReceivedOn = DateTime.UtcNow,
                            QueryString = req.QueryString.Value
                        }
                    }
                }, cancellationToken);

            return new OkObjectResult("{\"success\":true}");
        }

        [FunctionName("addToGroup")]
        public static async Task<IActionResult> AddToGroup(
            [HttpTrigger(AuthorizationLevel.Admin, "post",
                Route = "api/addToGroup")]
            HttpRequest req,
            ILogger log,
            [SignalR(HubName = "component")]
            IAsyncCollector<SignalRGroupAction> signalRGroupActions)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string groupName = data?.GroupName;
            string connectionId = data?.ConnectionId;

            log.LogInformation($"HTTP trigger function processed a request to add to a group {groupName}.");

            await signalRGroupActions.AddAsync(
                new SignalRGroupAction
                {
                    ConnectionId = connectionId,
                    GroupName = groupName,
                    Action = GroupAction.Add
                });

            return new OkObjectResult("{\"success\":true}");
        }
    }
}
