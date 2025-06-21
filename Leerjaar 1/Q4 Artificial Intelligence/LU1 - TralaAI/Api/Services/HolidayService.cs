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
        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(30);

        public async Task<bool?> IsHolidayAsync(DateTime date, string countryCode, string year)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentException("Country code cannot be null or empty.", nameof(countryCode));

            string cacheKey = $"holiday:{countryCode}:{year}:{date:yyyy-MM-dd}";

            // Use GetOrCreateAsync to avoid cache stampede
            return await _cache.GetOrCreateAsync<bool?>(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheDuration;

                try
                {
                    // Fetch the holidays for the year/country
                    string yearCacheKey = $"holidays:{countryCode}:{year}";
                    var holidayResponse = _cache.Get<List<HolidayApiResponse>>(yearCacheKey);

                    if (holidayResponse is null)
                    {
                        var response = await _httpClient.GetAsync($"/api/v3/PublicHolidays/{year}/{countryCode}");
                        if (response.StatusCode == HttpStatusCode.NotFound)
                            return false;

                        response.EnsureSuccessStatusCode();

                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"{jsonResponse}");
                        holidayResponse = JsonSerializer.Deserialize<List<HolidayApiResponse>>(jsonResponse, _jsonSerializerOptions);

                        if (holidayResponse is not null)
                            _cache.Set(yearCacheKey, holidayResponse, CacheDuration);
                    }

                    // Check if the date is a holiday
                    if (holidayResponse is not null)
                        return holidayResponse.Any(h => h.Date == date.Date.ToString("yyyy-MM-dd") && h.CountryCode.Equals(countryCode, StringComparison.OrdinalIgnoreCase));

                    return null;
                }
                catch (TaskCanceledException) // Timeout
                {
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            });
        }
    }
}