using Microsoft.EntityFrameworkCore;
using Api.Models.Enums;
using Api.Interfaces;
using Api.Models;
using Api.Data;
using System.Net.Http.Json;
using System.Linq;

namespace Api.Services
{
    public class TrashImportService(LitterDbContext dbContext, IHolidayApiService holidayApiService, IDTOService dTOService, HttpClient httpClient) : ITrashImportService
    {
        private readonly LitterDbContext _dbContext = dbContext;
        private readonly IHolidayApiService _holidayApiService = holidayApiService;
        private readonly IDTOService _dTOService = dTOService;
        private readonly HttpClient _httpClient = httpClient;

        public async Task<bool> ImportAsync(CancellationToken ct)
        {
            try
            {
                var results = await GetAggregatedTrashAsync();
                if (results is null || results.Count == 0)
                {
                    return false;
                }

                // Get all unique dates from the results
                var uniqueDates = results.Select(t => t.Date.Date).Distinct().ToList();

                // Get holiday information for all dates
                var holidayDictionary = new Dictionary<DateOnly, bool>();
                foreach (var date in uniqueDates)
                {
                    holidayDictionary[DateOnly.FromDateTime(date)] = await _holidayApiService.IsHolidayAsync(date, "NL");
                }

                var newLitters = new List<Litter>();

                foreach (var trash in results)
                {
                    // Determine if it's a holiday and get the category
                    var isHoliday = holidayDictionary[DateOnly.FromDateTime(trash.Date.Date)];
                    var switchedType = trash.Type is not null ? _dTOService.GetCategory(trash.Type) : LitterCategory.Unknown;

                    // Create a new Litter object
                    var litter = new Litter
                    {
                        Id = trash.Id,
                        Type = switchedType,
                        TimeStamp = trash.Date,
                        Confidence = trash.Confidence,
                        Weather = _dTOService.GetWeatherCategory(trash.Weather),
                        Temperature = trash.Temperature,
                        Location = "Sensoring",
                        IsHoliday = isHoliday
                    };

                    newLitters.Add(litter);
                }

                // Use batch operation for efficiency
                await _dbContext.Litters.AddRangeAsync(newLitters, ct);

                // Transaction for data consistency
                using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
                try
                {
                    var savedCount = await _dbContext.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);

                    return savedCount > 0;
                }
                catch
                {
                    await transaction.RollbackAsync(ct);
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private async Task<List<AggregatedTrashDto>> GetAggregatedTrashAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/Litter/getLitter");
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode) // TODO Je kunt hier eventueel logging of foutafhandeling toevoegen
                return [];

            var content = await response.Content.ReadFromJsonAsync<List<AggregatedTrashDto>>();
            return content ?? [];
        }
    }
}
