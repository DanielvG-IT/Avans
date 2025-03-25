using HQB.WebApi.Models;
using HQB.WebApi.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace HQB.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JournalController : ControllerBase
{
    private readonly IJournalRepository _journalRepository;

    public JournalController(IJournalRepository journalRepository)
    {
        _journalRepository = journalRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JournalEntry>>> GetJournals()
    {
        var journals = await _journalRepository.GetAllJournalEntriesAsync();
        return Ok(journals);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JournalEntry>> GetJournal(Guid id)
    {
        var journal = await _journalRepository.GetJournalEntryByIdAsync(id);
        if (journal == null)
        {
            return NotFound();
        }
        return Ok(journal);
    }

    [HttpPost]
    public async Task<ActionResult<JournalEntry>> PostJournal(JournalEntry journal)
    {
        await _journalRepository.AddJournalEntryAsync(journal);
        return CreatedAtAction("GetJournal", new { id = journal.ID }, journal);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutJournal(Guid id, JournalEntry journal)
    {
        if (id != journal.ID)
        {
            return BadRequest();
        }
        await _journalRepository.UpdateJournalEntryAsync(journal);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJournal(Guid id)
    {
        await _journalRepository.DeleteJournalEntryAsync(id);
        return NoContent();
    }
}