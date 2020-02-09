using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Otomatik.BlazorBin.Function
{
    public static class SignalRHub
    {
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo GetSignalRInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous,
                "post",
                Route = null)] 
            HttpRequest req,
            ILogger log,
            [SignalRConnectionInfo(HubName = "component")]
            SignalRConnectionInfo connectionInfo)
        {
            log.LogInformation("HTTP trigger function processed a request to negotiate.");

            return connectionInfo;
        }

        [FunctionName("bin")]
        public static async Task<IActionResult> SendMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous,
                "get", "post", "put", "delete", "options", "head", "connect",
                Route = null)]
            HttpRequest req,
            ILogger log,
            [SignalR(HubName = "component")] 
            IAsyncCollector<SignalRMessage> signalRMessages)
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
                        headers,
                        requestBody
                    }
                });

            return new OkObjectResult($"{group} received your message");
        }

        [FunctionName("addToGroup")]
        public static async Task<IActionResult> AddToGroup(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post",
                Route = null)]
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

            return new OkObjectResult($"You are added to the {groupName}");
        }
    }
}
