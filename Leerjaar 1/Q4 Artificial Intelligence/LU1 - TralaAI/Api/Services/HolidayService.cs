using System.Text.Json;
using Api.Interfaces;
using Api.Models;

namespace Api.Services
{
    public class HolidayApiService(string apiKey, HttpClient httpClient) : IHolidayApiService
    {
        private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        private readonly string _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        public async Task<bool> IsHolidayAsync(DateTime date, string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentException("Country code cannot be null or empty.", nameof(countryCode));

            string url = $"https://holidayapi.com/v1/holidays?country={countryCode}&year={date.Year}&month={date.Month}&day={date.Day}&key={_apiKey}";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                var holidayResponse = JsonSerializer.Deserialize<HolidayApiResponse>(jsonResponse, _jsonSerializerOptions);

                return holidayResponse?.Holidays?.Length > 0 ?? false;
            }
            catch (HttpRequestException ex)
            {
                // Log the exception or handle it as needed
                throw new Exception($"Error fetching holiday data: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                // Log the exception or handle it as needed
                throw new Exception($"Error parsing holiday data: {ex.Message}", ex);
            }
        }
    }
}