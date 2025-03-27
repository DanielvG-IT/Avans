using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HQB.WebApi.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class JournalController : ControllerBase
{
    private readonly IJournalRepository _journalRepository;
    private readonly ILogger<JournalController> _logger;

    public JournalController(IJournalRepository journalRepository, ILogger<JournalController> logger)
    {
        _journalRepository = journalRepository ?? throw new ArgumentNullException(nameof(journalRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JournalEntry>>> GetJournals()
    {
        _logger.LogInformation("Getting all journal entries");
        var journals = await _journalRepository.GetAllJournalEntriesAsync();
        if (journals == null)
        {
            _logger.LogWarning("No journal entries found");
            return NotFound();
        }
        return Ok(journals);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JournalEntry>> GetJournal(Guid id)
    {
        if (id == Guid.Empty)
        {
            _logger.LogWarning("Invalid journal entry ID: {Id}", id);
            return BadRequest("Invalid ID");
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

        _logger.LogInformation("Updating journal entry with ID: {Id}", id);
        await _journalRepository.UpdateJournalEntryAsync(journal);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJournal(Guid id)
    {
        if (id == Guid.Empty)
        {
            _logger.LogWarning("Invalid journal entry ID: {Id}", id);
            return BadRequest("Invalid ID");
        }

        _logger.LogInformation("Deleting journal entry with ID: {Id}", id);
        await _journalRepository.DeleteJournalEntryAsync(id);
        return NoContent();
    }
}