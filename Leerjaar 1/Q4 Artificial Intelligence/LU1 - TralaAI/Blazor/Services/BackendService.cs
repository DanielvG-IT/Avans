using Blazor.Interfaces;
using Blazor.Models.Health;

namespace Blazor.Services
{
  public class BackendService(HttpClient httpClient) : IBackendService
  {
    private readonly HttpClient _httpClient = httpClient;

    public async Task<HealthStatus?> GetHealthStatusAsync()
    {
      var response = await _httpClient.GetAsync("/api/v1/health/status");
      response.EnsureSuccessStatusCode();
      var healthStatus = await response.Content.ReadFromJsonAsync<HealthStatus>();
      return healthStatus;
    }
  }
}