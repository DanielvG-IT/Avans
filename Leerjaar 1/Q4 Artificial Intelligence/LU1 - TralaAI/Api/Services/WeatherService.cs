
using Api.Models;
using Api.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Api.Services;

public class WeatherService(HttpClient httpClient, IOptions<ApiKeysOptions> apiKeysOptions) : IWeatherService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _apiKey = apiKeysOptions.Value.WeatherApiKey;

    public async Task<FastApiWeatherRequirements> GetWeatherAsync(DateOnly date)
    {
        if (date < DateOnly.FromDateTime(DateTime.Now))
            throw new ArgumentException("The date must be now or in the future.");
        if (date > DateOnly.FromDateTime(DateTime.Now.AddDays(14)))
            throw new ArgumentException("The date must be within the next 14 days.");

        var requestUrl = $"/forecast.json?key={_apiKey}&q=Breda&dt={date:yyyy-MM-dd}&alerts=no&aqi=no&tides=no";
        var response = await _httpClient.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Failed to fetch weather data: {response.ReasonPhrase}");

        var content = await response.Content.ReadAsStringAsync();
        var weatherData = JsonSerializer.Deserialize<WeatherResponse>(content);

        if (weatherData is null || weatherData.Forecast?.ForecastDays is null || !weatherData.Forecast.ForecastDays.Any(f => f.Date == date.ToString("yyyy-MM-dd")))
            throw new InvalidOperationException("Invalid weather data received.");

        var forecastDay = weatherData.Forecast?.ForecastDays?.FirstOrDefault(f => f.Date == date.ToString("yyyy-MM-dd"));

        if (forecastDay?.Day?.AvgTempC is null)
            throw new InvalidOperationException("No weather data found for the specified date.");
        if (forecastDay.Day.Condition is null)
            throw new InvalidOperationException("No weather condition data found for the specified date.");

        return new FastApiWeatherRequirements
        {
            Temperature = forecastDay.Day.AvgTempC,
            Condition = forecastDay.Day.Condition.Text
        };
    }
}