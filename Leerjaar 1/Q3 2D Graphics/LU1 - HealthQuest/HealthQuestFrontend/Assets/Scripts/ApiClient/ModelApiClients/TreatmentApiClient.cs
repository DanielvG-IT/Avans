using System;
using UnityEngine;

public class TreatmentApiClient : MonoBehaviour
{
    public WebClient webClient;

    public async Awaitable<IWebRequestReponse> ReadTreatmentsAsync()
    {
        string route = $"/api/v1/treatments";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<Treatment>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> ReadTreatmentByIdAsync(string treatmentId)
    {
        string route = $"/api/v1/treatments/{treatmentId}";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseResponse<Treatment>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> CreateTreatmentAsync(Treatment treatmentData)
    {
        string route = $"/api/v1/treatments";
        string data = JsonUtility.ToJson(treatmentData);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequestAsync(route, data);
        return JsonHelper.ParseResponse<Treatment>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> UpdateTreatmentAsync(string treatmentId, Treatment treatmentData)
    {
        string route = $"/api/v1/treatments/{treatmentId}";
        string data = JsonUtility.ToJson(treatmentData);

        IWebRequestReponse webRequestResponse = await webClient.SendPutRequestAsync(route, data);
        return JsonHelper.ParseResponse<Patient>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> DeleteTreatmentAsync(string treatmentId)
    {
        string route = $"/api/v1/treatments/{treatmentId}";
        return await webClient.SendDeleteRequestAsync(route);
    }

    public async Awaitable<IWebRequestReponse> ReadAppointmentsByTreatmentIdAsync(string treatmentId)
    {
        string route = $"/api/v1/treatments/{treatmentId}/appointments";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<Appointment>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> AddAppointmentToTreatmentAsync(string treatmentId, string appointmentId, int sequence)
    {
        string route = $"/api/v1/treatments/{treatmentId}/appointments/{appointmentId}?sequence={sequence}";
        string data = "";

        // Returns 204 No Content on success so no need to parse response
        return await webClient.SendPostRequestAsync(route, data);
    }
}