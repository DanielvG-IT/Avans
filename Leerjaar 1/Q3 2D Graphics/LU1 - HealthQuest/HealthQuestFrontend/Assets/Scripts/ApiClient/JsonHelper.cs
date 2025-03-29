using System.Collections.Generic;
using System;
using UnityEngine;

public static class JsonHelper
{
    public static List<T> ParseJsonArray<T>(string jsonArray)
    {
        string extendedJson = "{\"list\":" + jsonArray + "}";
        JsonList<T> parsedList = JsonUtility.FromJson<JsonList<T>>(extendedJson);
        return parsedList.list;
    }

    public static Token ExtractToken(string data)
    {
        return JsonUtility.FromJson<Token>(data);
    }

    public static IWebRequestReponse ParseGuardianResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data); // TODO: remove debug log
                var guardian = JsonUtility.FromJson<Guardian>(data.Data);
                WebRequestData<Guardian> parsedWebRequestData = new(guardian);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }

    public static IWebRequestReponse ParseGuardianListResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data); // TODO: remove debug log
                List<Guardian> guardians = ParseJsonArray<Guardian>(data.Data);
                WebRequestData<List<Guardian>> parsedWebRequestData = new(guardians);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }

    public static IWebRequestReponse ParsePatientResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data); // TODO: remove debug log
                var patient = JsonUtility.FromJson<Patient>(data.Data);
                WebRequestData<Patient> parsedWebRequestData = new(patient);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }

    public static IWebRequestReponse ParsePatientListResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data); // TODO: remove debug log
                List<Patient> patients = ParseJsonArray<Patient>(data.Data);
                WebRequestData<List<Patient>> parsedWebRequestData = new(patients);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }

    public static IWebRequestReponse ParseStickerResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data); // TODO: remove debug log
                Sticker sticker = JsonUtility.FromJson<Sticker>(data.Data);
                WebRequestData<Sticker> parsedWebRequestData = new(sticker);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }

    public static IWebRequestReponse ParseStickerListResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data); // TODO: remove debug log
                List<Patient> patients = ParseJsonArray<Patient>(data.Data);
                WebRequestData<List<Patient>> parsedWebRequestData = new(patients);
                return parsedWebRequestData;
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