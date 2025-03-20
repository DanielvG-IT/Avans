using System;
using UnityEngine;

public class WebRequestError<T> : IWebRequestReponse
{
    public readonly T Data;

    public WebRequestError(T data)
    {
        Data = data;
    }
}