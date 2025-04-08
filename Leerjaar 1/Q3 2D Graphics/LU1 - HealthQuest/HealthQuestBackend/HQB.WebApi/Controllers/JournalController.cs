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

    [HttpGet(Name = "GetJournals")]
    public async Task<ActionResult<IEnumerable<JournalEntry>>> GetJournals(Guid patientId)
    {
        try
        {
                _logger.LogInformation("Getting journal entries for PatientID: {PatientId}", patientId);
                var journals = await _journalRepository.GetJournalEntriesByPatientIdAsync(patientId);
                if (journals == null || !journals.Any())
                {
                    _logger.LogWarning("No journal entries found for PatientID: {PatientId}", patientId);
                    return NotFound();
                }
                return Ok(journals);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching journal entries");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("{id}", Name = "GetJournal")]
    public async Task<ActionResult<JournalEntry>> GetJournal(Guid id)
    {
        try
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
                return StatusCode(403, "You do not have permission to view this journal entry.");
            }

            _logger.LogInformation("Getting journal entry with ID: {Id}", id);
            var journal = await _journalRepository.GetJournalEntryByIdAsync(id);
            if (journal == null)
            {
                _logger.LogWarning("Journal entry with ID: {Id} not found", id);
                return NotFound();
            }

            return Ok(journal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the journal entry");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost(Name = "PostJournal")]
    public async Task<ActionResult<JournalEntry>> PostJournal(JournalEntry journal)
    {
        try
        {
            if (journal == null)
            {
                _logger.LogWarning("Journal entry is null");
                return BadRequest("Journal entry cannot be null");
            }

            if (journal.PatientID == null)
            {
                _logger.LogWarning("PatientID is required");
                return BadRequest("PatientID is required");
            }

            if (journal.Date == default)
            {
                _logger.LogWarning("Date is required");
                return BadRequest("Date is required");
            }

            if (string.IsNullOrWhiteSpace(journal.Title))
            {
                _logger.LogWarning("Title is required");
                return BadRequest("Title is required");
            }

            if (journal.Title.Length > 25)
            {
                _logger.LogWarning("Title exceeds maximum length of 25 characters");
                return BadRequest("Title exceeds maximum length of 25 characters");
            }

            if (string.IsNullOrWhiteSpace(journal.Content))
            {
                _logger.LogWarning("Content is required");
                return BadRequest("Content is required");
            }

            if (journal.Content.Length > 850)
            {
                _logger.LogWarning("Content exceeds maximum length of 850 characters");
                return BadRequest("Content exceeds maximum length of 850 characters");
            }

            if (journal.Rating < 1 || journal.Rating > 10)
            {
                _logger.LogWarning("Rating must be between 1 and 10");
                return BadRequest("Rating must be between 1 and 10");
            }

            journal.ID = Guid.NewGuid();

            _logger.LogInformation("Creating a new journal entry");
            await _journalRepository.AddJournalEntryAsync(journal);
            return CreatedAtAction("GetJournal", new { id = journal.ID }, journal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the journal entry");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPut("{id}", Name = "PutJournal")]
    public async Task<IActionResult> PutJournal(Guid id, JournalEntry journal)
    {
        try
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

                        if (journal.Date == default)
            {
                _logger.LogWarning("Date is required");
                return BadRequest("Date is required");
            }

            if (string.IsNullOrWhiteSpace(journal.Title))
            {
                _logger.LogWarning("Title is required");
                return BadRequest("Title is required");
            }

            if (string.IsNullOrWhiteSpace(journal.Content))
            {
                _logger.LogWarning("Content is required");
                return BadRequest("Content is required");
            }

            if (journal.Rating < 1 || journal.Rating > 10)
            {
                _logger.LogWarning("Rating must be between 1 and 10");
                return BadRequest("Rating must be between 1 and 10");
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
                return StatusCode(403, "You do not have permission to modify this journal entry.");
            }

            var existingJournal = await _journalRepository.GetJournalEntryByIdAsync(id);
            if (existingJournal == null)
            {
                _logger.LogWarning("Journal entry with ID: {Id} not found", id);
                return NotFound("Journal entry not found.");
            }

            _logger.LogInformation("Updating journal entry with ID: {Id}", id);
            await _journalRepository.UpdateJournalEntryAsync(journal);
            return Ok(journal);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating the journal entry");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpDelete("{id}", Name = "DeleteJournal")]
    public async Task<IActionResult> DeleteJournal(Guid id)
    {
        try
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
                return StatusCode(403, "You do not have permission to delete this journal entry.");
            }

            var existingJournal = await _journalRepository.GetJournalEntryByIdAsync(id);
            if (existingJournal == null)
            {
                _logger.LogWarning("Journal entry with ID: {Id} not found", id);
                return NotFound("Journal entry not found.");
            }

            _logger.LogInformation("Deleting journal entry with ID: {Id}", id);
            await _journalRepository.DeleteJournalEntryAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting the journal entry");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}