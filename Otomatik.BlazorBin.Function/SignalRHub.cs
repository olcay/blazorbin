using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace Otomatik.BlazorBin.Function
{
    public static class SignalRHub
    {
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo GetSignalRInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "component")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

        [FunctionName("bin")]
        public static async Task<IActionResult> SendMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]
            HttpRequest req,
            ILogger log,
            [SignalR(HubName = "component")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            string group = req.Query["group"];

            log.LogInformation($"HTTP trigger function processed a request for group {group}.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var headers = req.Headers.Select(h => new KeyValuePair<string, string>(h.Key, h.Value.ToString())).ToList();

            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    GroupName = group,
                    Target = "ReceiveMessage",
                    Arguments = new object[]
                    {
                        req.Method,
                        req.GetDisplayUrl(),
                        headers,
                        requestBody
                    }
                });

            return group != null
                ? (ActionResult) new OkObjectResult($"{group} received your message")
                : new BadRequestObjectResult("Please pass a group name on the query string");
        }

        [FunctionName("addToGroup")]
        public static Task AddToGroup(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequest req,
            [SignalR(HubName = "component")]
            IAsyncCollector<SignalRGroupAction> signalRGroupActions)
        {
            string group = req.Query["group"];
            string connectionId = req.Query["connectionId"];

            return signalRGroupActions.AddAsync(
                new SignalRGroupAction
                {
                    ConnectionId = connectionId,
                    GroupName = group,
                    Action = GroupAction.Add
                });
        }
    }
}
