using HQB.WebApi.Models;

namespace HQB.WebApi.Interfaces;
public interface IJournalRepository
{
    Task<IEnumerable<JournalEntry>> GetAllJournalEntriesAsync();
    Task<JournalEntry?> GetJournalEntryByIdAsync(Guid id);
    Task AddJournalEntryAsync(JournalEntry journalEntry);
    Task UpdateJournalEntryAsync(JournalEntry journalEntry);
    Task DeleteJournalEntryAsync(Guid id);
}