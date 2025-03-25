using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace HQB.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JournalController : ControllerBase
{
    private readonly IJournalRepository _journalRepository;
    private readonly ILogger<JournalController> _logger;

    public JournalController(IJournalRepository journalRepository, ILogger<JournalController> logger)
    {
        _journalRepository = journalRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JournalEntry>>> GetJournals()
    {
        _logger.LogInformation("Getting all journal entries");
        var journals = await _journalRepository.GetAllJournalEntriesAsync();
        return Ok(journals);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JournalEntry>> GetJournal(Guid id)
    {
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
        _logger.LogInformation("Creating a new journal entry");
        await _journalRepository.AddJournalEntryAsync(journal);
        return CreatedAtAction("GetJournal", new { id = journal.ID }, journal);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutJournal(Guid id, JournalEntry journal)
    {
        if (id != journal.ID)
        {
            _logger.LogWarning("Journal entry ID mismatch: {Id} != {JournalId}", id, journal.ID);
            return BadRequest();
        }
        _logger.LogInformation("Updating journal entry with ID: {Id}", id);
        await _journalRepository.UpdateJournalEntryAsync(journal);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJournal(Guid id)
    {
        _logger.LogInformation("Deleting journal entry with ID: {Id}", id);
        await _journalRepository.DeleteJournalEntryAsync(id);
        return NoContent();
    }
}