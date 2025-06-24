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
      return null;

    return await response.Content.ReadFromJsonAsync<List<Litter>>();
  }

  public async Task<List<Camera>?> GetCamerasAsync()
  {
    var response = await _httpClient.GetAsync("/api/v1/litter/cameras");
    return await response.Content.ReadFromJsonAsync<List<Camera>>();
  }

  public async Task<List<PredictionResponse>?> PredictAsync(int amountOfDays, int cameraId)
  {
    var queryString = $"?amountOfDays={amountOfDays}&CameraId={cameraId}";
    var response = await _httpClient.PostAsync($"/api/v1/litter/predict{queryString}", null);
    if (!response.IsSuccessStatusCode)
      return null;

    return await response.Content.ReadFromJsonAsync<List<PredictionResponse>>();
  }

  public async Task<bool> RetrainModelAsync(int cameraId)
  {
    var queryString = $"?CameraId={cameraId}";
    var response = await _httpClient.PostAsync($"/api/v1/litter/retrain{queryString}", null);

    return response.IsSuccessStatusCode;
  }

  public async Task<bool> ImportTrashDataAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      var response = await _httpClient.PostAsync("/api/v1/TrashDTO/import-trash-data", null, cancellationToken);
      if (!response.IsSuccessStatusCode)
        return false;

      return true;
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error importing trash data: {ex.Message}");
      return false;
    }
  }

  public async Task<List<LitterAmountCamera>?> GetAmountPerLocationAsync()
  {
    var response = await _httpClient.GetAsync("/api/v1/litter/amount-per-location");
    if (!response.IsSuccessStatusCode)
      return null;

    return await response.Content.ReadFromJsonAsync<List<LitterAmountCamera>?>();
  }
}
