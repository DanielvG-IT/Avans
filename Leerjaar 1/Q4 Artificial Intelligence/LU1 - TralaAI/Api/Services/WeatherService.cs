
using Api.Models;
using Api.Interfaces;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Api.Services;

public class WeatherService(HttpClient httpClient, IOptions<ApiKeysOptions> apiKeysOptions) : IWeatherService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly string _apiKey = apiKeysOptions.Value.WeatherApiKey;

    public async Task<List<FastApiWeatherRequirements>> GetWeatherAsync(int amountOfDays)
    {
        if (amountOfDays < 1 || amountOfDays > 14)
            throw new ArgumentException("The number of days must be between 1 and 14.");

        var requestUrl = $"/forecast.json?key={_apiKey}&q=Breda&days={amountOfDays}&aqi=no&alerts=no";
        var response = await _httpClient.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Failed to fetch weather data: {response.ReasonPhrase}");

        var content = await response.Content.ReadAsStringAsync();
        var weatherData = JsonSerializer.Deserialize<ForecastResponse>(content);

        if (weatherData is null || weatherData.Forecastday is null || weatherData.Forecastday.Count == 0)
            throw new HttpRequestException("No weather data found in the response.");

        var result = new List<FastApiWeatherRequirements>();
        foreach (var day in weatherData.Forecastday)
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