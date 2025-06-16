using Api.Models;
using Api.Interfaces;
using System.Text;
using System.Text.Json;

namespace Api.Services
{
    public class FastApiPredictionService : IFastApiPredictionService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FastApiPredictionService> _logger;
        private readonly string _baseUrl = "http://localhost:8000";

        public FastApiPredictionService(HttpClient httpClient, ILogger<FastApiPredictionService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.BaseAddress = new Uri(_baseUrl);
        }

        public async Task<PredictionResponseWrapper> MakeLitterAmountPredictionAsync(List<PredictionRequestModel> requestModels)
        {
            try
            {
                var jsonRequest = JsonSerializer.Serialize(requestModels);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/predict", content);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var predictionResponse = JsonSerializer.Deserialize<PredictionResponseWrapper>(jsonResponse) ?? throw new Exception("Failed to deserialize prediction response.");
                return predictionResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making POST request to {Endpoint}", "/predict");
                throw;
            }
        }

        // [Obsolete("Retraining the model is not supported in the current version. This method will be added in a future release.")]
        // public async Task<bool> RetrainModelAsync()
        // {
        //     try
        //     {
        //         var response = await _httpClient.PostAsync("/retrain", null);
        //         response.EnsureSuccessStatusCode();
        //         return true; // TODO Return true or handle the success case as needed
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error making POST request to {Endpoint}", "/retrain");
        //         return false; // TODO Return false or handle the error as needed
        //     }
        // }
    }
}