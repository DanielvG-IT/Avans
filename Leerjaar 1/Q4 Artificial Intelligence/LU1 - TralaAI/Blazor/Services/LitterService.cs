using Blazor.Models;
using Blazor.Interfaces;

namespace Blazor.Services;

public class LitterService(HttpClient httpClient) : ILitterService
{
  private readonly HttpClient _httpClient = httpClient;

  public async Task<List<Litter>?> GetLittersAsync(LitterFilterDto? filter)
  {
    var queryString = "";
    if (filter is not null)
    {
      var queryParams = new List<string>();

      if (filter.Type.HasValue)
        queryParams.Add($"Type={filter.Type.Value}");

      if (filter.From.HasValue)
        queryParams.Add($"From={filter.From.Value:yyyy-MM-ddTHH:mm:ss.fffZ}");

      if (filter.To.HasValue)
        queryParams.Add($"To={filter.To.Value:yyyy-MM-ddTHH:mm:ss.fffZ}");

      if (queryParams.Count > 0)
        queryString = "?" + string.Join("&", queryParams);
    }
    Console.WriteLine(queryString);
    var response = await _httpClient.GetAsync($"/api/v1/litter{queryString}");
    if (!response.IsSuccessStatusCode)
    {
      var error = await response.Content.ReadAsStringAsync();
      throw new HttpRequestException($"Failed to get litters. Status: {response.StatusCode}. Details: {error}");
    }

    var result = await response.Content.ReadFromJsonAsync<List<Litter>>() ?? throw new InvalidOperationException("Failed to deserialize litter list from response.");
    return result;
  }

  public async Task<List<Camera>?> GetCamerasAsync()
  {
    var response = await _httpClient.GetAsync("/api/v1/litter/cameras");
    if (!response.IsSuccessStatusCode)
    {
      var error = await response.Content.ReadAsStringAsync();
      throw new HttpRequestException($"Failed to get cameras. Status: {response.StatusCode}. Details: {error}");
    }

    var result = await response.Content.ReadFromJsonAsync<List<Camera>>();
    return result == null ? throw new InvalidOperationException("Failed to deserialize camera list from response.") : result;
  }

  public async Task<List<PredictionResponse>?> PredictAsync(int amountOfDays, int cameraId)
  {
    var queryString = $"?amountOfDays={amountOfDays}&CameraId={cameraId}";
    var response = await _httpClient.PostAsync($"/api/v1/litter/predict{queryString}", null);
    if (!response.IsSuccessStatusCode)
    {
      var error = await response.Content.ReadAsStringAsync();
      throw new HttpRequestException($"Prediction failed. Status: {response.StatusCode}. Details: {error}");
    }

    var result = await response.Content.ReadFromJsonAsync<List<PredictionResponse>>();
    return result == null ? throw new InvalidOperationException("Failed to deserialize prediction response.") : result;
  }

  public async Task<bool> RetrainModelAsync(int cameraId)
  {
    var queryString = $"?CameraId={cameraId}";
    var response = await _httpClient.PostAsync($"/api/v1/litter/retrain{queryString}", null);

    if (!response.IsSuccessStatusCode)
    {
      var error = await response.Content.ReadAsStringAsync();
      throw new HttpRequestException($"Model retraining failed. Status: {response.StatusCode}. Details: {error}");
    }

    return true;
  }

  public async Task<bool> ImportTrashDataAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      var response = await _httpClient.PostAsync("/api/v1/TrashDTO/import-trash-data", null, cancellationToken);
      if (!response.IsSuccessStatusCode)
      {
        var error = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"Importing trash data failed. Status: {response.StatusCode}. Details: {error}");
      }

      return true;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error importing trash data: {ex.Message}");
      throw new InvalidOperationException("An error occurred while importing trash data.", ex);
    }
  }

  public async Task<List<LitterAmountCamera>?> GetAmountPerLocationAsync()
  {
    var response = await _httpClient.GetAsync("/api/v1/litter/amount-per-location");
    if (!response.IsSuccessStatusCode)
    {
      var error = await response.Content.ReadAsStringAsync();
      throw new HttpRequestException($"Failed to get amount per location. Status: {response.StatusCode}. Details: {error}");
    }

    var result = await response.Content.ReadFromJsonAsync<List<LitterAmountCamera>>();
    return result == null ? throw new InvalidOperationException("Failed to deserialize amount per location response.") : result;
  }
}
