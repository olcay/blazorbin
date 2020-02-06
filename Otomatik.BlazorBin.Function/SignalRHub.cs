using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        public static Task SendMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            [SignalR(HubName = "component")]IAsyncCollector<SignalRMessage> signalRMessages)
        {
            string group = req.Query["group"];

            log.LogInformation($"HTTP trigger function processed a request for group {group}.");

            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    GroupName = group,
                    Target = "ReceiveMessage",
                    Arguments = new [] { "AzureFunction", "hello!" }
                });
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
