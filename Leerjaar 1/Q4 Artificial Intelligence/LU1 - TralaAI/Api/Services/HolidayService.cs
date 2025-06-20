using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using Api.Interfaces;
using System.Net;
using Api.Models;

namespace Api.Services
{
    public class HolidayApiService(HttpClient httpClient, IMemoryCache cache) : IHolidayApiService
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
        private readonly HttpClient _httpClient = httpClient;
        private readonly IMemoryCache _cache = cache;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(12);

        public async Task<bool> IsHolidayAsync(DateTime date, string countryCode, string year)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentException("Country code cannot be null or empty.", nameof(countryCode));

            string cacheKey = $"{countryCode}:{year}";
            HolidayApiResponse? holidayResponse = _cache.Get<HolidayApiResponse>(cacheKey);

            if (holidayResponse == null)
            {
                try
                {
                    var response = await _httpClient.GetAsync($"/{year}/{countryCode}");
                    if (response.StatusCode == HttpStatusCode.NotFound)
                        return false;

                    response.EnsureSuccessStatusCode();

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    holidayResponse = JsonSerializer.Deserialize<HolidayApiResponse>(jsonResponse, _jsonSerializerOptions);

                    if (holidayResponse != null)
                    {
                        _cache.Set(cacheKey, holidayResponse, CacheDuration);
                    }
                }
                catch (Exception ex)
                {
                    // Optionally log the exception here
                    throw new ApplicationException("An error occurred while checking for holidays.", ex);
                }
            }

            return holidayResponse?.Holidays.Any(h => DateTime.Parse(h.Date).Date == date.Date && h.Public) ?? false;
        }
    }
}