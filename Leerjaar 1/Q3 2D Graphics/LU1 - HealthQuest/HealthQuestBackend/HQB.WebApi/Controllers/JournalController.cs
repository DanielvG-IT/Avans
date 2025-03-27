using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HQB.WebApi.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class JournalController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IGuardianRepository _guardianRepository;
    private readonly IJournalRepository _journalRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly ILogger<JournalController> _logger;

    public JournalController(ILogger<JournalController> logger, IJournalRepository journalRepository, IAuthenticationService authenticationService, IGuardianRepository guardianRepository, IPatientRepository patientRepository)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _guardianRepository = guardianRepository ?? throw new ArgumentNullException(nameof(guardianRepository));
        _journalRepository = journalRepository ?? throw new ArgumentNullException(nameof(journalRepository));
        _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JournalEntry>>> GetJournals(Guid? guardianId, Guid? patientId)
    {
        if (patientId != null)
        {
            _logger.LogInformation("Getting journal entries for PatientID: {PatientId}", patientId);
            var journals = await _journalRepository.GetJournalEntriesByPatientIdAsync(patientId.Value);
            if (journals == null || !journals.Any())
            {
                _logger.LogWarning("No journal entries found for PatientID: {PatientId}", patientId);
                return NotFound();
            }
            return Ok(journals);
        }
        else if (guardianId != null)
        {
            _logger.LogInformation("Getting journal entries for GuardianID: {GuardianId}", guardianId);
            var journals = await _journalRepository.GetJournalEntriesByGuardianIdAsync(guardianId.Value);
            if (journals == null || !journals.Any())
            {
                _logger.LogWarning("No journal entries found for GuardianID: {GuardianId}", guardianId);
                return NotFound();
            }
            return Ok(journals);
        }
        else
        {
            var loggedInUserId = _authenticationService.GetCurrentAuthenticatedUserId(); // Assuming the logged-in user's ID is stored in the Name claim
            if (string.IsNullOrEmpty(loggedInUserId))
            {
                _logger.LogWarning("Unable to determine logged-in user ID");
                return BadRequest("Unable to determine logged-in user ID");
            }

            _logger.LogInformation("Fetching guardian ID for logged-in user: {LoggedInUserId}", loggedInUserId);
            var guardian = await _guardianRepository.GetGuardianByUserIdAsync(loggedInUserId);

            if (guardian?.ID == null)
            {
                _logger.LogWarning("No guardian ID found for logged-in user: {LoggedInUserId}", loggedInUserId);
                return NotFound("Guardian ID not found for logged-in user");
            }

            guardianId = guardian.ID;
            _logger.LogInformation("Getting journal entries for GuardianID: {GuardianId}", guardianId);
            var journals = await _journalRepository.GetJournalEntriesByGuardianIdAsync(guardianId.Value);

            if (journals == null || !journals.Any())
            {
                _logger.LogWarning("No journal entries found for GuardianID: {GuardianId}", guardianId);
                return NotFound("No journal entries found.");
            }

            return Ok(journals);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JournalEntry>> GetJournal(Guid id)
    {
        if (id == Guid.Empty)
        {
            _logger.LogWarning("Invalid journal entry ID: {Id}", id);
            return BadRequest("Invalid ID");
        }

        var loggedInUserId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(loggedInUserId))
        {
            _logger.LogWarning("Unable to determine logged-in user ID");
            return BadRequest("Unable to determine logged-in user ID");
        }

        var guardian = await _guardianRepository.GetGuardianByUserIdAsync(loggedInUserId);
        if (guardian?.ID == null)
        {
            _logger.LogWarning("No guardian ID found for logged-in user: {LoggedInUserId}", loggedInUserId);
            return Forbid("You do not have permission to view this journal entry.");
        }

        _logger.LogInformation("Getting journal entry with ID: {Id}", id);
        var journal = await _journalRepository.GetJournalEntryByIdAsync(id);
        if (journal == null)
        {
            _logger.LogWarning("Journal entry with ID: {Id} not found", id);
            return NotFound();
        }

        if (journal.GuardianID != guardian.ID)
        {
            _logger.LogWarning("User does not own the journal entry with ID: {Id}", id);
            return Forbid("You do not have permission to view this journal entry.");
        }

        return Ok(journal);
    }

    [HttpPost]
    public async Task<ActionResult<JournalEntry>> PostJournal(JournalEntry journal)
    {
        if (journal == null)
        {
            _logger.LogWarning("Journal entry is null");
            return BadRequest("Journal entry cannot be null");
        }

        journal.ID = Guid.NewGuid();

        _logger.LogInformation("Creating a new journal entry");
        await _journalRepository.AddJournalEntryAsync(journal);
        return CreatedAtAction("GetJournal", new { id = journal.ID }, journal);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutJournal(Guid id, JournalEntry journal)
    {
        if (id == Guid.Empty || journal == null)
        {
            _logger.LogWarning("Invalid input: ID: {Id}, Journal: {Journal}", id, journal);
            return BadRequest("Invalid input");
        }

        if (id != journal.ID)
        {
            _logger.LogWarning("Journal entry ID mismatch: {Id} != {JournalId}", id, journal.ID);
            return BadRequest("ID mismatch");
        }

        var loggedInUserId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(loggedInUserId))
        {
            _logger.LogWarning("Unable to determine logged-in user ID");
            return BadRequest("Unable to determine logged-in user ID");
        }

        var guardian = await _guardianRepository.GetGuardianByUserIdAsync(loggedInUserId);
        if (guardian?.ID == null)
        {
            _logger.LogWarning("No guardian ID found for logged-in user: {LoggedInUserId}", loggedInUserId);
            return Forbid("You do not have permission to modify this journal entry.");
        }

        var existingJournal = await _journalRepository.GetJournalEntryByIdAsync(id);
        if (existingJournal == null)
        {
            _logger.LogWarning("Journal entry with ID: {Id} not found", id);
            return NotFound("Journal entry not found.");
        }

        if (existingJournal.GuardianID != guardian.ID)
        {
            _logger.LogWarning("User does not own the journal entry with ID: {Id}", id);
            return Forbid("You do not have permission to modify this journal entry.");
        }

        _logger.LogInformation("Updating journal entry with ID: {Id}", id);
        await _journalRepository.UpdateJournalEntryAsync(journal);
        return Ok(journal);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJournal(Guid id)
    {
        if (id == Guid.Empty)
        {
            _logger.LogWarning("Invalid journal entry ID: {Id}", id);
            return BadRequest("Invalid ID");
        }

        var loggedInUserId = _authenticationService.GetCurrentAuthenticatedUserId();
        if (string.IsNullOrEmpty(loggedInUserId))
        {
            _logger.LogWarning("Unable to determine logged-in user ID");
            return BadRequest("Unable to determine logged-in user ID");
        }

        var guardian = await _guardianRepository.GetGuardianByUserIdAsync(loggedInUserId);
        if (guardian?.ID == null)
        {
            _logger.LogWarning("No guardian ID found for logged-in user: {LoggedInUserId}", loggedInUserId);
            return Forbid("You do not have permission to delete this journal entry.");
        }

        var existingJournal = await _journalRepository.GetJournalEntryByIdAsync(id);
        if (existingJournal == null)
        {
            _logger.LogWarning("Journal entry with ID: {Id} not found", id);
            return NotFound("Journal entry not found.");
        }

        if (existingJournal.GuardianID != guardian.ID)
        {
            _logger.LogWarning("User does not own the journal entry with ID: {Id}", id);
            return Forbid("You do not have permission to delete this journal entry.");
        }

        _logger.LogInformation("Deleting journal entry with ID: {Id}", id);
        await _journalRepository.DeleteJournalEntryAsync(id);
        return NoContent();
    }
}