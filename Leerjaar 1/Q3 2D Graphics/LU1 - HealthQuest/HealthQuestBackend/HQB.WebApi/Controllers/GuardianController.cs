using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace HQB.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class GuardianController : ControllerBase
    {
        private readonly IGuardianRepository _guardianRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<GuardianController> _logger;

        public GuardianController(ILogger<GuardianController> logger, IGuardianRepository guardianRepository, IPatientRepository patientRepository)
        {
            _guardianRepository = guardianRepository;
            _patientRepository = patientRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Guardian>>> GetGuardian()
        {
            _logger.LogInformation("Getting all guardians");
            var guardians = await _guardianRepository.GetAllGuardiansAsync();

            if (guardians == null)
            {
                _logger.LogWarning("Guardian data is null.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }

            if (!guardians.Any())
            {
                _logger.LogWarning("No guardians found.");
                return NotFound("No guardians found.");
            }

            return Ok(guardians);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Guardian>> GetGuardianById(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid guardian ID provided.");
                return BadRequest("Invalid guardian ID.");
            }

            _logger.LogInformation("Getting guardian with ID: {id}", id);
            var guardian = await _guardianRepository.GetGuardianByIdAsync(id);
            if (guardian == null)
            {
                _logger.LogWarning("Guardian with ID: {Id} not found", id);
                return NotFound();
            }

            return Ok(guardian);
        }

        [HttpGet]
        [Route("{id}/patients")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatientsByGuardianId(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid guardian ID provided.");
                return BadRequest("Invalid guardian ID.");
            }

            _logger.LogInformation("Getting patients for guardian with ID: {id}.", id);
            var patients = await _patientRepository.GetPatientsByGuardianId(id);
            if (patients == null)
            {
                _logger.LogWarning("Patient data is null.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }

            if (!patients.Any())
            {
                _logger.LogWarning("No patients found for guardian with ID: {Id}.", id);
                return NotFound($"No patients found for guardian with ID: {id}.");
            }

            return Ok(patients);
        }

        [HttpPost]
        public async Task<ActionResult<Guardian>> AddGuardian([FromBody] Guardian guardian)
        {
            if (guardian == null)
            {
                _logger.LogWarning("Guardian information is required.");
                return BadRequest("Guardian information is required.");
            }

            if (string.IsNullOrEmpty(guardian.FirstName) || string.IsNullOrWhiteSpace(guardian.LastName))
            {
                _logger.LogWarning("Guardian name is required.");
                return BadRequest("Guardian name is required.");
            }

            _logger.LogInformation("Adding a new guardian with ID {guardian.ID}", guardian.ID);
            await _guardianRepository.AddGuardianAsync(guardian);
            return CreatedAtAction(nameof(GetGuardianById), new { id = guardian.ID }, guardian);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<Guardian>> UpdateGuardian(Guid id, [FromBody] Guardian guardian)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid guardian ID provided.");
                return BadRequest("Invalid guardian ID.");
            }

            if (guardian == null)
            {
                _logger.LogWarning("Guardian information is required.");
                return BadRequest("Guardian information is required.");
            }

            if (id != guardian.ID)
            {
                _logger.LogWarning("ID mismatch");
                return BadRequest("ID mismatch.");
            }

            _logger.LogInformation($"Updating guardian with ID: {id}");
            var result = await _guardianRepository.UpdateGuardianAsync(guardian);
            if (result > 0)
            {
                _logger.LogInformation("Guardian updated successfully");
                return Ok(guardian);
            }
            _logger.LogWarning($"Guardian with ID: {id} not found");
            return NotFound();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteGuardian(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid guardian ID provided.");
                return BadRequest("Invalid guardian ID.");
            }

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