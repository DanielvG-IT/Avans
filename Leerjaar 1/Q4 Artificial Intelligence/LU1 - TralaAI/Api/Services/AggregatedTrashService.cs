using Api.Interfaces;
using Api.Models;

namespace Api.Services
{
    public class AggregatedTrashService(HttpClient http) : IAggregatedTrashService
    {
        private readonly HttpClient _http = http;

        public async Task<List<AggregatedTrashDto>> GetAggregatedTrashAsync()
        {
            var response = await _http.GetFromJsonAsync<List<AggregatedTrashDto>>(
                "API URL GET REQUEST SENSORING");

            return response ?? [];
        }

    }
}