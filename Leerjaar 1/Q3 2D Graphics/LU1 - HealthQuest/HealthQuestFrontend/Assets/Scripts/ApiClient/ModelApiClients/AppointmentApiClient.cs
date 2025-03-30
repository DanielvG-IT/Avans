using UnityEngine;

public class AppointmentApiClient : MonoBehaviour
{
    public WebClient webClient;

    public async Awaitable<IWebRequestReponse> ReadAppointmentsAsync()
    {
        string route = "/api/v1/appointments";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<Appointment>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> ReadAppointmentByIdAsync(string appointmentId)
    {
        string route = "/api/v1/appointments/" + appointmentId;

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseResponse<Appointment>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> CreateAppointmentAsync(Appointment appointmentData)
    {
        string route = "/api/v1/appointments";
        string data = JsonUtility.ToJson(appointmentData);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequestAsync(route, data);
        return JsonHelper.ParseResponse<Appointment>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> UpdateAppointmentAsync(string appointmentId, Appointment appointmentData)
    {
        string route = "/api/v1/appointments/" + appointmentId;
        string data = JsonUtility.ToJson(appointmentData);

        IWebRequestReponse webRequestResponse = await webClient.SendPutRequestAsync(route, data);
        return JsonHelper.ParseResponse<Appointment>(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> DeleteAppointmentAsync(string appointmentId)
    {
        string route = "/api/v1/appointments/" + appointmentId;
        return await webClient.SendDeleteRequestAsync(route);
    }

    public async Awaitable<IWebRequestReponse> ReadAppointmentsByTreatmentIdAsync(string treatmentId)
    {
        string route = "/api/v1/appointments?treatmentId=" + treatmentId;

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<Appointment>(webRequestResponse);
    }
}