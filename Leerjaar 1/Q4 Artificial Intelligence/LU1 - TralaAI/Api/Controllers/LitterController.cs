using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Data;
using Api.Models;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LitterController : ControllerBase
{
    private readonly LitterDbContext _context;

    public LitterController(LitterDbContext context)
    {
        _context = context;
    }

    [HttpGet("filter")]
    public async Task<IActionResult> GetFiltered(
        [FromQuery] string? type,
        [FromQuery] DateTime? date,
        [FromQuery] bool? isHoliday,
        [FromQuery] string? weather,
        [FromQuery] int? minTemp,
        [FromQuery] int? maxTemp)
    {
        var query = _context.Litters.AsQueryable();

        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(l => l.Type == type);

        if (date.HasValue)
            query = query.Where(l => l.Date.Date == date.Value.Date);

        if (isHoliday.HasValue)
            query = query.Where(l => l.IsHoliday == isHoliday.Value);

        if (!string.IsNullOrWhiteSpace(weather))
            query = query.Where(l => l.Weather == weather);

        if (minTemp.HasValue)
            query = query.Where(l => l.Temperature >= minTemp.Value);

        if (maxTemp.HasValue)
            query = query.Where(l => l.Temperature <= maxTemp.Value);

        var result = await query.OrderByDescending(l => l.Date).ToListAsync();

        return Ok(result);
    }
}