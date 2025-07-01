using Api.Models;
using Blazor.Interfaces;
using System.Net.Http.Json;

namespace Blazor.Services;

public class HealthService(HttpClient httpClient) : IHealthService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<bool> GetBackendHealthAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/v1/health");
            return !response.IsSuccessStatusCode
                    ? throw new HttpRequestException($"Backend health check failed with status code: {response.StatusCode}")
                    : true;
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while checking backend health.", ex);
        }
    }

    public async Task<bool> GetFastApiHealthAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/v1/health/fastapi");
            return !response.IsSuccessStatusCode
                    ? throw new HttpRequestException($"FastAPI health check failed with status code: {response.StatusCode}")
                    : true;
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while checking FastAPI health.", ex);
        }
    }

    public async Task<ModelStatusResponse?> GetFastApiModelDataAsync(int cameraId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/v1/health/fastapi/model?cameraId={cameraId}");
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"FastAPI model data request failed for cameraId={cameraId} with status code: {response.StatusCode}");

            var modelStatus = await response.Content.ReadFromJsonAsync<ModelStatusResponse?>() ?? throw new Exception($"Failed to deserialize model status response for cameraId={cameraId}.");
            return modelStatus;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error occurred while fetching FastAPI model data for cameraId={cameraId}.", ex);
        }
    }

    public async Task<bool> GetSensoringHealthAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/v1/health/sensoringapi");
            return !response.IsSuccessStatusCode
                    ? throw new HttpRequestException($"Sensoring API health check failed with status code: {response.StatusCode}")
                    : true;
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while checking Sensoring API health.", ex);
        }
    }

    public async Task<bool> GetHolidayApiHealthAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/v1/health/holidayapi");
            return !response.IsSuccessStatusCode
                    ? throw new HttpRequestException($"Holiday API health check failed with status code: {response.StatusCode}")
                    : true;
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while checking Holiday API health.", ex);
        }
    }

    public async Task<bool> GetWeatherApiHealthAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/v1/health/weatherapi");
            return !response.IsSuccessStatusCode
                    ? throw new HttpRequestException($"Weather API health check failed with status code: {response.StatusCode}")
                    : true;
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while checking Weather API health.", ex);
        }
    }
}
