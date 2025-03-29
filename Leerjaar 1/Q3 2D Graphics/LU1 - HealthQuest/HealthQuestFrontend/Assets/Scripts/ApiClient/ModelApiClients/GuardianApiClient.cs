using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class GuardianApiClient : MonoBehaviour
{
  public WebClient webClient;

    public async Awaitable<IWebRequestReponse> ReadGuardian()
    {
        string route = "/api/v1/guardian";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return ParseGuardianResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> ReadGuardianById(string guardianId)
    {
        string route = "/api/v1/guardian" + guardianId;

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return ParseGuardianResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> CreateGuardian(Guardian guardianData)
    {
        string route = "/api/v1/guardian";
        string data = JsonUtility.ToJson(guardianData);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequestAsync(route, data);
        return ParseGuardianResponse(webRequestResponse);
    }

    //public async Awaitable<IWebRequestReponse> UpdateGuardian(string guardianId, Guardian guardianData)
    //{
    //    string route = "/api/v1/guardian/" + guardianId;
    //    string data = JsonUtility.ToJson(guardianData);
    //
    //    IWebRequestReponse webRequestResponse = await webClient.SendPutRequestAsync(route, data);
    //    return ParseGuardianResponse(webRequestResponse);
    //}

    public async Awaitable<IWebRequestReponse> DeleteGuardian(string guardianId)
    {
        string route = "/api/v1/guardian/" + guardianId;
        return await webClient.SendDeleteRequestAsync(route);
    }


    // Parse methodes for response data
    private IWebRequestReponse ParseGuardianResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data); // TODO: remove debug log
                var guardian = JsonUtility.FromJson<Guardian>(data.Data);
                WebRequestData<Guardian> parsedWebRequestData = new(guardian);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }

    private IWebRequestReponse ParseGuardianListResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data); // TODO: remove debug log
                List<Guardian> guardians = JsonHelper.ParseJsonArray<Guardian>(data.Data);
                WebRequestData<List<Guardian>> parsedWebRequestData = new(guardians);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }
}