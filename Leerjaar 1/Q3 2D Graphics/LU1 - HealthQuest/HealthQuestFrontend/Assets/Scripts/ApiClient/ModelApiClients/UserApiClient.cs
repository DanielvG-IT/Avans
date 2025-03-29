using UnityEngine;

public class UserApiClient : MonoBehaviour
{
    public WebClient webClient;

    public async Awaitable<IWebRequestReponse> Register(User user)
    {
        string route = "/api/v1/account/register";
        string data = JsonUtility.ToJson(user);

        return await webClient.SendPostRequestAsync(route, data);
    }

    public async Awaitable<IWebRequestReponse> Login(User user)
    {
        string route = "/api/v1/account/login";
        string data = JsonUtility.ToJson(user);

        IWebRequestReponse response = await webClient.SendPostRequestAsync(route, data);
        return ProcessLoginResponse(response);
    }

    private IWebRequestReponse ProcessLoginResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data); // TODO remove debug log
                var token = JsonHelper.ExtractToken(data.Data);
                webClient.SetToken(token);
                return new WebRequestData<string>("Login succesfull!");
            default:
                return webRequestResponse;
        }
    }

}

