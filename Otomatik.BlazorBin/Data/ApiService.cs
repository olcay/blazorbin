using System.Net.Http;
using System.Threading.Tasks;

namespace Otomatik.BlazorBin.Data
{
    public class ApiService
    {
        public HttpClient _httpClient;

        public ApiService(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task AddToGroup(string groupName, string connectionId)
        {
            var response = await _httpClient.GetAsync($"api/addToGroup?group={groupName}&connectionId={connectionId}");
            response.EnsureSuccessStatusCode();
        }
    }
}
