using UnityEngine;

/// <summary>
/// Provides methods to interact with the Guardian API for CRUD operations.
/// </summary>
public class GuardianApiClient : MonoBehaviour
{
    /// <summary>
    /// The web client used to send HTTP requests to the API.
    /// </summary>
    public WebClient webClient;

    /// <summary>
    /// Retrieves a list of all guardians from the API.
    /// </summary>
    /// <returns>
    /// An <see cref="IWebRequestReponse"/> containing the response data parsed as a list of <see cref="Guardian"/> objects.
    /// </returns>
    /// <exception cref="HttpRequestException">
    /// Thrown if the HTTP request fails or the response cannot be parsed.
    /// </exception>
    public async Awaitable<IWebRequestReponse> ReadGuardianAsync()
    {
        string route = $"/api/v1/guardian";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseResponse<Guardian>(webRequestResponse);
    }

    /// <summary>
    /// Retrieves a specific guardian by their unique identifier.
    /// </summary>
    /// <param name="guardianId">The unique identifier of the guardian to retrieve.</param>
    /// <returns>
    /// An <see cref="IWebRequestReponse"/> containing the response data parsed as a <see cref="Guardian"/> object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="guardianId"/> is null or empty.
    /// </exception>
    /// <exception cref="HttpRequestException">
    /// Thrown if the HTTP request fails or the response cannot be parsed.
    /// </exception>
    public async Awaitable<IWebRequestReponse> ReadGuardianById(string guardianId)
    {
        string route = $"/api/v1/guardian/{guardianId}";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseResponse<Guardian>(webRequestResponse);
    }

    /// <summary>
    /// Creates a new guardian in the API.
    /// </summary>
    /// <param name="guardianData">The <see cref="Guardian"/> object containing the data for the new guardian.</param>
    /// <returns>
    /// An <see cref="IWebRequestReponse"/> containing the response data parsed as the created <see cref="Guardian"/> object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="guardianData"/> is null.
    /// </exception>
    /// <exception cref="HttpRequestException">
    /// Thrown if the HTTP request fails or the response cannot be parsed.
    /// </exception>
    public async Awaitable<IWebRequestReponse> CreateGuardianAsync(Guardian guardianData)
    {
        string route = $"/api/v1/guardian";
        string data = JsonUtility.ToJson(guardianData);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequestAsync(route, data);
        return JsonHelper.ParseResponse<Guardian>(webRequestResponse);
    }

    /// <summary>
    /// Updates an existing guardian in the API.
    /// </summary>
    /// <param name="guardianId">The unique identifier of the guardian to update.</param>
    /// <param name="guardianData">The <see cref="Guardian"/> object containing the updated data for the guardian.</param>
    /// <returns>
    /// An <see cref="IWebRequestReponse"/> containing the response data parsed as the updated <see cref="Guardian"/> object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="guardianId"/> or <paramref name="guardianData"/> is null or empty.
    /// </exception>
    /// <exception cref="HttpRequestException">
    /// Thrown if the HTTP request fails or the response cannot be parsed.
    /// </exception>
    public async Awaitable<IWebRequestReponse> UpdateGuardianAsync(string guardianId, Guardian guardianData)
    {
        string route = $"/api/v1/guardian/{guardianId}";
        string data = JsonUtility.ToJson(guardianData);

        IWebRequestReponse webRequestResponse = await webClient.SendPutRequestAsync(route, data);
        return JsonHelper.ParseResponse<Guardian>(webRequestResponse);
    }

    /// <summary>
    /// Deletes a specific guardian from the API.
    /// </summary>
    /// <param name="guardianId">The unique identifier of the guardian to delete.</param>
    /// <returns>
    /// An <see cref="IWebRequestReponse"/> containing the response from the delete operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="guardianId"/> is null or empty.
    /// </exception>
    /// <exception cref="HttpRequestException">
    /// Thrown if the HTTP request fails.
    /// </exception>
    public async Awaitable<IWebRequestReponse> DeleteGuardianAsync(string guardianId)
    {
        string route = $"/api/v1/guardian/{guardianId}";
        return await webClient.SendDeleteRequestAsync(route);
    }
}