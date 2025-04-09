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
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly IPersonalAppointmentsRepository _personalAppointmentsRepository;

        public PatientController(
            ILogger<PatientController> logger,
            IDoctorRepository doctorRepository,
            IPatientRepository patientRepository,
            IJournalRepository journalRepository,
            IStickersRepository stickersRepository,
            IGuardianRepository guardianRepository,
            ITreatmentRepository treatmentRepository,
            IAppointmentRepository appointmentRepository,
            IAuthenticationService authenticationService,
            IPersonalAppointmentsRepository personalAppointmentsRepository
        )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _doctorRepository = doctorRepository ?? throw new ArgumentNullException(nameof(doctorRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _journalRepository = journalRepository ?? throw new ArgumentNullException(nameof(journalRepository));
            _stickersRepository = stickersRepository ?? throw new ArgumentNullException(nameof(stickersRepository));
            _guardianRepository = guardianRepository ?? throw new ArgumentNullException(nameof(guardianRepository));
            _treatmentRepository = treatmentRepository ?? throw new ArgumentNullException(nameof(treatmentRepository));
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _personalAppointmentsRepository = personalAppointmentsRepository ?? throw new ArgumentNullException(nameof(personalAppointmentsRepository));
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
                if (guardian is null)
                {
                    _logger.LogWarning("Guardian not found for the logged-in user with ID {UserId}.", loggedInUserId);
                    return NotFound($"Guardian not found for the logged-in user with ID {loggedInUserId}. Please ensure your account is correctly linked to a guardian.");
                }

                _logger.LogInformation("Fetching all patients associated with the guardian for user ID {UserId}.", loggedInUserId);

                var patients = await _patientRepository.GetPatientsByGuardianId(guardian.ID);
                if (patients is null || !patients.Any())
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
                if (patient is null)
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
                if (patient is null)
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
                    ? (patient.GuardianID.HasValue ? await _guardianRepository.GetGuardianByIdAsync(patient.GuardianID.Value) : null)
                    : await _guardianRepository.GetGuardianByUserIdAsync(loggedInUserId);

                if (guardian is null)
                {
                    _logger.LogWarning("Guardian with ID {GuardianId} not found", patient.GuardianID);
                    return BadRequest($"Guardian with ID {patient.GuardianID} not found");
                }
                patient.GuardianID = guardian.ID;

                if (patient.DoctorID != Guid.Empty)
                {
                    var doctor = patient.DoctorID.HasValue ? await _doctorRepository.GetDoctorByIdAsync(patient.DoctorID.Value) : null;
                    if (doctor is null)
                    {
                        _logger.LogWarning("Doctor with ID {DoctorId} not found", patient.DoctorID);
                        return BadRequest($"Doctor with ID {patient.DoctorID} not found");
                    }
                }

                if (patient.TreatmentID != Guid.Empty)
                {
                    var treatment = patient.TreatmentID.HasValue ? await _treatmentRepository.GetTreatmentByIdAsync(patient.TreatmentID.Value) : null;
                    if (treatment is null)
                    {
                        _logger.LogWarning("Treatment with ID {TreatmentId} not found", patient.TreatmentID);
                        return BadRequest($"Treatment with ID {patient.TreatmentID} not found");
                    }
                }

                if (patient.Avatar is null)
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

                if (patient is null)
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
                    if (guardian is null)
                    {
                        _logger.LogWarning("Guardian not found for the logged-in user with ID {UserId}.", loggedInUserId);
                        return BadRequest($"Guardian not found for the logged-in user with ID {loggedInUserId}. Please ensure your account is correctly linked to a guardian.");
                    }

                    patient.GuardianID = guardian.ID;
                }
                else
                {
                    var guardian = patient.GuardianID.HasValue ? await _guardianRepository.GetGuardianByIdAsync(patient.GuardianID.Value) : null;
                    if (guardian is null)
                    {
                        _logger.LogWarning("Guardian with ID {GuardianId} not found", patient.GuardianID);
                        return BadRequest($"Guardian with ID {patient.GuardianID} not found");
                    }
                }

                if (patient.DoctorID != Guid.Empty)
                {
                    var doctor = patient.DoctorID.HasValue
                        ? await _doctorRepository.GetDoctorByIdAsync(patient.DoctorID.Value)
                        : null;
                    if (doctor is null)
                    {
                        _logger.LogWarning("Doctor with ID {DoctorId} not found", patient.DoctorID);
                        return BadRequest($"Doctor with ID {patient.DoctorID} not found");
                    }
                }

                if (patient.TreatmentID != Guid.Empty)
                {
                    if (patient.TreatmentID.HasValue)
                    {
                        var treatment = await _treatmentRepository.GetTreatmentByIdAsync(patient.TreatmentID.Value);
                        if (treatment is null)
                        {
                            _logger.LogWarning("Treatment with ID {TreatmentId} not found", patient.TreatmentID);
                            return BadRequest($"Treatment with ID {patient.TreatmentID} not found");
                        }
                    }
                }

                _logger.LogInformation("Updating patient with ID {PatientId}", id);
                var existingPatient = await _patientRepository.GetPatientByIdAsync(id);
                if (existingPatient is null)
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
                if (existingPatient is null)
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
            var user = _authenticationService.GetCurrentAuthenticatedUserId();
            if (string.IsNullOrEmpty(user))
            {
                _logger.LogWarning("User ID is required but was not provided.");
                return Forbid("User ID is required but was not provided. Please ensure you are logged in.");
            }

            try
            {
                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Invalid patient ID");
                    return BadRequest("Invalid patient ID");
                }

                _logger.LogInformation("Getting patient with ID {PatientId}", id);
                var patient = await _patientRepository.GetPatientByIdAsync(id);
                if (patient is null)
                {
                    _logger.LogWarning("Patient with ID {PatientId} not found", id);
                    return NotFound();
                }

                var roles = await _authenticationService.GetCurrentAuthenticatedUserRoles();
                if (roles is not null && roles.Contains("Doctor") && patient.DoctorAccessJournal is false)
                {
                    return Forbid("Patient has blocked access to journal entries!");
                }

                _logger.LogInformation("Getting journal entries for patient with ID {PatientId}", id);
                var journalEntries = await _journalRepository.GetJournalEntriesByPatientIdAsync(id);
                if (journalEntries is null)
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

        [HttpGet("{id}/appointments", Name = "GetPersonalAppointments")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetPersonalAppointments(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    _logger.LogWarning("Invalid patient ID");
                    return BadRequest("Invalid patient ID");
                }

                _logger.LogInformation("Getting completed appointments for patient with ID {PatientId}", id);
                var personalAppointments = await _personalAppointmentsRepository.GetPersonalAppointmentsByPatientId(id);
                if (personalAppointments is null || !personalAppointments.Any())
                {
                    _logger.LogWarning("No completed appointments found for patient with ID {PatientId}", id);
                    return NotFound($"No completed appointments found for patient with ID {id}.");
                }

                return Ok(personalAppointments.OrderBy(x => x.Sequence).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching completed appointments.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("{id}/appointments", Name = "GeneratePersonalAppointments")]
        public async Task<IActionResult> GeneratePersonalAppointments(Guid id)
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
                if (patient is null)
                {
                    _logger.LogWarning("Patient with ID {PatientId} not found", id);
                    return NotFound();
                }

                _logger.LogInformation("Generating personal appointments for patient with ID {PatientId}", id);
                var existingAppointments = await _personalAppointmentsRepository.GetPersonalAppointmentsByPatientId(id);
                if (existingAppointments != null && existingAppointments.Any())
                {
                    _logger.LogWarning("Personal appointments already exist for patient with ID {PatientId}", id);
                    return Conflict($"Personal appointments already exist for patient with ID {id}.");
                }

                if (patient.TreatmentID is null || patient.TreatmentID == Guid.Empty || patient.ID is null || patient.ID == Guid.Empty)
                {
                    _logger.LogWarning("Patient with ID {PatientId} is not valid", id);
                    return BadRequest($"Patient with ID {id} is not valid.");
                }

                _logger.LogInformation("Generating personal appointments for patient with ID {PatientId}", id);
                var treatmentAppointments = await _appointmentRepository.GetAppointmentsByTreatmentIdAsync(patient.TreatmentID.Value);
                if (treatmentAppointments is null || !treatmentAppointments.Any())
                {
                    _logger.LogWarning("Appointments do not exist for treatment with ID {TreatmentID}", patient.TreatmentID);
                    return BadRequest($"Appointments do not exist for treatment with ID {patient.TreatmentID}.");
                }

                foreach (var appointment in treatmentAppointments)
                {
                    var personalAppointment = new PersonalAppointments
                    {
                        ID = Guid.NewGuid(),
                        PatientID = patient.ID.Value,
                        AppointmentID = appointment.ID,
                        AppointmentDate = null,
                        CompletedDate = null,
                        CompletedQuestion = false,
                        Sequence = appointment.Sequence
                    };
                    await _personalAppointmentsRepository.AddPersonalAppointment(personalAppointment);
                }

                _logger.LogInformation("Personal appointments generated successfully for patient with ID {PatientId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a completed appointment.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
        [HttpPut("{id}/appointments/{appointmentId}", Name = "UpdatePersonalAppointment")]
        public async Task<IActionResult> UpdatePersonalAppointment(Guid id, Guid appointmentId, [FromBody] PersonalAppointments personalAppointment)
        {
            try
            {
                // TODO: Fix mapping string date to DateTime (cuz Unity is shit)
                if (id == Guid.Empty || appointmentId == Guid.Empty)
                {
                    _logger.LogWarning("Invalid patient ID or appointment ID");
                    return BadRequest("Invalid patient ID or appointment ID");
                }

                if (personalAppointment is null)
                {
                    _logger.LogWarning("Personal appointment object is null");
                    return BadRequest("Personal appointment object is null");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for personal appointment");
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Updating personal appointment with ID {AppointmentId} for patient with ID {PatientId}", appointmentId, id);
                var existingAppointment = await _personalAppointmentsRepository.GetPersonalAppointmentById(appointmentId);
                if (existingAppointment is null)
                {
                    _logger.LogWarning("Personal appointment with ID {AppointmentId} not found", appointmentId);
                    return NotFound();
                }

                // Ensure the patient ID exists
                var patient = await _patientRepository.GetPatientByIdAsync(id);
                if (patient is null)
                {
                    _logger.LogWarning("Patient with ID {PatientId} not found", id);
                    return NotFound();
                }

                // Ensure the patient ID matches
                if (patient.ID != existingAppointment.PatientID)
                {
                    _logger.LogWarning("Patient ID {PatientId} does not match the personal appointment's patient ID {AppointmentPatientId}", id, existingAppointment.PatientID);
                    return BadRequest($"Patient ID {id} does not match the personal appointment's patient ID {existingAppointment.PatientID}");
                }

                // Ensure the appointment ID matches
                if (existingAppointment.AppointmentID != appointmentId)
                {
                    _logger.LogWarning("Appointment ID {AppointmentId} does not match the personal appointment's appointment ID {ExistingAppointmentId}", appointmentId, existingAppointment.AppointmentID);
                    return BadRequest($"Appointment ID {appointmentId} does not match the personal appointment's appointment ID {existingAppointment.AppointmentID}");
                }

                // Ensure ID's do not change during update
                personalAppointment.ID = existingAppointment.ID;
                personalAppointment.PatientID = existingAppointment.PatientID;
                personalAppointment.AppointmentID = existingAppointment.AppointmentID;

                var result = await _personalAppointmentsRepository.UpdatePersonalAppointment(personalAppointment);
                if (result == 0)
                {
                    _logger.LogError("Error updating personal appointment");
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error updating personal appointment");
                }

                _logger.LogInformation("Personal appointment with ID {AppointmentId} updated successfully for patient with ID {PatientId}", appointmentId, id);
                return Ok(personalAppointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the personal appointment.");
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
                if (stickers is null || !stickers.Any())
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

        [HttpPost("{id}/stickers", Name = "AddStickerToPatient")]
        public async Task<IActionResult> AddStickerToPatient(Guid id, [FromQuery] Guid stickerId)
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
                if (patient is null)
                {
                    _logger.LogWarning("Patient with ID {PatientId} not found", id);
                    return NotFound($"Patient with ID {id} not found.");
                }

                var sticker = await _stickersRepository.GetStickerByIdAsync(stickerId);
                if (sticker is null)
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

        [HttpDelete("{id}/stickers/{stickerId}", Name = "DeleteStickerFromPatient")]
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
                if (patient is null)
                {
                    _logger.LogWarning("Patient with ID {PatientId} not found", id);
                    return NotFound($"Patient with ID {id} not found.");
                }

                var sticker = await _stickersRepository.GetStickerByIdAsync(stickerId);
                if (sticker is null)
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
                if (patient is null)
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
