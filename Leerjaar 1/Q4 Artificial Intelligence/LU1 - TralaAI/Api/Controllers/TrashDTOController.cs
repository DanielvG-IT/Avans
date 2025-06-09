using Microsoft.AspNetCore.Mvc;
using Api.Interfaces;
using Api.Models;
using Api.Data;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]

    public class TrashDTOController(LitterDbContext context, IAggregatedTrashService aggregatedTrashService, ILitterRepository litterRepository) : ControllerBase
    {
        private readonly LitterDbContext _context = context;
        private readonly IAggregatedTrashService _aggregatedTrashService = aggregatedTrashService;
        private readonly ILitterRepository _litterRepository = litterRepository;

        [HttpPost("import-trash-data")]
        public async Task<IActionResult> ImportTrashData()
        {
            var sensoringData = await _aggregatedTrashService.GetAggregatedTrashAsync();

            foreach (var dto in sensoringData)
            {
                //Misschien hier HOLIDAY API aan toevoegen. Dat het gelijk datum uitleest en aangeeft of het een Vakantiedag is of niet.
                //bool isHoliday = await _holidayService.IsHolidayAsync(dto.Date);
                var litter = new Litter
                {
                    Type = dto.Type,
                    Date = dto.Date,
                    Confidence = dto.Confidence,
                    Weather = dto.Weather,
                    Temperature = dto.Temperature,
                    //IsHoliday = isHoliday,
                    //Latitud = ?,
                    //Longitude = ?

                };

                await _litterRepository.AddAsync(litter);
            }

            await _litterRepository.SaveChangesAsync();

            return Ok("Data imported");
        }
    }
}