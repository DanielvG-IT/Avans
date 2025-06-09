using Microsoft.EntityFrameworkCore;
using TralaAI.CoreApi.Data;
using TralaAI.CoreApi.Interfaces;
using TralaAI.CoreApi.Models;
namespace TralaAI.CoreApi.Services
{
    public class AggregatedTrashService : IAggregatedTrashService
    {
        private readonly HttpClient _http;

        public AggregatedTrashService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<AggregatedTrashDto>> GetAggregatedTrashAsync()
        {
            var response = await _http.GetFromJsonAsync<List<AggregatedTrashDto>>(
                "API URL GET REQUEST SENSORING");

            return response ?? new List<AggregatedTrashDto>();
        }

    }
}