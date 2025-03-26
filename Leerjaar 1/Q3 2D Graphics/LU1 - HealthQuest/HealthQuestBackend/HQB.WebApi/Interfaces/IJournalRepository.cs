using HQB.WebApi.Models;

namespace HQB.WebApi.Interfaces;
/// <summary>
/// Interface for managing journal entries in the repository.
/// </summary>
public interface IJournalRepository
{
    /// <summary>
    /// Retrieves all journal entries asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of journal entries.</returns>
    Task<IEnumerable<JournalEntry>> GetAllJournalEntriesAsync();

    /// <summary>
    /// Retrieves a journal entry by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the journal entry.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the journal entry if found; otherwise, null.</returns>
    Task<JournalEntry?> GetJournalEntryByIdAsync(Guid id);

    Task<IEnumerable<JournalEntry>> GetJournalEntriesByPatientIdAsync(Guid id);

    /// <summary>
    /// Adds a new journal entry asynchronously.
    /// </summary>
    /// <param name="journalEntry">The journal entry to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddJournalEntryAsync(JournalEntry journalEntry);

    /// <summary>
    /// Updates an existing journal entry asynchronously.
    /// </summary>
    /// <param name="journalEntry">The journal entry to update.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateJournalEntryAsync(JournalEntry journalEntry);

    /// <summary>
    /// Deletes a journal entry by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the journal entry to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteJournalEntryAsync(Guid id);
}