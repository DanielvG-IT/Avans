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

    /// <summary>
    /// Processes the server's response to a login request.
    /// </summary>
    /// <param name="webRequestResponse">The raw response from the server.</param>
    /// <returns>
    /// An <see cref="IWebRequestReponse"/> object that contains either a success message 
    /// or the original response if processing fails.
    /// </returns>
    /// <remarks>
    /// If the response contains a valid token, it is extracted and set in the <see cref="WebClient"/>.
    /// </remarks>
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