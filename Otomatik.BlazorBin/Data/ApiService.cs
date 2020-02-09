using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Otomatik.BlazorBin.Data
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient client)
        {
            _httpClient = client;
        }

        public string GetAddress()
        {
            return _httpClient.BaseAddress.ToString();
        }

        public string GetKey()
        {
            return _httpClient.DefaultRequestHeaders.GetValues("x-functions-key").FirstOrDefault();
        }

        public async Task AddToGroup(string groupName, string connectionId)
        {
            var json = JsonConvert.SerializeObject(new { GroupName = groupName, ConnectionId = connectionId });
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/addToGroup", data);
            response.EnsureSuccessStatusCode();
        }
    }
}
