using Microsoft.Extensions.Options;
using System.Text.Json;
using Api.Interfaces;
using System.Net;
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

            var queryparams = $"?country={countryCode}&year={date.Year}&month={date.Month}&day={date.Day}&key={_apiKey}";

            var response = await _httpClient.GetAsync(queryparams);
            if (response.StatusCode == HttpStatusCode.NotFound)
                return false; // No holidays found for the given date and country code

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var holidayResponse = JsonSerializer.Deserialize<HolidayApiResponse>(jsonResponse, _jsonSerializerOptions);

            return holidayResponse?.Holidays is not null && holidayResponse.Holidays.Length > 0;
        }
    }
}