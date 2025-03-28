using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet(Name = "GetPatientsForCurrentUser")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatientsForCurrentUser()
        {
            try
            {
                var loggedInUserId = _authenticationService.GetCurrentAuthenticatedUserId();
                if (string.IsNullOrEmpty(loggedInUserId))
                {
                    _logger.LogWarning("User ID is required but was not provided.");
                    return BadRequest("User ID is required but was not provided. Please ensure you are logged in.");
                }

                var guardian = await _guardianRepository.GetGuardianByUserIdAsync(loggedInUserId);
                if (guardian == null)
                {
                    _logger.LogWarning("Guardian not found for the logged-in user with ID {UserId}.", loggedInUserId);
                    return NotFound($"Guardian not found for the logged-in user with ID {loggedInUserId}. Please ensure your account is correctly linked to a guardian.");
                }

                _logger.LogInformation("Fetching all patients associated with the guardian for user ID {UserId}.", loggedInUserId);

                var patients = await _patientRepository.GetPatientsByGuardianId(guardian.ID);
                if (patients == null || !patients.Any())
                {
                    _logger.LogWarning("No patients found for the guardian with ID {GuardianId} associated with user ID {UserId}.", guardian.ID, loggedInUserId);
                    return NotFound($"No patients found for the guardian with ID {guardian.ID} associated with your account. Please ensure patients are correctly linked to your guardian.");
                }

                return Ok(patients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching patients for the current user.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}", Name = "GetPatientById")]
        public async Task<ActionResult<Patient>> GetPatientById(Guid id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the patient by ID.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost(Name = "AddPatient")]
        public async Task<ActionResult<Patient>> AddPatient([FromBody] Patient patient)
        {
            try
            {
                if (patient == null)
                {
                    _logger.LogWarning("Patient object is null");
                    return BadRequest("Patient object is null");
                }

                if (string.IsNullOrWhiteSpace(patient.FirstName) || string.IsNullOrWhiteSpace(patient.LastName))
                {
                    _logger.LogWarning("Patient's first name or last name is missing");
                    return BadRequest("Patient's first name and last name are required");
                }

                patient.ID = Guid.NewGuid();

                var loggedInUserId = _authenticationService.GetCurrentAuthenticatedUserId();

                if (string.IsNullOrEmpty(loggedInUserId))
                {
                    _logger.LogWarning("User ID is required but was not provided");
                    return BadRequest("User ID is required but was not provided");
                }

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

                if (patient.Avatar == null)
                {
                    patient.Avatar ??= "default_avatar.png";
                }

                if (patient.ID == Guid.Empty || patient.GuardianID == Guid.Empty)
                {
                    _logger.LogWarning("Patient ID or Guardian ID is required but was not provided");
                    return BadRequest("Patient ID or Guardian ID is required but was not provided");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for patient");
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Adding new patient");
                await _patientRepository.AddPatientAsync(patient);
                return CreatedAtAction(nameof(GetPatientById), new { id = patient.ID }, patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new patient.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{id}", Name = "UpdatePatient")]
        public async Task<IActionResult> UpdatePatient(Guid id, [FromBody] Patient patient)
        {
            try
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

                if (patient.GuardianID == Guid.Empty)
                {
                    var loggedInUserId = _authenticationService.GetCurrentAuthenticatedUserId();
                    if (string.IsNullOrEmpty(loggedInUserId))
                    {
                        _logger.LogWarning("User ID is required but was not provided.");
                        return BadRequest("User ID is required but was not provided. Please ensure you are logged in.");
                    }

                    var guardian = await _guardianRepository.GetGuardianByUserIdAsync(loggedInUserId);
                    if (guardian == null)
                    {
                        _logger.LogWarning("Guardian not found for the logged-in user with ID {UserId}.", loggedInUserId);
                        return BadRequest($"Guardian not found for the logged-in user with ID {loggedInUserId}. Please ensure your account is correctly linked to a guardian.");
                    }

                    patient.GuardianID = guardian.ID;
                }
                else
                {
                    var guardian = await _guardianRepository.GetGuardianByIdAsync(patient.GuardianID);
                    if (guardian == null)
                    {
                        _logger.LogWarning("Guardian with ID {GuardianId} not found", patient.GuardianID);
                        return BadRequest($"Guardian with ID {patient.GuardianID} not found");
                    }
                }

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the patient.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{id}", Name = "DeletePatient")]
        public async Task<IActionResult> DeletePatient(Guid id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the patient.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}/journal-entries", Name = "GetJournalEntries")]
        public async Task<ActionResult<IEnumerable<JournalEntry>>> GetJournalEntries(Guid id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching journal entries.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}/completed-appointments", Name = "GetCompletedAppointments")]
        public async Task<ActionResult<IEnumerable<CompletedAppointment>>> GetCompletedAppointments(Guid id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching completed appointments.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}/stickers", Name = "GetStickers")]
        public async Task<ActionResult<IEnumerable<Sticker>>> GetStickers(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Invalid patient ID");
                    return BadRequest("Invalid patient ID");
                }

                _logger.LogInformation("Getting stickers for patient with ID {PatientId}", id);
                var stickers = await _stickersRepository.GetUnlockedStickersByPatientIdAsync(id);
                if (stickers == null || !stickers.Any())
                {
                    _logger.LogWarning("No stickers found for patient with ID {PatientId}", id);
                    return NotFound($"No stickers found for patient with ID {id}.");
                }
                return Ok(stickers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching stickers.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("{id}/stickers", Name = "AddSticker")]
        public async Task<IActionResult> AddStickerToPatient(Guid id, [FromBody] Guid stickerId)
        {
            if (id == Guid.Empty || stickerId == Guid.Empty)
            {
                _logger.LogWarning("Invalid patient ID or sticker ID");
                return BadRequest("Patient ID and Sticker ID must be valid non-empty GUIDs.");
            }

            try
            {
                _logger.LogInformation("Adding sticker with ID {StickerId} to patient with ID {PatientId}", stickerId, id);

                var patient = await _patientRepository.GetPatientByIdAsync(id);
                if (patient == null)
                {
                    _logger.LogWarning("Patient with ID {PatientId} not found", id);
                    return NotFound($"Patient with ID {id} not found.");
                }

                var sticker = await _stickersRepository.GetStickerByIdAsync(stickerId);
                if (sticker == null)
                {
                    _logger.LogWarning("Sticker with ID {StickerId} not found", stickerId);
                    return NotFound($"Sticker with ID {stickerId} not found.");
                }

                var isStickerUnlocked = await _stickersRepository.IsStickerUnlockedByPatientAsync(id, stickerId);
                if (isStickerUnlocked)
                {
                    _logger.LogWarning("Sticker with ID {StickerId} is already unlocked for patient with ID {PatientId}", stickerId, id);
                    return Conflict($"Sticker with ID {stickerId} is already unlocked for patient with ID {id}.");
                }

                await _stickersRepository.AddStickerToPatientAsync(id, stickerId);

                return CreatedAtAction(nameof(GetStickers), new { id }, sticker);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a sticker to the patient.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{id}/stickers/{stickerId}", Name = "DeleteSticker")]
        public async Task<IActionResult> DeleteSticker(Guid id, Guid stickerId)
        {
            if (id == Guid.Empty || stickerId == Guid.Empty)
            {
                _logger.LogWarning("Invalid patient ID or sticker ID");
                return BadRequest("Patient ID and Sticker ID must be valid non-empty GUIDs.");
            }

            try
            {
                _logger.LogInformation("Deleting sticker with ID {StickerId} for patient with ID {PatientId}", stickerId, id);

                var patient = await _patientRepository.GetPatientByIdAsync(id);
                if (patient == null)
                {
                    _logger.LogWarning("Patient with ID {PatientId} not found", id);
                    return NotFound($"Patient with ID {id} not found.");
                }

                var sticker = await _stickersRepository.GetStickerByIdAsync(stickerId);
                if (sticker == null)
                {
                    _logger.LogWarning("Sticker with ID {StickerId} not found", stickerId);
                    return NotFound($"Sticker with ID {stickerId} not found.");
                }

                var isStickerUnlocked = await _stickersRepository.IsStickerUnlockedByPatientAsync(id, stickerId);
                if (!isStickerUnlocked)
                {
                    _logger.LogWarning("Sticker with ID {StickerId} is not unlocked for patient with ID {PatientId}", stickerId, id);
                    return NotFound($"Sticker with ID {stickerId} is not unlocked for patient with ID {id}.");
                }

                await _stickersRepository.DeleteStickerFromPatientAsync(id, stickerId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the sticker from the patient.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}/stickers/{stickerId}/is-unlocked", Name = "IsStickerUnlockedByPatient")]
        public async Task<ActionResult<bool>> IsStickerUnlockedByPatient(Guid id, Guid stickerId)
        {
            if (id == Guid.Empty || stickerId == Guid.Empty)
            {
                _logger.LogWarning("Invalid patient ID or sticker ID");
                return BadRequest("Patient ID and Sticker ID must be valid non-empty GUIDs.");
            }

            try
            {
                _logger.LogInformation("Checking if sticker with ID {StickerId} is unlocked for patient with ID {PatientId}", stickerId, id);

                var patient = await _patientRepository.GetPatientByIdAsync(id);
                if (patient == null)
                {
                    _logger.LogWarning("Patient with ID {PatientId} not found", id);
                    return NotFound($"Patient with ID {id} not found.");
                }

                var isStickerUnlocked = await _stickersRepository.IsStickerUnlockedByPatientAsync(id, stickerId);
                return Ok(isStickerUnlocked);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if the sticker is unlocked for the patient.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
