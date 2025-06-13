using Microsoft.AspNetCore.Mvc;
using Api.Models;
using Api.Interfaces;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LitterController(ILitterRepository litterRepository) : ControllerBase
{
    private readonly ILitterRepository _litterRepository = litterRepository;

    [HttpGet]
    public async Task<ActionResult<List<Litter>>> Get([FromQuery] LitterFilterDto filter)
    {
        var litters = await _litterRepository.GetFilteredAsync(filter);

        if (litters == null || litters.Count == 0)
            return NotFound("No litter found matching the filter.");

        return Ok(litters);
    }
}