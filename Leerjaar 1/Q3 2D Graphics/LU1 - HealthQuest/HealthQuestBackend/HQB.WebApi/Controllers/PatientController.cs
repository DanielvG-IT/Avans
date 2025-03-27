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
    public class PatientController : ControllerBase
    {
        private readonly ILogger<PatientController> _logger;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IJournalRepository _journalRepository;
        private readonly IStickersRepository _stickersRepository;
        private readonly IGuardianRepository _guardianRepository;
        private readonly ITreatmentRepository _treatmentRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICompletedAppointmentsRepository _completedAppointmentsRepository;

        public PatientController(
            ILogger<PatientController> logger,
            IDoctorRepository doctorRepository,
            IPatientRepository patientRepository,
            IJournalRepository journalRepository,
            IStickersRepository stickersRepository,
            IGuardianRepository guardianRepository,
            ITreatmentRepository treatmentRepository,
            IAuthenticationService authenticationService,
            ICompletedAppointmentsRepository completedAppointmentsRepository
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _doctorRepository = doctorRepository ?? throw new ArgumentNullException(nameof(doctorRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _journalRepository = journalRepository ?? throw new ArgumentNullException(nameof(journalRepository));
            _stickersRepository = stickersRepository ?? throw new ArgumentNullException(nameof(stickersRepository));
            _guardianRepository = guardianRepository ?? throw new ArgumentNullException(nameof(guardianRepository));
            _treatmentRepository = treatmentRepository ?? throw new ArgumentNullException(nameof(treatmentRepository));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _completedAppointmentsRepository = completedAppointmentsRepository ?? throw new ArgumentNullException(nameof(completedAppointmentsRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAllPatients()
        {
            _logger.LogInformation("Getting all patients");
            var patients = await _patientRepository.GetAllPatientsAsync();
            if (patients == null)
            {
                _logger.LogWarning("No patients found");
                return NotFound();
            }
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatientById(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid patient ID");
                return BadRequest("Invalid patient ID");
            }

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
        public async Task<ActionResult<Patient>> AddPatient([FromBody] Patient patient)
        {
            if (patient == null)
            {
                _logger.LogWarning("Patient object is null");
                return BadRequest("Patient object is null");
            }

            patient.ID = Guid.NewGuid();

            var loggedInUserId = _authenticationService.GetCurrentAuthenticatedUserId();

            if (string.IsNullOrEmpty(loggedInUserId))
            {
                _logger.LogWarning("User ID is required");
                return BadRequest("User ID is required");
            }

            // Auto-link to guardian
            /// Retrieves the guardian information for the patient. 
            /// If the patient's GuardianID is not empty, fetches the guardian by their ID. 
            /// Otherwise, fetches the guardian associated with the logged-in user's ID.
            var guardian = patient.GuardianID != Guid.Empty
                ? await _guardianRepository.GetGuardianByIdAsync(patient.GuardianID)
                : await _guardianRepository.GetGuardianByUserIdAsync(loggedInUserId);

            if (guardian == null)
            {
                _logger.LogWarning("Guardian with ID {GuardianId} not found", patient.GuardianID);
                return BadRequest($"Guardian with ID {patient.GuardianID} not found");
            }
            patient.GuardianID = guardian.ID;

            if (patient.DoctorID != Guid.Empty)
            {
                var doctor = await _doctorRepository.GetDoctorByIdAsync(patient.DoctorID);
                if (doctor == null)
                {
                    _logger.LogWarning("Doctor with ID {DoctorId} not found", patient.DoctorID);
                    return BadRequest($"Doctor with ID {patient.DoctorID} not found");
                }
            }

            if (patient.TreatmentID != Guid.Empty)
            {
                var treatment = await _treatmentRepository.GetTreatmentByIdAsync(patient.TreatmentID);
                if (treatment == null)
                {
                    _logger.LogWarning("Treatment with ID {TreatmentId} not found", patient.TreatmentID);
                    return BadRequest($"Treatment with ID {patient.TreatmentID} not found");
                }
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for patient");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Adding new patient");
            var result = await _patientRepository.AddPatientAsync(patient);
            if (result == 0)
            {
                _logger.LogError("Error adding patient");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error adding patient");
            }
            return CreatedAtAction(nameof(GetPatientById), new { id = patient.ID }, patient);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(Guid id, [FromBody] Patient patient)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid patient ID");
                return BadRequest("Invalid patient ID");
            }

            if (patient == null)
            {
                _logger.LogWarning("Patient object is null");
                return BadRequest("Patient object is null");
            }

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
            var result = await _patientRepository.UpdatePatientAsync(patient);
            if (result == 0)
            {
                _logger.LogError("Error updating patient");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating patient");
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid patient ID");
                return BadRequest("Invalid patient ID");
            }

            _logger.LogInformation("Deleting patient with ID {PatientId}", id);
            var existingPatient = await _patientRepository.GetPatientByIdAsync(id);
            if (existingPatient == null)
            {
                _logger.LogWarning("Patient with ID {PatientId} not found", id);
                return NotFound();
            }

            var result = await _patientRepository.DeletePatientAsync(id);
            if (result == 0)
            {
                _logger.LogError("Error deleting patient");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting patient");
            }
            return NoContent();
        }

        [HttpGet("{id}/journal-entries")]
        public async Task<ActionResult<IEnumerable<JournalEntry>>> GetJournalEntries(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid patient ID");
                return BadRequest("Invalid patient ID");
            }

            _logger.LogInformation("Getting journal entries for patient with ID {PatientId}", id);
            var journalEntries = await _journalRepository.GetJournalEntriesByPatientIdAsync(id);
            if (journalEntries == null)
            {
                _logger.LogWarning("Journal entries for patient with ID {PatientId} not found", id);
                return NotFound();
            }
            return Ok(journalEntries);
        }

        [HttpGet("{id}/completed-appointments")]
        public async Task<ActionResult<IEnumerable<CompletedAppointment>>> GetCompletedAppointments(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid patient ID");
                return BadRequest("Invalid patient ID");
            }

            _logger.LogInformation("Getting completed appointments for patient with ID {PatientId}", id);
            var completedAppointments = await _completedAppointmentsRepository.GetCompletedAppointmentsByPatientIdAsync(id);
            if (completedAppointments == null)
            {
                _logger.LogWarning("Completed appointments for patient with ID {PatientId} not found", id);
                return NotFound();
            }
            return Ok(completedAppointments);
        }

        [HttpGet("{id}/stickers")]
        public async Task<ActionResult<IEnumerable<Sticker>>> GetStickers(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid patient ID");
                return BadRequest("Invalid patient ID");
            }

            _logger.LogInformation("Getting stickers for patient with ID {PatientId}", id);
            var stickers = await _stickersRepository.GetUnlockedStickersByPatientId(id);
            if (stickers == null)
            {
                _logger.LogWarning("Stickers for patient with ID {PatientId} not found", id);
                return NotFound();
            }
            return Ok(stickers);
        }
    }
}
