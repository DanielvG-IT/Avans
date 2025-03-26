using HQB.WebApi.Interfaces;
using HQB.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace HQB.WebApi.Controllers
{
    [ApiController]
    [Route("api/doctors")]
    public class DoctorsController : ControllerBase
    {
        private readonly ILogger<DoctorsController> _logger;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;

        public DoctorsController(ILogger<DoctorsController> logger, IDoctorRepository doctorRepository, IPatientRepository patientRepository)
        {
            _logger = logger;
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
        }

        /// <summary>
        /// Get all doctors
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetDoctors()
        {
            _logger.LogInformation("Fetching all doctors.");
            var doctors = await _doctorRepository.GetAllDoctorsAsync();

            if (!doctors.Any())
            {
                _logger.LogWarning("No doctors found.");
                return NotFound("No doctors found.");
            }

            return Ok(doctors);
        }

        /// <summary>
        /// Get a single doctor by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctor(Guid id)
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

        /// <summary>
        /// Get a doctor assigned to a specific patient
        /// </summary>
        [HttpGet("by-patient/{patientId}")]
        public async Task<IActionResult> GetDoctorByPatientId(Guid patientId)
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

            var doctor = await _doctorRepository.GetDoctorByIdAsync(patient.DoctorID);
            if (doctor == null)
            {
                _logger.LogWarning($"No doctor found for patient ID {patientId}.");
                return NotFound($"No doctor found for patient ID {patientId}.");
            }

            return Ok(doctor);
        }

        /// <summary>
        /// Get all patients assigned to a specific doctor
        /// </summary>
        [HttpGet("{id}/patients")]
        public async Task<IActionResult> GetDoctorPatients(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid doctor ID provided.");
                return BadRequest("Invalid doctor ID.");
            }

            _logger.LogInformation($"Fetching patients for doctor with ID {id}.");
            var patients = await _patientRepository.GetPatientsByDoctorIdAsync(id);
            return Ok(patients);
        }

        /// <summary>
        /// Add a new doctor
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddDoctor([FromBody] Doctor doctor)
        {
            if (doctor == null)
            {
                _logger.LogWarning("Doctor information is required.");
                return BadRequest("Doctor information is required.");
            }

            _logger.LogInformation($"Adding new doctor with ID {doctor.ID}.");
            await _doctorRepository.AddDoctorAsync(doctor);
            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.ID }, doctor);
        }

        /// <summary>
        /// Update an existing doctor
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(Guid id, [FromBody] Doctor doctor)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid doctor ID provided.");
                return BadRequest("Invalid doctor ID.");
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

        /// <summary>
        /// Delete a doctor
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(Guid id)
        {
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
    }
}