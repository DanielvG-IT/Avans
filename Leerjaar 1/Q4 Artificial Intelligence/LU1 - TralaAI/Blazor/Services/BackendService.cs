using Blazor.Interfaces;

namespace Blazor.Services
{
  public class BackendService(HttpClient httpClient) : IBackendService // TODO: May add API key into constructor if needed
  {
    private readonly HttpClient _httpClient = httpClient;

    // TODO: Make specific methodes for each endpoint
  }
}