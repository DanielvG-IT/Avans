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
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(IAppointmentRepository appointmentRepository, ILogger<AppointmentsController> logger)
        {
            _appointmentRepository = appointmentRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAppointments()
        {
            _logger.LogInformation("Getting all appointments");
            var appointments = await _appointmentRepository.GetAllAppointmentsAsync();
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(Guid id)
        {
            _logger.LogInformation("Getting appointment with ID: {AppointmentId}", id);
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment with ID: {AppointmentId} not found", id);
                return NotFound();
            }
            return Ok(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] Appointment appointment)
        {
            if (appointment == null)
            {
                _logger.LogWarning("Attempted to create a null appointment");
                return BadRequest();
            }

            appointment.ID = Guid.NewGuid();

            var result = await _appointmentRepository.AddAppointmentAsync(appointment);
            _logger.LogInformation("Created new appointment with ID: {AppointmentId}", appointment.ID);
            return CreatedAtAction(nameof(GetAppointments), new { id = appointment.ID }, appointment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(Guid id, [FromBody] Appointment appointment)
        {
            if (appointment == null || appointment.ID != id)
            {
                _logger.LogWarning("Invalid update attempt for appointment with ID: {AppointmentId}", id);
                return BadRequest();
            }
            var existingAppointment = await _appointmentRepository.GetAppointmentByIdAsync(id);
            if (existingAppointment == null)
            {
                _logger.LogWarning("Appointment with ID: {AppointmentId} not found for update", id);
                return NotFound();
            }
            await _appointmentRepository.UpdateAppointmentAsync(appointment);
            _logger.LogInformation("Updated appointment with ID: {AppointmentId}", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(Guid id)
        {
            _logger.LogInformation("Deleting appointment with ID: {AppointmentId}", id);
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment with ID: {AppointmentId} not found for deletion", id);
                return NotFound();
            }
            await _appointmentRepository.DeleteAppointmentAsync(id);
            _logger.LogInformation("Deleted appointment with ID: {AppointmentId}", id);
            return NoContent();
        }
    }
}