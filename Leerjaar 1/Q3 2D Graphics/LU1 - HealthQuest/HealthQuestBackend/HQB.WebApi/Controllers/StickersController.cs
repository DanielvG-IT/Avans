using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HQB.WebApi.Controllers
{
  [Route("api/v1/[controller]")]
  [ApiController]
  public class StickersController : ControllerBase
  {
    private readonly IStickersRepository _stickerService;
    private readonly ILogger<StickersController> _logger;

    public StickersController(IStickersRepository stickerService, ILogger<StickersController> logger)
    {
      _stickerService = stickerService ?? throw new ArgumentNullException(nameof(stickerService));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // GET: api/v1/stickers
    [HttpGet(Name = "GetAllStickers")]
    public async Task<ActionResult<IEnumerable<Sticker>>> GetAllStickers()
    {
      try
      {
        var stickers = await _stickerService.GetAllStickersAsync();
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
        await _stickerService.AddStickerAsync(sticker);
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

        var sticker = await _stickerService.GetStickerByIdAsync(id);
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
        if (!ModelState.IsValid)
        {
          _logger.LogWarning("Invalid sticker model.");
          return BadRequest(ModelState);
        }

        if (id != sticker.Id || id == Guid.Empty)
        {
          _logger.LogWarning("Sticker ID mismatch or invalid ID.");
          return BadRequest("Sticker ID mismatch or invalid ID.");
        }

        var existingSticker = await _stickerService.GetStickerByIdAsync(id);
        if (existingSticker == null)
        {
          _logger.LogWarning($"Sticker with ID {id} not found.");
          return NotFound();
        }

        await _stickerService.UpdateStickerAsync(sticker);

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

        var sticker = await _stickerService.GetStickerByIdAsync(id);
        if (sticker == null)
        {
          _logger.LogWarning($"Sticker with ID {id} not found.");
          return NotFound();
        }

        await _stickerService.DeleteStickerAsync(id);
        return NoContent();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"An error occurred while deleting sticker with ID {id}.");
        return StatusCode(500, "Internal server error.");
      }
    }
  }
}
