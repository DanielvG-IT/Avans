using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HQB.WebApi.Controllers
{
  [Route("api/v1/[controller]")]
  [ApiController]
  public class StickersController : ControllerBase
  {
    private readonly IStickersRepository _stickerRepository;
    private readonly ILogger<StickersController> _logger;

    public StickersController(IStickersRepository stickerRepository, ILogger<StickersController> logger)
    {
      _stickerRepository = stickerRepository ?? throw new ArgumentNullException(nameof(stickerRepository));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // GET: api/v1/stickers
    [HttpGet(Name = "GetAllStickers")]
    public async Task<ActionResult<IEnumerable<Sticker>>> GetAllStickers()
    {
      try
      {
        var stickers = await _stickerRepository.GetAllStickersAsync();
        if (stickers == null)
        {
          _logger.LogWarning("No stickers found.");
          return NotFound("No stickers found.");
        }
        return Ok(stickers);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "An error occurred while retrieving stickers.");
        return StatusCode(500, "Internal server error.");
      }
    }

    // POST: api/v1/stickers
    [HttpPost(Name = "AddSticker")]
    public async Task<ActionResult<Sticker>> AddSticker([FromBody] Sticker sticker)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          _logger.LogWarning("Invalid sticker model.");
          return BadRequest(ModelState);
        }

        sticker.Id = Guid.NewGuid();
        await _stickerRepository.AddStickerAsync(sticker);
        return CreatedAtAction(nameof(GetStickerById), new { id = sticker.Id }, sticker);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "An error occurred while adding a sticker.");
        return StatusCode(500, "Internal server error.");
      }
    }

    // GET: api/v1/stickers/{id}
    [HttpGet("{id}", Name = "GetStickerById")]
    public async Task<ActionResult<Sticker>> GetStickerById(Guid id)
    {
      try
      {
        if (id == Guid.Empty)
        {
          _logger.LogWarning("Invalid sticker ID.");
          return BadRequest("Invalid sticker ID.");
        }

        var sticker = await _stickerRepository.GetStickerByIdAsync(id);
        if (sticker == null)
        {
          _logger.LogWarning($"Sticker with ID {id} not found.");
          return NotFound();
        }

        return Ok(sticker);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while retrieving sticker with ID {id}.");
        return StatusCode(500, "Internal server error.");
      }
    }

    // PUT: api/v1/stickers/{id}
    [HttpPut("{id}", Name = "UpdateSticker")]
    public async Task<IActionResult> UpdateSticker(Guid id, [FromBody] Sticker sticker)
    {
      try
      {
        if (id == Guid.Empty)
        {
          _logger.LogWarning("Sticker ID mismatch or invalid ID.");
          return BadRequest("Sticker ID mismatch or invalid ID.");
        }

        sticker.Id = id;

        if (!ModelState.IsValid)
        {
          _logger.LogWarning("Invalid sticker model.");
          return BadRequest(ModelState);
        }

        var existingSticker = await _stickerRepository.GetStickerByIdAsync(id);
        if (existingSticker == null)
        {
          _logger.LogWarning($"Sticker with ID {id} not found.");
          return NotFound();
        }

        await _stickerRepository.UpdateStickerAsync(sticker);

        return NoContent();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while updating sticker with ID {id}.");
        return StatusCode(500, "Internal server error.");
      }
    }

    // DELETE: api/v1/stickers/{id}
    [HttpDelete("{id}", Name = "DeleteSticker")]
    public async Task<IActionResult> DeleteSticker(Guid id)
    {
      try
      {
        if (id == Guid.Empty)
        {
          _logger.LogWarning("Invalid sticker ID.");
          return BadRequest("Invalid sticker ID.");
        }

        var sticker = await _stickerRepository.GetStickerByIdAsync(id);
        if (sticker == null)
        {
          _logger.LogWarning($"Sticker with ID {id} not found.");
          return NotFound();
        }

        await _stickerRepository.DeleteStickerAsync(id);
        return NoContent();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while deleting sticker with ID {id}.");
        return StatusCode(500, "Internal server error.");
      }
    }

    // GET: api/v1/stickers/search
    [HttpGet("search", Name = "SearchStickersByName")]
    public async Task<ActionResult<IEnumerable<Sticker>>> SearchStickersByName([FromQuery] string name)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(name))
        {
          _logger.LogWarning("Search name is empty or null.");
          return BadRequest("Search name cannot be empty or null.");
        }

        var sticker = (await _stickerRepository.GetAllStickersAsync())?.FirstOrDefault(s => s.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        if (sticker == null)
        {
          _logger.LogWarning($"No sticker found with name containing '{name}'.");
          return NotFound($"No sticker found with name containing '{name}'.");
        }

        return Ok(sticker);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while searching for stickers with name containing '{name}'.");
        return StatusCode(500, "Internal server error.");
      }
    }
  }
}
