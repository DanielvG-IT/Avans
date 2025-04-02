using System;
using System.Collections.Generic;
using UnityEngine;

public static class JsonHelper
{
    public static List<T> ParseJsonArray<T>(string jsonArray)
    {
        // Wrap the JSON array in a container object to use JsonUtility
        string wrappedJson = $"{{\"list\":{jsonArray}}}";
        JsonList<T> container = JsonUtility.FromJson<JsonList<T>>(wrappedJson);
        return container.list;
    }
    public static Token ExtractToken(string data)
    {
        return JsonUtility.FromJson<Token>(data);
    }

    public static IWebRequestReponse ParseResponse<T>(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data); // TODO: remove debug log
                T parsedData = JsonUtility.FromJson<T>(data.Data);
                return new WebRequestData<T>(parsedData, data.StatusCode);
            default:
                return webRequestResponse;
        }
    }

    public static IWebRequestReponse ParseListResponse<T>(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data); // TODO: remove debug log
                List<T> parsedList = ParseJsonArray<T>(data.Data);
                return new WebRequestData<List<T>>(parsedList, data.StatusCode);
            default:
                return webRequestResponse;
        }
    }
}

[Serializable]
public class JsonList<T>
{
    public List<T> list;
}