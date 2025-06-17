using Microsoft.Extensions.Options;
using Api.Interfaces;
using Api.Models;

namespace Api.Services
{
    public class AggregatedTrashService(HttpClient httpClient) : IAggregatedTrashService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<List<AggregatedTrashDto>> GetAggregatedTrashAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "API URL GET REQUEST SENSORING");
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                // TODO Je kunt hier eventueel logging of foutafhandeling toevoegen
                return [];
            }

            var content = await response.Content.ReadFromJsonAsync<List<AggregatedTrashDto>>();
            return content ?? [];
        }
    }
}