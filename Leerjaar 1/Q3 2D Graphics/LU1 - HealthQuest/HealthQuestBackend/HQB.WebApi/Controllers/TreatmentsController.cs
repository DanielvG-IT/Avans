using HQB.WebApi.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

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
            _appointmentRepository = appointmentRepository;
            _treatmentRepository = treatmentRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetTreatmentsAsync()
        {
            _logger.LogInformation("Getting all treatments");
            var treatments = await _treatmentRepository.GetAllTreatmentsAsync();
            return Ok(treatments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTreatmentAsync(Guid id)
        {
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
        public IActionResult GetAppointmentsByTreatmentId(Guid treatmentId)
        {
            _logger.LogInformation("Getting appointments for treatment ID: {TreatmentId}", treatmentId);
            var appointments = _appointmentRepository.GetAppointmentByTreatmentIdAsync(treatmentId);
            if (appointments == null)
            {
                _logger.LogWarning("Appointments for treatment ID: {TreatmentId} not found", treatmentId);
                return NotFound();
            }
            return Ok(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTreatmentAsync([FromBody] Treatment treatment)
        {
            if (treatment == null)
            {
                _logger.LogWarning("Treatment object is null");
                return BadRequest();
            }

            var newTreatment = new Treatment
            {
                ID = Guid.NewGuid(),
                Name = treatment.Name,
            };

            _logger.LogInformation("Creating new treatment with ID: {Id}", newTreatment.ID);
            var result = await _treatmentRepository.AddTreatmentAsync(treatment);
            return CreatedAtAction(nameof(GetTreatmentAsync), new { id = newTreatment.ID }, newTreatment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTreatmentAsync(Guid id, [FromBody] Treatment treatment)
        {
            if (treatment == null)
            {
                _logger.LogWarning("Treatment object is null");
                return BadRequest();
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
            return Ok(treatment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTreatmentAsync(Guid id)
        {
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
