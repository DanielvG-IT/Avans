using Api.Models;
using Api.Interfaces;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Api.Services
{
    public class FastApiPredictionService(HttpClient httpClient, ILogger<FastApiPredictionService> logger) : IFastApiPredictionService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<FastApiPredictionService> _logger = logger;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public async Task<List<PredictionResponse>> MakeLitterAmountPredictionAsync(PredictionRequest requestModels)
        {
            try
            {
                var jsonRequest = JsonSerializer.Serialize(requestModels, _jsonOptions);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/predict", content);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    var retrainResponse = await RetrainModelAsync(requestModels.CameraId);
                    if (!retrainResponse)
                    {
                        _logger.LogError("Failed to retrain model for camera ID: {CameraId}", requestModels.CameraId);
                        throw new Exception("Failed to retrain model.");
                    }
                }

                response.EnsureSuccessStatusCode();

                await using var responseStream = await response.Content.ReadAsStreamAsync();
                var fastapiresponse = await JsonSerializer.DeserializeAsync<List<PredictionResponse>>(responseStream, _jsonOptions) ?? throw new Exception("Failed to deserialize prediction response.");
                return fastapiresponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making POST request to {Endpoint}", "/predict");
                throw;
            }
        }

        public async Task<bool> RetrainModelAsync(int cameraId)
        {
            try
            {
                var request = new RetrainRequest { CameraLocation = cameraId };
                var jsonRequest = JsonSerializer.Serialize(request, _jsonOptions);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/retrain", content);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making POST request to {Endpoint}", "/retrain");
                return false;
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