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
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sticker>>> GetAllStickers()
    {
      var stickers = await _stickerService.GetAllStickersAsync();
      if (stickers == null)
      {
        _logger.LogWarning("No stickers found.");
        return NotFound("No stickers found.");
      }
      return Ok(stickers);
    }

    // POST: api/v1/stickers
    [HttpPost]
    public async Task<ActionResult<Sticker>> AddSticker([FromBody] Sticker sticker)
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

    // GET: api/v1/stickers/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Sticker>> GetStickerById(Guid id)
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

    // PUT: api/v1/stickers/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSticker(Guid id, [FromBody] Sticker sticker)
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

    // DELETE: api/v1/stickers/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSticker(Guid id)
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
  }
}
