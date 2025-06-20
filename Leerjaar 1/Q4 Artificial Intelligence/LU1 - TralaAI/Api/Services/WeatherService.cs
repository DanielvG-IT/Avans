
using Api.Models;
using Api.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Api.Services;

public class WeatherService(HttpClient httpClient, IOptions<ApiKeysOptions> apiKeysOptions) : IWeatherService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _apiKey = apiKeysOptions.Value.WeatherApiKey;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<List<FastApiWeatherRequirements>> GetWeatherAsync(int amountOfDays)
    {
        if (amountOfDays < 1 || amountOfDays > 14)
            throw new ArgumentException("The number of days must be between 1 and 14.");

        var requestUrl = $"/v1/forecast.json?key={_apiKey}&q=Breda&days={amountOfDays}&aqi=no&alerts=no";
        var response = await _httpClient.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Failed to fetch weather data: {response.ReasonPhrase}");
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Weather API Response: {content}");
        var weatherData = JsonSerializer.Deserialize<WeatherResponse>(content, _jsonOptions);

        if (weatherData is null || weatherData.Forecast is null || weatherData.Forecast.Forecastday == null || weatherData.Forecast.Forecastday.Count == 0)
            throw new HttpRequestException("No weather data found in the response.");

        var result = new List<FastApiWeatherRequirements>();
        foreach (var day in weatherData.Forecast.Forecastday)
        {
            result.Add(new FastApiWeatherRequirements
            {
                Condition = day.Day.Condition.Text,
                Temperature = (double)day.Day.AvgtempC
            });
        }
        return result;
    }
}