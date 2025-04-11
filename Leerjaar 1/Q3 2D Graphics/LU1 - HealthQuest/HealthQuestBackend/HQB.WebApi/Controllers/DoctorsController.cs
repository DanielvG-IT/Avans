using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HQB.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly ILogger<DoctorsController> _logger;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IAuthenticationService _authenticationService;

        public DoctorsController(ILogger<DoctorsController> logger, IDoctorRepository doctorRepository, IPatientRepository patientRepository, IAuthenticationService authenticationService)
        {
            _logger = logger;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _authenticationService = authenticationService;
        }

        [HttpGet(Name = "GetAllDoctors")]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors()
        {
            try
            {
                _logger.LogInformation("Fetching all doctors.");
                var doctors = await _doctorRepository.GetAllDoctorsAsync();

                if (doctors == null)
                {
                    _logger.LogError("Doctor repository returned null.");
                    return StatusCode(500, "Internal server error.");
                }

                if (!doctors.Any())
                {
                    _logger.LogWarning("No doctors found.");
                    return NotFound("No doctors found.");
                }

                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching doctors.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}", Name = "GetDoctorById")]
        public async Task<ActionResult<Doctor>> GetDoctor(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Invalid doctor ID provided.");
                    return BadRequest("Invalid doctor ID.");
                }

                _logger.LogInformation($"Fetching doctor with ID {id}.");
                var doctor = await _doctorRepository.GetDoctorByIdAsync(id);

                if (doctor == null)
                {
                    _logger.LogWarning($"Doctor with ID {id} not found.");
                    return NotFound($"Doctor with ID {id} not found.");
                }

                return Ok(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching doctor with ID {id}.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("me", Name = "GetCurrentDoctor")]
        public async Task<ActionResult<Doctor>> GetCurrentDoctor()
        {
            var userId = _authenticationService.GetCurrentAuthenticatedUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                return BadRequest("Invalid user ID format.");
            }
            if (parsedUserId == Guid.Empty)
            {
                return BadRequest("User ID cannot be empty.");
            }

            try
            {
                var userRoles = await _authenticationService.GetCurrentAuthenticatedUserRoles();
                if (!userRoles.Contains("Doctor"))
                {
                    return Forbid("You are not the doctor!");
                }

                var doctor = await _doctorRepository.GetDoctorByUserIDAsync(Guid.Parse(userId));
                if (doctor is null)
                {
                    return BadRequest("Doctor is null.");
                }
                
                return Ok(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching doctor with ID {userId}.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("by-patient/{patientId}", Name = "GetDoctorByPatientId")]
        public async Task<ActionResult<Doctor>> GetDoctorByPatientId(Guid patientId)
        {
            try
            {
                if (patientId == Guid.Empty)
                {
                    _logger.LogWarning("Invalid patient ID provided.");
                    return BadRequest("Invalid patient ID.");
                }

                _logger.LogInformation($"Fetching doctor for patient with ID {patientId}.");
                var patient = await _patientRepository.GetPatientByIdAsync(patientId);

                if (patient == null)
                {
                    _logger.LogWarning($"No patient found with ID {patientId}.");
                    return NotFound($"No patient found with ID {patientId}.");
                }

                if (patient.DoctorID == Guid.Empty)
                {
                    _logger.LogWarning($"Patient with ID {patientId} has no assigned doctor.");
                    return NotFound($"Patient with ID {patientId} has no assigned doctor.");
                }

                var doctor = patient.DoctorID.HasValue
                    ? await _doctorRepository.GetDoctorByIdAsync(patient.DoctorID.Value)
                    : null;
                if (doctor == null)
                {
                    _logger.LogWarning($"No doctor found for patient ID {patientId}.");
                    return NotFound($"No doctor found for patient ID {patientId}.");
                }

                return Ok(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching doctor for patient ID {patientId}.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}/patients", Name = "GetPatientsByDoctorId")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetDoctorPatients(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Invalid doctor ID provided.");
                    return BadRequest("Invalid doctor ID.");
                }

                _logger.LogInformation($"Fetching patients for doctor with ID {id}.");
                var patients = await _patientRepository.GetPatientsByDoctorIdAsync(id);

                if (patients == null)
                {
                    _logger.LogError("Patient repository returned null.");
                    return StatusCode(500, "Internal server error.");
                }

                if (!patients.Any())
                {
                    _logger.LogWarning($"No patients found for doctor with ID {id}.");
                    return NotFound($"No patients found for doctor with ID {id}.");
                }

                return Ok(patients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching patients for doctor ID {id}.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost(Name = "AddDoctor")]
        public async Task<ActionResult<Doctor>> AddDoctor([FromBody] Doctor doctor)
        {
            try
            {
                if (doctor == null)
                {
                    _logger.LogWarning("Doctor information is required.");
                    return BadRequest("Doctor information is required.");
                }

                doctor.ID = Guid.NewGuid();

                if (doctor.ID == Guid.Empty)
                {
                    _logger.LogWarning("Doctor ID is required.");
                    return BadRequest("Doctor ID is required.");
                }

                _logger.LogInformation($"Adding new doctor with ID {doctor.ID}.");
                await _doctorRepository.AddDoctorAsync(doctor);
                return CreatedAtAction(nameof(GetDoctor), new { id = doctor.ID }, doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new doctor.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{id}", Name = "UpdateDoctor")]
        public async Task<IActionResult> UpdateDoctor(Guid id, [FromBody] Doctor doctor)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Invalid doctor ID provided.");
                    return BadRequest("Invalid doctor ID.");
                }

                if (doctor == null)
                {
                    _logger.LogWarning("Doctor information is required.");
                    return BadRequest("Doctor information is required.");
                }

                if (id != doctor.ID)
                {
                    _logger.LogWarning("ID in the URL and body must match.");
                    return BadRequest("ID in the URL and body must match.");
                }

                _logger.LogInformation($"Updating doctor with ID {id}.");
                var existingDoctor = await _doctorRepository.GetDoctorByIdAsync(id);
                if (existingDoctor == null)
                {
                    _logger.LogWarning($"Doctor with ID {id} not found.");
                    return NotFound($"Doctor with ID {id} not found.");
                }

                await _doctorRepository.UpdateDoctorAsync(doctor);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating doctor with ID {id}.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{id}", Name = "DeleteDoctor")]
        public async Task<IActionResult> DeleteDoctor(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Invalid doctor ID provided.");
                    return BadRequest("Invalid doctor ID.");
                }

                _logger.LogInformation($"Deleting doctor with ID {id}.");
                var existingDoctor = await _doctorRepository.GetDoctorByIdAsync(id);
                if (existingDoctor == null)
                {
                    _logger.LogWarning($"Doctor with ID {id} not found.");
                    return NotFound($"Doctor with ID {id} not found.");
                }

                await _doctorRepository.DeleteDoctorAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting doctor with ID {id}.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}