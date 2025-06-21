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

            string cacheKey = $"holiday:{countryCode}:{year}:{date:yyyy-MM-dd}";

            // Use GetOrCreateAsync to avoid cache stampede
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheDuration;

                // Fetch the holidays for the year/country
                string yearCacheKey = $"holidays:{countryCode}:{year}";
                HolidayApiResponse? holidayResponse = _cache.Get<HolidayApiResponse>(yearCacheKey);

                if (holidayResponse is null)
                {
                    var response = await _httpClient.GetAsync($"/{year}/{countryCode}");
                    if (response.StatusCode == HttpStatusCode.NotFound)
                        return false;

                    response.EnsureSuccessStatusCode();

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    holidayResponse = JsonSerializer.Deserialize<HolidayApiResponse>(jsonResponse, _jsonSerializerOptions);

                    if (holidayResponse != null)
                    {
                        _cache.Set(yearCacheKey, holidayResponse, CacheDuration);
                    }
                }

                return holidayResponse?.Holidays.Any(h => DateTime.Parse(h.Date).Date == date.Date && h.Public) ?? false;
            });
        }
    }
}