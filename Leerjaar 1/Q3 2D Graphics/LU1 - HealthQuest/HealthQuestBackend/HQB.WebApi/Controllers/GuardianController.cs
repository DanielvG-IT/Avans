using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HQB.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class GuardianController : ControllerBase
    {
        private readonly IGuardianRepository _guardianRepository;
        private readonly ILogger<GuardianController> _logger;

        public GuardianController(IGuardianRepository guardianRepository, ILogger<GuardianController> logger)
        {
            _guardianRepository = guardianRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetOuderVoogd()
        {
            _logger.LogInformation("Getting all guardians");
            var guardians = await _guardianRepository.GetAllGuardiansAsync();
            return Ok(guardians);
        }

        [HttpPost]
        public async Task<IActionResult> PostOuderVoogd([FromBody] Guardian guardian)
        {
            _logger.LogInformation("Adding a new guardian");
            var result = await _guardianRepository.AddGuardianAsync(guardian);
            if (result > 0)
            {
                _logger.LogInformation("Guardian added successfully");
                return CreatedAtAction(nameof(GetOuderVoogdById), new { id = guardian.ID }, guardian);
            }
            _logger.LogWarning("Failed to add guardian");
            return BadRequest();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetOuderVoogdById(Guid id)
        {
            _logger.LogInformation($"Getting guardian with ID: {id}");
            var guardian = await _guardianRepository.GetGuardianByIdAsync(id);
            if (guardian == null)
            {
                _logger.LogWarning($"Guardian with ID: {id} not found");
                return NotFound();
            }
            return Ok(guardian);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> PutOuderVoogd(Guid id, [FromBody] Guardian guardian)
        {
            if (id != guardian.ID)
            {
                _logger.LogWarning("ID mismatch");
                return BadRequest();
            }

            _logger.LogInformation($"Updating guardian with ID: {id}");
            var result = await _guardianRepository.UpdateGuardianAsync(guardian);
            if (result > 0)
            {
                _logger.LogInformation("Guardian updated successfully");
                return NoContent();
            }
            _logger.LogWarning($"Guardian with ID: {id} not found");
            return NotFound();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteOuderVoogd(Guid id)
        {
            _logger.LogInformation($"Deleting guardian with ID: {id}");
            var result = await _guardianRepository.DeleteGuardianAsync(id);
            if (result > 0)
            {
                _logger.LogInformation("Guardian deleted successfully");
                return NoContent();
            }
            _logger.LogWarning($"Guardian with ID: {id} not found");
            return NotFound();
        }
    }
}
