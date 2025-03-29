using UnityEngine;

public class GuardianApiClient : MonoBehaviour
{
  public WebClient webClient;

    public async Awaitable<IWebRequestReponse> ReadGuardianAsync()
    {
        string route = "/api/v1/guardian";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseResponse<Guardian>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> ReadGuardianById(string guardianId)
    {
        string route = "/api/v1/guardian" + guardianId;

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseResponse<Guardian>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> CreateGuardianAsync(Guardian guardianData)
    {
        string route = "/api/v1/guardian";
        string data = JsonUtility.ToJson(guardianData);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequestAsync(route, data);
        return JsonHelper.ParseResponse<Guardian>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> UpdateGuardianAsync(string guardianId, Guardian guardianData)
    {
        string route = "/api/v1/guardian/" + guardianId;
        string data = JsonUtility.ToJson(guardianData);

        IWebRequestReponse webRequestResponse = await webClient.SendPutRequestAsync(route, data);
        return JsonHelper.ParseResponse<Guardian>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> DeleteGuardianAsync(string guardianId)
    {
        string route = "/api/v1/guardian/" + guardianId;
        return await webClient.SendDeleteRequestAsync(route);
    }
}