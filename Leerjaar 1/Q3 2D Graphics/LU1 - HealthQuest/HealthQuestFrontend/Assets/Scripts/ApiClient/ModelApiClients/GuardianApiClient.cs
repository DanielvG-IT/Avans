using UnityEngine;

public class GuardianApiClient : MonoBehaviour
{
  public WebClient webClient;

    public async Awaitable<IWebRequestReponse> ReadGuardian()
    {
        string route = "/api/v1/guardian";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseGuardianResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> ReadGuardianById(string guardianId)
    {
        string route = "/api/v1/guardian" + guardianId;

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseGuardianResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> CreateGuardian(Guardian guardianData)
    {
        string route = "/api/v1/guardian";
        string data = JsonUtility.ToJson(guardianData);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequestAsync(route, data);
        return JsonHelper.ParseGuardianResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> UpdateGuardian(string guardianId, Guardian guardianData)
    {
        string route = "/api/v1/guardian/" + guardianId;
        string data = JsonUtility.ToJson(guardianData);

        IWebRequestReponse webRequestResponse = await webClient.SendPutRequestAsync(route, data);
        return JsonHelper.ParseGuardianResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> DeleteGuardian(string guardianId)
    {
        string route = "/api/v1/guardian/" + guardianId;
        return await webClient.SendDeleteRequestAsync(route);
    }
}