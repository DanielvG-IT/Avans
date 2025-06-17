using Microsoft.EntityFrameworkCore;
using Api.Models.Enums;
using Api.Interfaces;
using Api.Models;
using Api.Data;

namespace Api.Services
{

    public class TrashImportService(
        LitterDbContext dbContext,
        IAggregatedTrashService aggregatedTrashService,
        IHolidayApiService holidayApiService,
        IDTOService dTOService) : ITrashImportService
    {
        private readonly LitterDbContext _dbContext = dbContext;
        private readonly IAggregatedTrashService _aggregatedTrashService = aggregatedTrashService;
        private readonly IHolidayApiService _holidayApiService = holidayApiService;
        private readonly IDTOService _dTOService = dTOService;

        public async Task<bool> ImportAsync(CancellationToken ct)
        {
            try
            {
                var results = await _aggregatedTrashService.GetAggregatedTrashAsync();
                if (results == null || results.Count == 0)
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

                // Check for existing IDs to avoid duplicates
                var existingIds = await _dbContext.Litters
                    .Where(l => results.Select(r => r.Id).Contains(l.Id))
                    .Select(l => l.Id)
                    .ToListAsync(ct);

                var newLitters = new List<Litter>();

                foreach (var trash in results)
                {
                    // Skip if already exists
                    if (existingIds.Contains(trash.Id))
                    {
                        continue;
                    }

                    // Determine if it's a holiday and get the category
                    var isHoliday = holidayDictionary[DateOnly.FromDateTime(trash.Date.Date)];
                    var switchedType = trash.Type is not null ? _dTOService.GetCategory(trash.Type) : Category.Unknown;

                    // Create a new Litter object
                    var litter = new Litter
                    {
                        Id = trash.Id,
                        Type = switchedType,
                        Date = trash.Date,
                        Confidence = trash.Confidence,
                        Weather = trash.Weather,
                        Temperature = trash.Temperature,
                        Latitude = trash.Latitude,
                        Longitude = trash.Longitude,
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
    }
}
