using UnityEngine;

public class StickerApiClient : MonoBehaviour
{
    public WebClient webClient;

    /// <summary>
    /// Retrieves a list of all stickers from the API.
    /// </summary>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing a list of stickers.
    /// </returns>
    /// <exception cref="WebRequestException">Thrown if the GET request fails.</exception>
    public async Awaitable<IWebRequestReponse> ReadStickersAsync()
    {
        string route = $"/api/v1/stickers";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<Sticker>(webRequestResponse);
    }

    /// <summary>
    /// Retrieves a sticker by its unique identifier.
    /// </summary>
    /// <param name="stickerId">The unique identifier of the sticker to retrieve.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing the sticker data.
    /// </returns>
    /// <exception cref="WebRequestException">Thrown if the GET request fails or the sticker is not found.</exception>
    public async Awaitable<IWebRequestReponse> ReadStickerByIdAsync(string stickerId)
    {
        string route = $"/api/v1/stickers/{stickerId}";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseResponse<Sticker>(webRequestResponse);
    }

    /// <summary>
    /// Searches for a sticker by its name.
    /// </summary>
    /// <param name="stickerName">The name of the sticker to search for.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing the sticker data.
    /// </returns>
    /// <exception cref="WebRequestException">Thrown if the GET request fails or no sticker matches the given name.</exception>
    public async Awaitable<IWebRequestReponse> ReadStickerByNameAsync(string stickerName)
    {
        string route = $"/api/v1/stickers/search?name={stickerName}";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseResponse<Sticker>(webRequestResponse);
    }

    /// <summary>
    /// Creates a new sticker in the API.
    /// </summary>
    /// <param name="stickerData">The data of the sticker to create.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing the created sticker data.
    /// </returns>
    /// <exception cref="WebRequestException">Thrown if the POST request fails.</exception>
    public async Awaitable<IWebRequestReponse> CreateStickerAsync(Sticker stickerData)
    {
        string route = $"/api/v1/stickers";
        string data = JsonUtility.ToJson(stickerData);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequestAsync(route, data);
        return JsonHelper.ParseResponse<Sticker>(webRequestResponse);
    }

    /// <summary>
    /// Updates an existing sticker in the API.
    /// </summary>
    /// <param name="stickerId">The unique identifier of the sticker to update.</param>
    /// <param name="stickerData">The updated data for the sticker.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing the updated sticker data.
    /// </returns>
    /// <exception cref="WebRequestException">Thrown if the PUT request fails or the sticker is not found.</exception>
    public async Awaitable<IWebRequestReponse> UpdateStickerAsync(string stickerId, Sticker stickerData)
    {
        string route = $"/api/v1/stickers/{stickerId}";
        string data = JsonUtility.ToJson(stickerData);

        IWebRequestReponse webRequestResponse = await webClient.SendPutRequestAsync(route, data);
        return JsonHelper.ParseResponse<Sticker>(webRequestResponse);
    }

    /// <summary>
    /// Deletes a sticker from the API by its unique identifier.
    /// </summary>
    /// <param name="stickerId">The unique identifier of the sticker to delete.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> indicating the result of the delete operation.
    /// </returns>
    /// <exception cref="WebRequestException">Thrown if the DELETE request fails or the sticker is not found.</exception>
    public async Awaitable<IWebRequestReponse> DeleteStickerAsync(string stickerId)
    {
        string route = $"/api/v1/stickers/{stickerId}";
        return await webClient.SendDeleteRequestAsync(route);
    }
}