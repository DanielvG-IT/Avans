using System;
using UnityEngine;

public class WebRequestError : IWebRequestReponse
{
    public readonly string ErrorMessage;
    public readonly int StatusCode;

    public WebRequestError(string errormessage, int statusCode)
    {
        ErrorMessage = errormessage;
        StatusCode = statusCode;
    }
}