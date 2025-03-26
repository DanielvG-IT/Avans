using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

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
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {
            _logger.LogInformation("Getting all appointments");
            var appointments = await _appointmentRepository.GetAllAppointmentsAsync();
            if (appointments == null)
            {
                _logger.LogWarning("No appointments found");
                return NotFound();
            }
            return Ok(appointments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> GetAppointmentById(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid appointment ID: {AppointmentId}", id);
                return BadRequest("Invalid appointment ID");
            }

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
        public async Task<ActionResult<Appointment>> CreateAppointment([FromBody] Appointment appointment)
        {
            if (appointment == null)
            {
                _logger.LogWarning("Attempted to create a null appointment");
                return BadRequest("Appointment cannot be null");
            }

            if (string.IsNullOrWhiteSpace(appointment.Name))
            {
                _logger.LogWarning("Attempted to create an appointment with an empty name");
                return BadRequest("Appointment name cannot be empty");
            }

            appointment.ID = Guid.NewGuid();

            var result = await _appointmentRepository.AddAppointmentAsync(appointment);
            if (result == 0)
            {
                _logger.LogError("Failed to create appointment");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create appointment");
            }

            _logger.LogInformation("Created new appointment with ID: {AppointmentId}", appointment.ID);
            return CreatedAtAction(nameof(GetAppointmentById), new { id = appointment.ID }, appointment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAppointment(Guid id, [FromBody] Appointment appointment)
        {
            if (id == Guid.Empty || appointment == null || appointment.ID != id)
            {
                _logger.LogWarning("Invalid update attempt for appointment with ID: {AppointmentId}", id);
                return BadRequest("Invalid appointment data");
            }

            if (string.IsNullOrWhiteSpace(appointment.Name))
            {
                _logger.LogWarning("Attempted to update an appointment with an empty title");
                return BadRequest("Appointment title cannot be empty");
            }

            var existingAppointment = await _appointmentRepository.GetAppointmentByIdAsync(id);
            if (existingAppointment == null)
            {
                _logger.LogWarning("Appointment with ID: {AppointmentId} not found for update", id);
                return NotFound();
            }

            var result = await _appointmentRepository.UpdateAppointmentAsync(appointment);
            if (result == 0)
            {
                _logger.LogError("Failed to update appointment with ID: {AppointmentId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update appointment");
            }

            _logger.LogInformation("Updated appointment with ID: {AppointmentId}", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid appointment ID: {AppointmentId}", id);
                return BadRequest("Invalid appointment ID");
            }

            _logger.LogInformation("Deleting appointment with ID: {AppointmentId}", id);
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(id);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment with ID: {AppointmentId} not found for deletion", id);
                return NotFound();
            }

            var result = await _appointmentRepository.DeleteAppointmentAsync(id);
            if (result == 0)
            {
                _logger.LogError("Failed to delete appointment with ID: {AppointmentId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to delete appointment");
            }

            _logger.LogInformation("Deleted appointment with ID: {AppointmentId}", id);
            return NoContent();
        }
    }
}