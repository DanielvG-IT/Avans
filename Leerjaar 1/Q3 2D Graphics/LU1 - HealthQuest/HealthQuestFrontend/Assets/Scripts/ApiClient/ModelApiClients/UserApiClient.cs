using UnityEngine;

public class UserApiClient : MonoBehaviour
{
    public WebClient webClient;

    /// <summary>
    /// Registers a new user by sending their information to the server.
    /// </summary>
    /// <param name="user">The user object containing registration details such as username, email, and password.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> object, 
    /// which contains the server's response to the registration request.
    /// </returns>
    /// <exception cref="System.Exception">
    /// Throws an exception if the webClient fails to send the request or if the server returns an error.
    /// </exception>
    public async Awaitable<IWebRequestReponse> Register(User user)
    {
        string route = $"{"/api/v1/account/register"}";
        string data = JsonUtility.ToJson(user);

        return await webClient.SendPostRequestAsync(route, data);
    }

    /// <summary>
    /// Logs in a user by sending their credentials to the server.
    /// </summary>
    /// <param name="user">The user object containing login details such as username and password.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> object, 
    /// which contains the processed response from the server.
    /// </returns>
    /// <exception cref="System.Exception">
    /// Throws an exception if the webClient fails to send the request or if the server returns an error.
    /// </exception>
    public async Awaitable<IWebRequestReponse> Login(User user)
    {
        string route = $"{"/api/v1/account/login"}";
        string data = JsonUtility.ToJson(user);

        IWebRequestReponse response = await webClient.SendPostRequestAsync(route, data);
        return ProcessLoginResponse(response);
    }

    // TODO: Add refresh token method here to refresh the token when it expires or is 


    private IWebRequestReponse ProcessLoginResponse(IWebRequestReponse response)
    {
        if (response is WebRequestData<string> data)
        {
            webClient.SetToken(JsonHelper.ExtractToken(data.Data));
            return new WebRequestData<string>("Login successful!");
        }

        return new WebRequestError("Unknown error occurred during login.");
    }
}