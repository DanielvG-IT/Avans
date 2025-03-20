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
}

[Serializable]
public class JsonList<T>
{
    public List<T> list;
}