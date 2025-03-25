using HQB.WebApi.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HQB.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TreatmentsController : ControllerBase
    {
        private readonly ITreatmentRepository _treatmentRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public TreatmentsController
        (
            ITreatmentRepository treatmentRepository,
            IAppointmentRepository appointmentRepository
        )
        {
            _appointmentRepository = appointmentRepository;
            _treatmentRepository = treatmentRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetTreatmentsAsync()
        {
            var treatments = await _treatmentRepository.GetAllTreatmentsAsync();
            return Ok(treatments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTreatmentAsync(Guid id)
        {
            var treatment = await _treatmentRepository.GetTreatmentByIdAsync(id);
            if (treatment == null)
            {
                return NotFound();
            }
            return Ok(treatment);
        }

        [HttpGet("{treatmentId}/appointments")]
        public IActionResult GetAppointmentsByTreatmentId(Guid treatmentId)
        {
            var appointments = _appointmentRepository.GetAppointmentByTreatmentIdAsync(treatmentId);
            if (appointments == null)
            {
                return NotFound();
            }
            return Ok(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTreatmentAsync([FromBody] Treatment treatment)
        {
            if (treatment == null)
            {
                return BadRequest();
            }

            var newTreatment = new Treatment
            {
                ID = Guid.NewGuid(),
                Name = treatment.Name,
            };

            var result = await _treatmentRepository.AddTreatmentAsync(treatment);
            return CreatedAtAction(nameof(GetTreatmentAsync), new { id = newTreatment.ID }, newTreatment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTreatmentAsync(Guid id, [FromBody] Treatment treatment)
        {
            if (treatment == null)
            {
                return BadRequest();
            }
            var existingTreatment = await _treatmentRepository.GetTreatmentByIdAsync(id);
            if (existingTreatment == null)
            {
                return NotFound();
            }

            treatment.ID = id;

            var result = await _treatmentRepository.UpdateTreatmentAsync(treatment);
            return Ok(treatment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTreatmentAsync(Guid id)
        {
            var existingTreatment = await _treatmentRepository.GetTreatmentByIdAsync(id);
            if (existingTreatment == null)
            {
                return NotFound();
            }
            await _treatmentRepository.DeleteTreatmentAsync(id);
            return NoContent();
        }
    }
}
