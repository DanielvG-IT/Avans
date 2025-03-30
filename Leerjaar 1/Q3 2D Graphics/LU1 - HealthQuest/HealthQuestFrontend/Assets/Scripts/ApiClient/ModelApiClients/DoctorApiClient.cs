using System.Numerics;
using UnityEngine;

public class DoctorApiClient : MonoBehaviour
{
    public WebClient webClient;

    public async Awaitable<IWebRequestReponse> ReadDoctorsAsync()
    {
        string route = $"/api/v1/doctors";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<Doctor>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> ReadDoctorByIdAsync(string doctorId)
    {
        string route = $"/api/v1/doctors/{doctorId}";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseResponse<Doctor>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> CreateDoctorAsync(Doctor doctorData)
    {
        string route = $"/api/v1/doctors";
        string data = JsonUtility.ToJson(doctorData);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequestAsync(route, data);
        return JsonHelper.ParseResponse<Doctor>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> UpdateDoctorAsync(string doctorId, Doctor doctorData)
    {
        string route = $"/api/v1/doctors/{doctorId}";
        string data = JsonUtility.ToJson(doctorData);

        IWebRequestReponse webRequestResponse = await webClient.SendPutRequestAsync(route, data);
        return JsonHelper.ParseResponse<Appointment>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> DeleteDoctorAsync(string doctorId)
    {
        string route = $"/api/v1/doctors/{doctorId}";
        return await webClient.SendDeleteRequestAsync(route);
    }

    public async Awaitable<IWebRequestReponse> ReadDoctorFromPatientAsync(string patientId)
    {
        string route = $"/api/v1/doctors/by-patient/{patientId}";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseResponse<Doctor>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> ReadPatientsFromDoctorAsync(string doctorId)
    {
        string route = $"/api/v1/doctors/{doctorId}/patients";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<Patient>(webRequestResponse);
    }
}