using System.Net;
using Blazor.Models;
using Blazor.Interfaces;
using Blazor.Models.Health;

namespace Blazor.Services
{
  public class BackendService(HttpClient httpClient) : IBackendService
  {
    private readonly HttpClient _httpClient = httpClient;

    public async Task<List<Litter>?> GetLittersAsync(LitterFilterDto? filter)
    {
      filter ??= new LitterFilterDto();

      var queryString = BuildQueryString(filter);
      var response = await _httpClient.GetAsync($"/api/v1/litters?{queryString}");

      if (response.StatusCode == HttpStatusCode.NotFound)
        return null;

      response.EnsureSuccessStatusCode();
      var litters = await response.Content.ReadFromJsonAsync<List<Litter>>();
      return litters;
    }

    public async Task<List<PredictionResponse>?> PredictAsync(int amountOfDays, string location)
    {
      var queryString = $"amountOfDays={amountOfDays}&location={Uri.EscapeDataString(location)}";
      var response = await _httpClient.PostAsync($"/api/v1/litter/predict?{queryString}", null);

      if (response.StatusCode == HttpStatusCode.BadRequest)
        return null;

      response.EnsureSuccessStatusCode();

      var predictions = await response.Content.ReadFromJsonAsync<List<PredictionResponse>>();
      return predictions;
    }

    public async Task<string?> RetrainModelAsync()
    {
      var response = await _httpClient.PostAsync("/api/v1/litter/retrain", null);

      if (response.StatusCode == HttpStatusCode.BadRequest)
        return null;

      response.EnsureSuccessStatusCode();

      var message = await response.Content.ReadAsStringAsync();
      return message;
    }

    public async Task<HealthStatus?> GetHealthStatusAsync()
    {
      var response = await _httpClient.GetAsync("/api/v1/health");
      response.EnsureSuccessStatusCode();
      var healthStatus = await response.Content.ReadFromJsonAsync<HealthStatus>();
      return healthStatus;
    }

    public async Task<string?> ImportTrashDataAsync(CancellationToken cancellationToken = default)
    {
      var response = await _httpClient.PostAsync("/api/v1/TrashDTO/import-trash-data", null, cancellationToken);

      if (response.StatusCode == HttpStatusCode.BadRequest)
        return null;

      response.EnsureSuccessStatusCode();

      var message = await response.Content.ReadAsStringAsync(cancellationToken);
      return message;
    }

    private static string BuildQueryString(LitterFilterDto filter)
    {
      var properties = typeof(LitterFilterDto).GetProperties();
      var queryParams = new List<string>();

      foreach (var prop in properties)
      {
        var value = prop.GetValue(filter);
        if (value != null)
        {
          queryParams.Add($"{prop.Name}={Uri.EscapeDataString(value.ToString()!)}");
        }
      }

      return queryParams.Count > 0 ? string.Join("&", queryParams) : string.Empty;
    }
  }
}