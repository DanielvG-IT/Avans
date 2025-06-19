using Api.Models;
using Api.Interfaces;
using System.Text;
using System.Text.Json;

namespace Api.Services
{
    public class FastApiPredictionService(HttpClient httpClient, ILogger<FastApiPredictionService> logger) : IFastApiPredictionService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<FastApiPredictionService> _logger = logger;

        public async Task<PredictionResponse> MakeLitterAmountPredictionAsync(PredictionRequest requestModels)
        {
            try
            {
                var jsonRequest = JsonSerializer.Serialize(requestModels);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/predict", content);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var predictionResponse = JsonSerializer.Deserialize<PredictionResponse>(jsonResponse) ?? throw new Exception("Failed to deserialize prediction response.");
                return predictionResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making POST request to {Endpoint}", "/predict");
                throw;
            }
        }

        public async Task<bool> RetrainModelAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("/retrain", null);
                response.EnsureSuccessStatusCode();
                return true; // TODO Return true or handle the success case as needed
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making POST request to {Endpoint}", "/retrain");
                return false; // TODO Return false or handle the error as needed
            }
        }

        public async Task<bool> GetStatusAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("/status", null);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Request to {Endpoint} timed out", "/status");
                return false; // Explicitly handle timeout as a failure
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making POST request to {Endpoint}", "/status");
                return false;
            }
        }
    }
}