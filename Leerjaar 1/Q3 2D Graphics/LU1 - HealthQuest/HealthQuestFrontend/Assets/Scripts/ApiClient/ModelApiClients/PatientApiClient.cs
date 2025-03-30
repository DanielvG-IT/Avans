using System;
using UnityEngine;

public class PatientApiClient : MonoBehaviour
{
    public WebClient webClient;

    public async Awaitable<IWebRequestReponse> ReadPatientAsync()
    {
        string route = $"/api/v1/patient";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseResponse<Patient>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> ReadPatientByIdAsync(string patientId)
    {
        string route = $"/api/v1/patient/{patientId}";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseResponse<Patient>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> CreatePatientAsync(Patient patientData)
    {
        string route = $"/api/v1/patient";
        string data = JsonUtility.ToJson(patientData);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequestAsync(route, data);
        return JsonHelper.ParseResponse<Patient>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> UpdatePatient(string patientId, Patient patientData)
    {
        string route = $"/api/v1/patient/{patientId}";
        string data = JsonUtility.ToJson(patientData);

        IWebRequestReponse webRequestResponse = await webClient.SendPutRequestAsync(route, data);
        return JsonHelper.ParseResponse<Patient>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> DeletePatientAsync(string patientId)
    {
        string route = $"/api/v1/patient/{patientId}";
        return await webClient.SendDeleteRequestAsync(route);
    }

    public async Awaitable<IWebRequestReponse> ReadUnlockedStickersFromPatientAsync(string patientId)
    {
        string route = $"/api/v1/patient/{patientId}/stickers";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<Sticker>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> AddUnlockedStickerToPatientAsync(string patientId, Sticker sticker)
    {
        string route = $"/api/v1/patient/{patientId}/stickers?stickerId={sticker.Id}";
        string data = JsonUtility.ToJson(sticker);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequestAsync(route, data);
        return JsonHelper.ParseResponse<Sticker>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> ReadCompletedAppointmentsFromPatientAsync(string patientId)
    {
        string route = $"/api/v1/patient/{patientId}/completed-appointments";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<Appointment>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> AddCompletedAppointmentsToPatientAsync(string patientId, string appointmentId, DateTime completedDateTime)
    {
        string route = $"/api/v1/patient/{patientId}/completed-appointments?appointmentId={appointmentId}&completedDate={completedDateTime}";
        string data = "";

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequestAsync(route, data);
        return JsonHelper.ParseResponse<Sticker>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> DeleteCompletedAppointmentFromPatientAsync(string patientId, string appointmentId)
    {
        string route = $"/api/v1/patient/{patientId}/completed-appointments/{appointmentId}";
        return await webClient.SendDeleteRequestAsync(route);
    }

    public async Awaitable<IWebRequestReponse> ReadJournalEntriesFromPatientAsync(string patientId)
    {
        string route = $"/api/v1/patient/{patientId}/journal-entries";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<JournalEntry>(webRequestResponse);
    }
}