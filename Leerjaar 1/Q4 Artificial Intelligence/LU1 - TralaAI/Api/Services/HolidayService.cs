using Microsoft.Extensions.Options;
using System.Text.Json;
using Api.Interfaces;
using Api.Models;

namespace Api.Services
{
    public class HolidayApiService(HttpClient httpClient, IOptions<ApiKeysOptions> apiKeysOptions) : IHolidayApiService
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
        private readonly HttpClient _httpClient = httpClient;
        private readonly string _apiKey = apiKeysOptions.Value.HolidayApiKey;

        public async Task<bool> IsHolidayAsync(DateTime date, string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentException("Country code cannot be null or empty.", nameof(countryCode));

            string queryparams = $"?country={countryCode}&year={date.Year}&month={date.Month}&day={date.Day}&key={_apiKey}";

            HttpResponseMessage response = await _httpClient.GetAsync(queryparams);
            response.EnsureSuccessStatusCode();

            string jsonResponse = await response.Content.ReadAsStringAsync();
            var holidayResponse = JsonSerializer.Deserialize<HolidayApiResponse>(jsonResponse, _jsonSerializerOptions);

            return holidayResponse?.Holidays != null && holidayResponse.Holidays.Length > 0;
        }
    }
}