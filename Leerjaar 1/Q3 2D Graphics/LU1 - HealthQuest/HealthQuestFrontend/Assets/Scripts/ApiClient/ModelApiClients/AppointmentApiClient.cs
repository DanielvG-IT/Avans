using UnityEngine;

/// <summary>
/// Handles API interactions related to appointments.
/// Provides methods to perform CRUD operations on appointments and retrieve appointments by specific criteria.
/// </summary>
public class AppointmentApiClient : MonoBehaviour
{
    /// <summary>
    /// The WebClient instance used to send HTTP requests.
    /// </summary>
    public WebClient webClient;

    /// <summary>
    /// Retrieves a list of all appointments from the API.
    /// </summary>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing a list of appointments.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request fails.</exception>
    public async Awaitable<IWebRequestReponse> ReadAppointmentsAsync()
    {
        string route = $"/api/v1/appointments";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<Appointment>(webRequestResponse);
    }

    /// <summary>
    /// Retrieves a specific appointment by its unique identifier.
    /// </summary>
    /// <param name="appointmentId">The unique identifier of the appointment to retrieve.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing the appointment details.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="appointmentId"/> is null or empty.</exception>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request fails.</exception>
    public async Awaitable<IWebRequestReponse> ReadAppointmentByIdAsync(string appointmentId)
    {
        string route = $"/api/v1/appointments/{appointmentId}";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseResponse<Appointment>(webRequestResponse);
    }

    /// <summary>
    /// Creates a new appointment in the API.
    /// </summary>
    /// <param name="appointmentData">The data of the appointment to create.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing the created appointment details.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="appointmentData"/> is null.</exception>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request fails.</exception>
    public async Awaitable<IWebRequestReponse> CreateAppointmentAsync(Appointment appointmentData)
    {
        string route = $"/api/v1/appointments";
        string data = JsonUtility.ToJson(appointmentData);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequestAsync(route, data);
        return JsonHelper.ParseResponse<Appointment>(webRequestResponse);
    }

    /// <summary>
    /// Updates an existing appointment in the API.
    /// </summary>
    /// <param name="appointmentId">The unique identifier of the appointment to update.</param>
    /// <param name="appointmentData">The updated data for the appointment.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing the updated appointment details.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="appointmentId"/> or <paramref name="appointmentData"/> is null or empty.</exception>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request fails.</exception>
    public async Awaitable<IWebRequestReponse> UpdateAppointmentAsync(string appointmentId, Appointment appointmentData)
    {
        string route = $"/api/v1/appointments/{appointmentId}";
        string data = JsonUtility.ToJson(appointmentData);

        IWebRequestReponse webRequestResponse = await webClient.SendPutRequestAsync(route, data);
        return JsonHelper.ParseResponse<Appointment>(webRequestResponse);
    }

    /// <summary>
    /// Deletes an appointment from the API.
    /// </summary>
    /// <param name="appointmentId">The unique identifier of the appointment to delete.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> indicating the result of the delete operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="appointmentId"/> is null or empty.</exception>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request fails.</exception>
    public async Awaitable<IWebRequestReponse> DeleteAppointmentAsync(string appointmentId)
    {
        string route = $"/api/v1/appointments/{appointmentId}";
        return await webClient.SendDeleteRequestAsync(route);
    }

    /// <summary>
    /// Retrieves a list of appointments associated with a specific treatment ID.
    /// </summary>
    /// <param name="treatmentId">The unique identifier of the treatment to filter appointments by.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing a list of appointments.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="treatmentId"/> is null or empty.</exception>
    /// <exception cref="HttpRequestException">Thrown if the HTTP request fails.</exception>
    public async Awaitable<IWebRequestReponse> ReadAppointmentsByTreatmentIdAsync(string treatmentId)
    {
        string route = $"/api/v1/appointments?treatmentId={treatmentId}";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<Appointment>(webRequestResponse);
    }
}