using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace HQB.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TreatmentsController : ControllerBase
    {
        private readonly ITreatmentRepository _treatmentRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ILogger<TreatmentsController> _logger;

        public TreatmentsController
        (
            ITreatmentRepository treatmentRepository,
            IAppointmentRepository appointmentRepository,
            ILogger<TreatmentsController> logger
        )
        {
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
            _treatmentRepository = treatmentRepository ?? throw new ArgumentNullException(nameof(treatmentRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Treatment>>> GetTreatmentsAsync()
        {
            _logger.LogInformation("Getting all treatments");
            var treatments = await _treatmentRepository.GetAllTreatmentsAsync();
            if (treatments == null || !treatments.Any())
            {
                _logger.LogWarning("No treatments found");
                return NotFound();
            }
            return Ok(treatments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Treatment>> GetTreatmentAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid treatment ID");
                return BadRequest("Invalid treatment ID");
            }

            _logger.LogInformation("Getting treatment with ID: {Id}", id);
            var treatment = await _treatmentRepository.GetTreatmentByIdAsync(id);
            if (treatment == null)
            {
                _logger.LogWarning("Treatment with ID: {Id} not found", id);
                return NotFound();
            }
            return Ok(treatment);
        }

        [HttpGet("{treatmentId}/appointments")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentByTreatmentId(Guid treatmentId)
        {
            if (treatmentId == Guid.Empty)
            {
                _logger.LogWarning("Invalid treatment ID");
                return BadRequest("Invalid treatment ID");
            }

            _logger.LogInformation("Getting appointments for treatment ID: {TreatmentId}", treatmentId);
            var appointment = await _appointmentRepository.GetAppointmentByTreatmentIdAsync(treatmentId);
            if (appointment == null)
            {
                _logger.LogWarning("Appointments for treatment ID: {TreatmentId} not found", treatmentId);
                return NotFound();
            }
            return Ok(appointment);
        }

        [HttpPost]
        public async Task<ActionResult<Treatment>> CreateTreatmentAsync([FromBody] Treatment treatment)
        {
            if (treatment == null)
            {
                _logger.LogWarning("Treatment object is null.");
                return BadRequest("Treatment object is null.");
            }

            if (string.IsNullOrWhiteSpace(treatment.Name))
            {
                _logger.LogWarning("Treatment name is null or empty");
                return BadRequest("Treatment name is required");
            }

            var newTreatment = new Treatment
            {
                ID = Guid.NewGuid(),
                Name = treatment.Name,
            };

            _logger.LogInformation("Creating new treatment with ID: {Id}", newTreatment.ID);
            var result = await _treatmentRepository.AddTreatmentAsync(newTreatment);
            if (result == 0)
            {
                _logger.LogError("Error creating treatment");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating treatment");
            }
            return CreatedAtAction(nameof(GetTreatmentAsync), new { id = newTreatment.ID }, newTreatment);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Treatment>> UpdateTreatmentAsync(Guid id, [FromBody] Treatment treatment)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid treatment ID");
                return BadRequest("Invalid treatment ID");
            }

            if (treatment == null)
            {
                _logger.LogWarning("Treatment object is null");
                return BadRequest("Treatment object is null");
            }

            if (string.IsNullOrWhiteSpace(treatment.Name))
            {
                _logger.LogWarning("Treatment name is null or empty");
                return BadRequest("Treatment name is required");
            }

            var existingTreatment = await _treatmentRepository.GetTreatmentByIdAsync(id);
            if (existingTreatment == null)
            {
                _logger.LogWarning("Treatment with ID: {Id} not found", id);
                return NotFound();
            }

            treatment.ID = id;

            _logger.LogInformation("Updating treatment with ID: {Id}", id);
            var result = await _treatmentRepository.UpdateTreatmentAsync(treatment);
            if (result == 0)
            {
                _logger.LogError("Error updating treatment");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating treatment");
            }
            return Ok(treatment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTreatmentAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid treatment ID");
                return BadRequest("Invalid treatment ID");
            }

            _logger.LogInformation("Deleting treatment with ID: {Id}", id);
            var existingTreatment = await _treatmentRepository.GetTreatmentByIdAsync(id);
            if (existingTreatment == null)
            {
                _logger.LogWarning("Treatment with ID: {Id} not found", id);
                return NotFound();
            }
            await _treatmentRepository.DeleteTreatmentAsync(id);
            return NoContent();
        }
    }
}
