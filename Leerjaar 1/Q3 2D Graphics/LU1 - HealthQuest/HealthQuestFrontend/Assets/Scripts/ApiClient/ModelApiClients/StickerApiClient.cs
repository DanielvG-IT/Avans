using UnityEngine;

public class StickerApiClient : MonoBehaviour
{
    public WebClient webClient;

    public async Awaitable<IWebRequestReponse> ReadStickersAsync()
    {
        string route = $"/api/v1/stickers";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<Sticker>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> ReadStickerByIdAsync(string stickerId)
    {
        string route = $"/api/v1/stickers/{stickerId}";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseResponse<Sticker>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> CreateStickerAsync(Sticker stickerData)
    {
        string route = $"/api/v1/stickers";
        string data = JsonUtility.ToJson(stickerData);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequestAsync(route, data);
        return JsonHelper.ParseResponse<Sticker>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> UpdateStickerAsync(string stickerId, Sticker stickerData)
    {
        string route = $"/api/v1/stickers/{stickerId}";
        string data = JsonUtility.ToJson(stickerData);

        IWebRequestReponse webRequestResponse = await webClient.SendPutRequestAsync(route, data);
        return JsonHelper.ParseResponse<Sticker>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> DeleteStickerAsync(string stickerId)
    {
        string route = $"/api/v1/stickers/{stickerId}";
        return await webClient.SendDeleteRequestAsync(route);
    }
}
