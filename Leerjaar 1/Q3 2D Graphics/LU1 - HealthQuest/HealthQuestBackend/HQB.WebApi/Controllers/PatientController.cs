using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HQB.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<PatientController> _logger;

        public PatientController(IPatientRepository patientRepository, ILogger<PatientController> logger)
        {
            _patientRepository = patientRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPatients()
        {
            _logger.LogInformation("Getting all patients");
            var patients = await _patientRepository.GetAllPatientsAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            _logger.LogInformation("Getting patient with ID {PatientId}", id);
            var patient = await _patientRepository.GetPatientByIdAsync(id);
            if (patient == null)
            {
                _logger.LogWarning("Patient with ID {PatientId} not found", id);
                return NotFound();
            }
            return Ok(patient);
        }

        [HttpPost]
        public async Task<IActionResult> AddPatient([FromBody] Patient patient)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for patient");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Adding new patient");
            var result = await _patientRepository.AddPatientAsync(patient);
            return CreatedAtAction(nameof(GetPatientById), new { id = patient.ID }, patient);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] Patient patient)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for patient");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating patient with ID {PatientId}", id);
            var existingPatient = await _patientRepository.GetPatientByIdAsync(id);
            if (existingPatient == null)
            {
                _logger.LogWarning("Patient with ID {PatientId} not found", id);
                return NotFound();
            }

            patient.ID = existingPatient.ID;
            await _patientRepository.UpdatePatientAsync(patient);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            _logger.LogInformation("Deleting patient with ID {PatientId}", id);
            var existingPatient = await _patientRepository.GetPatientByIdAsync(id);
            if (existingPatient == null)
            {
                _logger.LogWarning("Patient with ID {PatientId} not found", id);
                return NotFound();
            }

            await _patientRepository.DeletePatientAsync(id);
            return NoContent();
        }
    }
}
