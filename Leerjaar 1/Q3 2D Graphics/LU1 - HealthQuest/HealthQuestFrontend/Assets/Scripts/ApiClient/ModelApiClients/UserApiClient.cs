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
        string route = $"{"/api/v1/auth/register"}";
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
        string route = $"/api/v1/auth/login";
        string data = JsonUtility.ToJson(user);

        IWebRequestReponse response = await webClient.SendPostRequestAsync(route, data);
        return ProcessLoginResponse(response);
    }

    /// <summary>
    /// Sends a request to refresh the access token using the provided refresh token.
    /// </summary>
    /// <param name="token">The refresh token used to obtain a new access token.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing the response
    /// from the server after attempting to refresh the access token.
    /// </returns>
    public async Awaitable<IWebRequestReponse> RefreshAccessToken(RefreshToken token)
    {
        string route = $"/api/v1/auth/refresh";
        string data = JsonUtility.ToJson(token);

        IWebRequestReponse response = await webClient.SendPostRequestAsync(route, data);
        return ProcessLoginResponse(response);
    }

    /// <summary>
    /// Sends a GET request to retrieve the roles of the current user.
    /// </summary>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> 
    /// containing the response from the server.
    /// </returns>
    /// <remarks>
    /// This method uses the web client to send a request to the endpoint 
    /// <c>/api/v1/auth/roles</c>.
    /// </remarks>
    public async Awaitable<IWebRequestReponse> GetRole()
    {
        string route = $"/api/v1/auth/roles";

        return await webClient.SendGetRequestAsync(route);
    }

    private IWebRequestReponse ProcessLoginResponse(IWebRequestReponse response)
    {
        if (response is WebRequestData<string> data)
        {
            webClient.SetToken(JsonHelper.ExtractToken(data.Data));
            return new WebRequestData<string>("Login successful!", 200);
        }

        return new WebRequestError("Unknown error occurred during login.", 400);
    }
}