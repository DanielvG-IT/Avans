using Api.Models;
using Blazor.Interfaces;

namespace Blazor.Services;

public class HealthService(HttpClient httpClient) : IHealthService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<bool> GetBackendHealthAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/v1/health");
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> GetFastApiHealthAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/v1/health/fastapi");
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<ModelStatusResponse?> GetFastApiModelDataAsync(int cameraId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/v1/health/fastapi/model?cameraId={cameraId}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<ModelStatusResponse?>();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> GetSensoringHealthAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/v1/health/sensoringapi");
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> GetHolidayApiHealthAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/v1/health/holidayapi");
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> GetWeatherApiHealthAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/v1/health/weatherapi");
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
