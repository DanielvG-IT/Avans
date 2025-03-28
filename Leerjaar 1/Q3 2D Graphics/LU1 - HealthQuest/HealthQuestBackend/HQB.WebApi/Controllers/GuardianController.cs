using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HQB.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class GuardianController : ControllerBase
    {
        private readonly IGuardianRepository _guardianRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<GuardianController> _logger;
        private readonly IAuthenticationService _authenticationService;

        public GuardianController(ILogger<GuardianController> logger, IGuardianRepository guardianRepository, IPatientRepository patientRepository, IAuthenticationService authenticationService)
        {
            _guardianRepository = guardianRepository;
            _patientRepository = patientRepository;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpGet(Name = "GetGuardianForCurrentUser")]
        public async Task<ActionResult<IEnumerable<Guardian>>> GetGuardianForCurrentUser()
        {
            try
            {
                var userId = _authenticationService.GetCurrentAuthenticatedUserId();
                if (userId == null)
                {
                    _logger.LogWarning("Authenticated user ID is null.");
                    return BadRequest("Authenticated user ID is required.");
                }

                _logger.LogInformation("Getting guardians for user with ID: {userId}", userId);

                var guardians = await _guardianRepository.GetGuardianByUserIdAsync(userId);
                if (guardians == null)
                {
                    _logger.LogWarning("No guardians found for user with ID: {userId}.", userId);
                    return NotFound($"No guardians found for user with ID: {userId}.");
                }

                return Ok(guardians);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting guardians for the current user.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet]
        [Route("{id}", Name = "GetGuardianById")]
        public async Task<ActionResult<Guardian>> GetGuardianById(Guid id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting the guardian by ID.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet]
        [Route("{id}/patients", Name = "GetPatientsByGuardianId")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatientsByGuardianId(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Invalid guardian ID provided.");
                    return BadRequest("Invalid guardian ID.");
                }

                _logger.LogInformation("Getting patients for guardian with ID: {id}.", id);

                var patients = await _patientRepository.GetPatientsByGuardianId(id);
                if (patients == null || !patients.Any())
                {
                    _logger.LogWarning("No patients found for guardian with ID: {Id}.", id);
                    return NotFound($"No patients found for guardian with ID: {id}.");
                }

                return Ok(patients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting patients by guardian ID.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost(Name = "AddGuardian")]
        public async Task<ActionResult<Guardian>> AddGuardian([FromBody] Guardian guardian)
        {
            try
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

                guardian.ID = Guid.NewGuid();

                var userId = _authenticationService.GetCurrentAuthenticatedUserId();
                if (userId == null)
                {
                    _logger.LogWarning("Authenticated user ID is null.");
                    return BadRequest("Authenticated user ID is required.");
                }

                guardian.UserID = userId;

                _logger.LogInformation("Adding a new guardian with ID {guardian.ID}", guardian.ID);
                await _guardianRepository.AddGuardianAsync(guardian);
                return CreatedAtAction(nameof(GetGuardianById), new { id = guardian.ID }, guardian);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new guardian.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut]
        [Route("{id}", Name = "UpdateGuardian")]
        public async Task<ActionResult<Guardian>> UpdateGuardian(Guid id, [FromBody] Guardian guardian)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the guardian.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete]
        [Route("{id}", Name = "DeleteGuardian")]
        public async Task<IActionResult> DeleteGuardian(Guid id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the guardian.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}