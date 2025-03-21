using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class OuderVoogdApiClient : MonoBehaviour
{
    public WebClient webClient;

    public async Awaitable<IWebRequestReponse> Register(OuderVoogd user)
    {
        string route = "/api/auth/register";
        string data = JsonUtility.ToJson(user);

        return await webClient.SendPostRequestAsync(route, data);
    }

    public async Awaitable<IWebRequestReponse> Login(OuderVoogd user)
    {
        string route = "/api/auth/login";
        string data = JsonUtility.ToJson(user);

        IWebRequestReponse response = await webClient.SendPostRequestAsync(route, data);
        return ProcessLoginResponse(response);
    }

    private IWebRequestReponse ProcessLoginResponse(IWebRequestReponse webRequestResponse)
    {
        if (webRequestResponse is WebRequestData<string> data)
        {
            Token token = JsonUtility.FromJson<Token>(data.Data);
            Debug.Log("Login successful. Token: " + token.accessToken);
            webClient.SetToken(token);
        }

        return webRequestResponse;
    }

}

