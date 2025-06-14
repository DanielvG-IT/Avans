using Api.Interfaces;
using System.Text;
using System.Text.Json;

namespace Api.Services
{
    public class FastApiPredictionService : IFastApiPredictionService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FastApiPredictionService> _logger;
        private readonly string _baseUrl = "http://localhost:8000/api/v1/predictions";

        public FastApiPredictionService(HttpClient httpClient, ILogger<FastApiPredictionService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.BaseAddress = new Uri(_baseUrl);
        }

        public async Task<PredictionResponse?> MakeLitterAmountPredictionAsync(DateTime date) // TODO Figure out what data the AI model wants
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(new { date }), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/v1/predictions", content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<PredictionResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making POST request to {Endpoint}", "/api/v1/predictions");
                return null; // TODO Return null or handle the error as needed
            }
        }

        [Obsolete("Retraining the model is not supported in the current version. This method will be added in a future release.")]
        public async Task<bool> RetrainModelAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("/api/v1/predictions/retrain", null);
                response.EnsureSuccessStatusCode();
                return true; // TODO Return true or handle the success case as needed
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making POST request to {Endpoint}", "/api/v1/predictions/retrain");
                return false; // TODO Return false or handle the error as needed
            }
        }

        // TODO Implement other methods as needed
    }

    public class PredictionResponse // TODO Make final version with Wouter and move to the models folder
    {
        public required string Prediction { get; set; }
        public DateTime Date { get; set; }
    }
}