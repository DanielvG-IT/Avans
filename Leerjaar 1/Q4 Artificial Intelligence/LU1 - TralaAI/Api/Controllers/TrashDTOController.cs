using Microsoft.AspNetCore.Mvc;
using Api.Interfaces;
using Api.Models;
using Api.Data;
using Api.Services;
using Api.Models.Enums;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TrashDTOController(LitterDbContext dbContext, IAggregatedTrashService aggregatedTrashService, IHolidayApiService holidayApiService, DTOService dTOService) : ControllerBase
    {
        private readonly LitterDbContext _dbContext = dbContext;
        private readonly IAggregatedTrashService _aggregatedTrashService = aggregatedTrashService;
        private readonly IHolidayApiService _holidayApiService = holidayApiService;
        private readonly DTOService _dTOService = dTOService;

        [HttpPost("import-trash-data")]
        public async Task<IActionResult> ImportTrashData()
        {
            var results = await _aggregatedTrashService.GetAggregatedTrashAsync();
            foreach (var trash in results)
            {
                // Finding out if it is a holiday
                var isHoliday = await _holidayApiService.IsHolidayAsync(trash.Date, "NL");
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

                await _dbContext.Litters.AddAsync(litter);
            }
            await _dbContext.SaveChangesAsync();

            return Ok("Data imported");
        }
    }
}