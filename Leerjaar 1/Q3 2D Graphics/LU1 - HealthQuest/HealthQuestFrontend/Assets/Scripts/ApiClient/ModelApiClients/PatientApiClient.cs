using UnityEngine;

class PatientApiClient
{
    public WebClient webClient;

    public async Awaitable<IWebRequestReponse> ReadPatientAsync()
    {
        string route = "/api/v1/patient";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParsePatientResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> ReadPatientById(string patientId)
    {
        string route = "/api/v1/patient" + patientId;

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParsePatientResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> CreatePatient(Patient patientData)
    {
        string route = "/api/v1/patient";
        string data = JsonUtility.ToJson(patientData);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequestAsync(route, data);
        return JsonHelper.ParsePatientResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> UpdatePatient(string patientId, Patient patientData)
    {
        string route = "/api/v1/patient/" + patientId;
        string data = JsonUtility.ToJson(patientData);

        IWebRequestReponse webRequestResponse = await webClient.SendPutRequestAsync(route, data);
        return JsonHelper.ParsePatientResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> DeletePatient(string patientId)
    {
        string route = "/api/v1/patient/" + patientId;
        return await webClient.SendDeleteRequestAsync(route);
    }
}