using System.Net;
using Blazor.Models;
using Blazor.Interfaces;

namespace Blazor.Services;

public class LitterService(HttpClient httpClient) : ILitterService
{
  private readonly HttpClient _httpClient = httpClient;

  public async Task<List<Litter>?> GetLittersAsync(LitterFilterDto? filter)
  {
    filter ??= new LitterFilterDto();
    var queryString = BuildQueryString(filter);
    var response = await _httpClient.GetAsync($"/api/v1/litter{queryString}");

    if (response.StatusCode == HttpStatusCode.NotFound)
      return null;

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

    if (response.StatusCode == HttpStatusCode.BadRequest)
      return null;

    if (response.StatusCode == HttpStatusCode.NotFound)
      return null;

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

  public async Task<string?> ImportTrashDataAsync(CancellationToken cancellationToken = default)
  {
    var response = await _httpClient.PostAsync("/api/v1/TrashDTO/import-trash-data", null, cancellationToken);

    if (response.StatusCode == HttpStatusCode.BadRequest)
      return null;

    if (!response.IsSuccessStatusCode)
      return null;

    return await response.Content.ReadAsStringAsync(cancellationToken);
  }

  public async Task<List<Litter>?> GetLatestLittersAsync(int? amount)
  {
    var queryString = amount.HasValue ? $"?amount={amount.Value}" : string.Empty;
    var response = await _httpClient.GetAsync($"/api/v1/litter/latest{queryString}");

    if (response.StatusCode == HttpStatusCode.NotFound)
      return null;

    if (!response.IsSuccessStatusCode)
      return null;

    return await response.Content.ReadFromJsonAsync<List<Litter>>();
  }

  public async Task<LitterTypeAmount?> GetAmountPerLocationAsync()
  {
    var response = await _httpClient.GetAsync("/api/v1/litter/amount-per-location");

    if (response.StatusCode == HttpStatusCode.NotFound)
      return null;

    if (!response.IsSuccessStatusCode)
      return null;

    return await response.Content.ReadFromJsonAsync<LitterTypeAmount>();
  }

  public async Task<LitterHistoryResponse?> GetLitterHistoryAsync()
  {
    var response = await _httpClient.GetAsync("/api/v1/litter/history");

    if (response.StatusCode == HttpStatusCode.InternalServerError)
      throw new Exception("An unexpected error occurred while retrieving litter history.");

    if (!response.IsSuccessStatusCode)
      return null;

    return await response.Content.ReadFromJsonAsync<LitterHistoryResponse>();
  }

  private static string BuildQueryString(LitterFilterDto filter)
  {
    var properties = typeof(LitterFilterDto).GetProperties();
    var queryParams = new List<string>();

    foreach (var prop in properties)
    {
      var value = prop.GetValue(filter);
      if (value is not null)
      {
        queryParams.Add($"{prop.Name}={Uri.EscapeDataString(value.ToString()!)}");
      }
    }

    return queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
  }
}
