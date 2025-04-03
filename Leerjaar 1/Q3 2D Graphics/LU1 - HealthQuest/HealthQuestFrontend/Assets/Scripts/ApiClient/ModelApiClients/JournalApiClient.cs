using UnityEngine;

/// <summary>
/// A client for interacting with the Journal API, providing methods to perform CRUD operations on journal entries.
/// </summary>
public class JournalApiClient : MonoBehaviour
{
    /// <summary>
    /// The WebClient instance used to send HTTP requests.
    /// </summary>
    public WebClient webClient;

    /// <summary>
    /// Retrieves all journal entries.
    /// </summary>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing a list of <see cref="JournalEntry"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request fails.</exception>
    public async Awaitable<IWebRequestReponse> ReadJournalEntriesAsync()
    {
        string route = $"/api/v1/journal";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<JournalEntry>(webRequestResponse);
    }

    /// <summary>
    /// Retrieves journal entries for a specific patient.
    /// </summary>
    /// <param name="patientId">The ID of the patient whose journal entries are to be retrieved.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing a list of <see cref="JournalEntry"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request fails.</exception>
    public async Awaitable<IWebRequestReponse> ReadJournalEntriesAsync(string patientId)
    {
        string route = $"/api/v1/journal?patientId={patientId}";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<JournalEntry>(webRequestResponse);
    }

    /// <summary>
    /// Retrieves journal entries for a specific guardian and patient.
    /// </summary>
    /// <param name="guardianId">The ID of the guardian.</param>
    /// <param name="patientId">The ID of the patient.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing a list of <see cref="JournalEntry"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request fails.</exception>
    public async Awaitable<IWebRequestReponse> ReadJournalEntriesAsync(string guardianId, string patientId)
    {
        string route = $"/api/v1/journal?guardianId={guardianId}&patientId={patientId}";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<JournalEntry>(webRequestResponse);
    }

    /// <summary>
    /// Retrieves journal entries for a specific guardian.
    /// </summary>
    /// <param name="guardianId">The ID of the guardian whose journal entries are to be retrieved.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing a list of <see cref="JournalEntry"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request fails.</exception>
    public async Awaitable<IWebRequestReponse> ReadJournalEntriesByGuardianAsync(string guardianId)
    {
        string route = $"/api/v1/journal?guardianId={guardianId}";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<JournalEntry>(webRequestResponse);
    }

    /// <summary>
    /// Retrieves a specific journal entry by its ID.
    /// </summary>
    /// <param name="journalEntryId">The ID of the journal entry to retrieve.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing the <see cref="Sticker"/> object.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request fails.</exception>
    public async Awaitable<IWebRequestReponse> ReadJournalEntryByIdAsync(string journalEntryId)
    {
        string route = $"/api/v1/journal/{journalEntryId}";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseResponse<Sticker>(webRequestResponse);
    }

    /// <summary>
    /// Creates a new journal entry.
    /// </summary>
    /// <param name="journalData">The data for the new journal entry.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing the created <see cref="Sticker"/> object.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request fails.</exception>
    public async Awaitable<IWebRequestReponse> CreateJournalEntryAsync(JournalEntry journalData)
    {
        string route = $"/api/v1/journal";
        string data = JsonUtility.ToJson(journalData);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequestAsync(route, data);
        return JsonHelper.ParseResponse<Sticker>(webRequestResponse);
    }

    /// <summary>
    /// Updates an existing journal entry.
    /// </summary>
    /// <param name="journalEntryId">The ID of the journal entry to update.</param>
    /// <param name="journalData">The updated data for the journal entry.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing the updated <see cref="Sticker"/> object.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request fails.</exception>
    public async Awaitable<IWebRequestReponse> UpdateJournalEntryAsync(string journalEntryId, JournalEntry journalData)
    {
        string route = $"/api/v1/journal/{journalEntryId}";
        string data = JsonUtility.ToJson(journalData);

        IWebRequestReponse webRequestResponse = await webClient.SendPutRequestAsync(route, data);
        return JsonHelper.ParseResponse<Sticker>(webRequestResponse);
    }

    /// <summary>
    /// Deletes a journal entry by its ID.
    /// </summary>
    /// <param name="journalEntryId">The ID of the journal entry to delete.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> indicating the result of the delete operation.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request fails.</exception>
    public async Awaitable<IWebRequestReponse> DeleteJournalEntryAsync(string journalEntryId)
    {
        string route = $"/api/v1/journal/{journalEntryId}";
        return await webClient.SendDeleteRequestAsync(route);
    }
}