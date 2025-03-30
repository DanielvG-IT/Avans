using UnityEngine;
public class JournalApiClient : MonoBehaviour
{
    public WebClient webClient;

    public async Awaitable<IWebRequestReponse> ReadJournalEntriesAsync()
    {
        string route = $"/api/v1/journal";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<JournalEntry>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> ReadJournalEntriesAsync(string patientId)
    {
        string route = $"/api/v1/journal?patientId={patientId}";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<JournalEntry>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> ReadJournalEntriesAsync(string guardianId, string patientId)
    {
        string route = $"/api/v1/journal?guardianId={guardianId}&patientId={patientId}";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<JournalEntry>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> ReadJournalEntriesByGuardianAsync(string guardianId)
    {
        string route = $"/api/v1/journal?guardianId={guardianId}";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<JournalEntry>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> ReadJournalEntryByIdAsync(string journalEntryId)
    {
        string route = $"/api/v1/stickers/{journalEntryId}";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseResponse<Sticker>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> CreateJournalEntryAsync(JournalEntry journalData)
    {
        string route = $"/api/v1/stickers";
        string data = JsonUtility.ToJson(journalData);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequestAsync(route, data);
        return JsonHelper.ParseResponse<Sticker>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> UpdateJournalEntryAsync(string journalEntryId, JournalEntry journalData)
    {
        string route = $"/api/v1/stickers/{journalEntryId}";
        string data = JsonUtility.ToJson(journalData);

        IWebRequestReponse webRequestResponse = await webClient.SendPutRequestAsync(route, data);
        return JsonHelper.ParseResponse<Sticker>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> DeleteJournalEntryAsync(string journalEntryId)
    {
        string route = $"/api/v1/stickers/{journalEntryId}";
        return await webClient.SendDeleteRequestAsync(route);
    }
}