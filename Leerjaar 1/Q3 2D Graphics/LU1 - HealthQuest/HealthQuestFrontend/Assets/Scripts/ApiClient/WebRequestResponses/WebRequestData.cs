using UnityEngine;

public class WebRequestData<T> : IWebRequestReponse
{
    public readonly T Data;
    public readonly int StatusCode;

    public WebRequestData(T data, int statusCode)
    {
        Data = data;
        StatusCode = statusCode;
    }
}
