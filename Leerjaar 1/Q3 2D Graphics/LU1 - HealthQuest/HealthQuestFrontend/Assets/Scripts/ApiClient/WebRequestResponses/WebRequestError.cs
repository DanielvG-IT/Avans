using System;
using UnityEngine;

public class WebRequestError : IWebRequestReponse
{
    public readonly string ErrorMessage;

    public WebRequestError(string errormessage)
    {
        ErrorMessage = errormessage;
    }
}