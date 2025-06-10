using Api.Interfaces;
using Api.Models;

namespace Api.Services
{
    public class AggregatedTrashService(HttpClient http, string apiKey) : IAggregatedTrashService
    {
        private readonly HttpClient _http = http;
        private readonly string _apiKey = apiKey;

         public async Task<List<AggregatedTrashDto>> GetAggregatedTrashAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "API URL GET REQUEST SENSORING");

        // Voeg de API Key toe aan de headers
        request.Headers.Add("API-Key", _apiKey);  

        var response = await _http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            // Je kunt hier eventueel logging of foutafhandeling toevoegen
            return [];
        }

        var content = await response.Content.ReadFromJsonAsync<List<AggregatedTrashDto>>();
        return content ?? [];
    }
    }
}