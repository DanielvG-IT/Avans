using System;
using UnityEngine;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class WebClient : MonoBehaviour
{
    // Singleton instance
    public static WebClient Instance { get; private set; }

    [Header("WebClient Settings")]
    public string baseUrl;
    public Token token;

    // Set the token for authentication
    public void SetToken(Token reseivedToken)
    {
        token = reseivedToken;
    }

    // Send GET request
    public async Task<IWebRequestReponse> SendGetRequestAsync(string route)
    {
        return await SendRequestAsync("GET", route, string.Empty);
    }

    // Send POST request
    public async Task<IWebRequestReponse> SendPostRequestAsync(string route, string data)
    {
        return await SendRequestAsync("POST", route, data);
    }

    // Send PUT request
    public async Task<IWebRequestReponse> SendPutRequestAsync(string route, string data)
    {
        return await SendRequestAsync("PUT", route, data);
    }

    // Send DELETE request
    public async Task<IWebRequestReponse> SendDeleteRequestAsync(string route)
    {
        return await SendRequestAsync("DELETE", route, string.Empty);
    }

    // Centralized method to handle all request types
    private async Task<IWebRequestReponse> SendRequestAsync(string method, string route, string data)
    {
        var webRequest = CreateWebRequest(method, route, data);
        return await ProcessWebRequestAsync(webRequest);
    }

    // Create a web request with method, route, and data
    private UnityWebRequest CreateWebRequest(string method, string route, string data)
    {
        string url = baseUrl + route;

        data = RemoveIdFromJson(data); // Clean the data to avoid invalid GUIDs

        UnityWebRequest webRequest = new(url, method)
        {
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data)),
            downloadHandler = new DownloadHandlerBuffer()
        };

        webRequest.SetRequestHeader("Content-Type", "application/json");
        AddTokenToRequest(webRequest);

        return webRequest;
    }

    // Process web request and handle success/error responses
    private async Task<IWebRequestReponse> ProcessWebRequestAsync(UnityWebRequest webRequest)
    {
        await webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string responseData = webRequest.downloadHandler.text;
            return new WebRequestData<string>(responseData);
        }
        else
        {
            string responseData = webRequest.downloadHandler.text;
            return new WebRequestError<string>(responseData);
        }
    }

    // Add the token to the request headers for authentication
    private void AddTokenToRequest(UnityWebRequest webRequest)
    {
        if (!string.IsNullOrEmpty(token.accessToken))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + token);
        }
        else
        {
            Debug.LogWarning("Token is not set. Please set the token before making requests.");
        }
    }

    // Clean out empty GUID fields in JSON data to prevent errors on the server
    private string RemoveIdFromJson(string json)
    {
        return json.Replace("\"id\":\"\",", "");
    }
}