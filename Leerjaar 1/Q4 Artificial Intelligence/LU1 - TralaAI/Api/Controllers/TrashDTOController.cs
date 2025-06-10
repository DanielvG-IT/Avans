using Microsoft.AspNetCore.Mvc;
using Api.Interfaces;
using Api.Models;
using Api.Data;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TrashDTOController(LitterDbContext dbContext, IAggregatedTrashService aggregatedTrashService, IHolidayApiService holidayApiService) : ControllerBase
    {
        private readonly LitterDbContext _dbContext = dbContext;
        private readonly IAggregatedTrashService _aggregatedTrashService = aggregatedTrashService;
        private readonly IHolidayApiService _holidayApiService = holidayApiService;

        [HttpPost("import-trash-data")]
        public async Task<IActionResult> ImportTrashData()
        {
            var results = await _aggregatedTrashService.GetAggregatedTrashAsync();
            foreach (var trash in results)
            {
                // Finding out if it is a holiday
                var isHoliday = await _holidayApiService.IsHolidayAsync(trash.Date, "NL");

                // Create a new Litter object
                var litter = new Litter
                {
                    Id = trash.Id,
                    Type = trash.Type,
                    Date = trash.Date,
                    Confidence = trash.Confidence,
                    Weather = trash.Weather,
                    Temperature = trash.Temperature,
                    Latitude = trash.Latitude,
                    Longitude = trash.Longitude,
                    IsHoliday = isHoliday
                };

                await _dbContext.Litters.AddAsync(litter);
            }
            await _dbContext.SaveChangesAsync();

            return Ok("Data imported");
        }
    }
}