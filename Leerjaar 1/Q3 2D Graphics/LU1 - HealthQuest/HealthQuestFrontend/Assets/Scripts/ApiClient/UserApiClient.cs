//using System;
//using System.Collections.Generic;
//using System.Net;
//using UnityEngine;

//public class UserApiClient : MonoBehaviour
//{
//    public WebClient webClient;

//    public async Awaitable<IWebRequestReponse> Register(User user)
//    {
//        string route = "/account/register";
//        string data = JsonUtility.ToJson(user);

//        return await webClient.SendPostRequestAsync(route, data);
//    }

//    public async Awaitable<IWebRequestReponse> Login(User user)
//    {
//        string route = "/account/login";
//        string data = JsonUtility.ToJson(user);

//        IWebRequestReponse response = await webClient.SendPostRequestAsync(route, data);
//        return ProcessLoginResponse(response);
//    }

//    private IWebRequestReponse ProcessLoginResponse(IWebRequestReponse webRequestResponse)
//    {
//        if (webRequestResponse is WebRequestData<string> data)
//        {
//            Token token = JsonHelper.ExtractToken(data.Data);
//            webClient.SetToken(token);

//            return new WebRequestData<string>("Success");
//        }

//        return webRequestResponse;
//    }

//}

