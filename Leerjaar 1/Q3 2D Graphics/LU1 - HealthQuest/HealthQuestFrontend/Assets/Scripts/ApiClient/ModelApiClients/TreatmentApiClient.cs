using System;
using UnityEngine;

/// <summary>
/// A client for interacting with the Treatment API.
/// Provides methods to perform CRUD operations on treatments and manage related appointments.
/// </summary>
public class TreatmentApiClient : MonoBehaviour
{
    /// <summary>
    /// The web client used to send HTTP requests.
    /// </summary>
    public WebClient webClient;

    /// <summary>
    /// Retrieves a list of all treatments from the API.
    /// </summary>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing a list of treatments.
    /// </returns>
    /// <exception cref="Exception">Thrown if the request fails or the response cannot be parsed.</exception>
    public async Awaitable<IWebRequestReponse> ReadTreatmentsAsync()
    {
        string route = $"/api/v1/treatments";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<Treatment>(webRequestResponse);
    }

    /// <summary>
    /// Retrieves a specific treatment by its ID from the API.
    /// </summary>
    /// <param name="treatmentId">The unique identifier of the treatment to retrieve.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing the treatment details.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="treatmentId"/> is null or empty.</exception>
    /// <exception cref="Exception">Thrown if the request fails or the response cannot be parsed.</exception>
    public async Awaitable<IWebRequestReponse> ReadTreatmentByIdAsync(string treatmentId)
    {
        string route = $"/api/v1/treatments/{treatmentId}";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseResponse<Treatment>(webRequestResponse);
    }

    /// <summary>
    /// Creates a new treatment in the API.
    /// </summary>
    /// <param name="treatmentData">The treatment data to create.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing the created treatment details.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="treatmentData"/> is null.</exception>
    /// <exception cref="Exception">Thrown if the request fails or the response cannot be parsed.</exception>
    public async Awaitable<IWebRequestReponse> CreateTreatmentAsync(Treatment treatmentData)
    {
        string route = $"/api/v1/treatments";
        string data = JsonUtility.ToJson(treatmentData);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequestAsync(route, data);
        return JsonHelper.ParseResponse<Treatment>(webRequestResponse);
    }

    /// <summary>
    /// Updates an existing treatment in the API.
    /// </summary>
    /// <param name="treatmentId">The unique identifier of the treatment to update.</param>
    /// <param name="treatmentData">The updated treatment data.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing the updated treatment details.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="treatmentId"/> or <paramref name="treatmentData"/> is null.</exception>
    /// <exception cref="Exception">Thrown if the request fails or the response cannot be parsed.</exception>
    public async Awaitable<IWebRequestReponse> UpdateTreatmentAsync(string treatmentId, Treatment treatmentData)
    {
        string route = $"/api/v1/treatments/{treatmentId}";
        string data = JsonUtility.ToJson(treatmentData);

        IWebRequestReponse webRequestResponse = await webClient.SendPutRequestAsync(route, data);
        return JsonHelper.ParseResponse<Patient>(webRequestResponse);
    }

    /// <summary>
    /// Deletes a treatment from the API.
    /// </summary>
    /// <param name="treatmentId">The unique identifier of the treatment to delete.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> indicating the result of the operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="treatmentId"/> is null or empty.</exception>
    /// <exception cref="Exception">Thrown if the request fails.</exception>
    public async Awaitable<IWebRequestReponse> DeleteTreatmentAsync(string treatmentId)
    {
        string route = $"/api/v1/treatments/{treatmentId}";
        return await webClient.SendDeleteRequestAsync(route);
    }

    /// <summary>
    /// Retrieves a list of appointments associated with a specific treatment.
    /// </summary>
    /// <param name="treatmentId">The unique identifier of the treatment.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing a list of appointments.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="treatmentId"/> is null or empty.</exception>
    /// <exception cref="Exception">Thrown if the request fails or the response cannot be parsed.</exception>
    public async Awaitable<IWebRequestReponse> ReadAppointmentsByTreatmentIdAsync(string treatmentId)
    {
        string route = $"/api/v1/treatments/{treatmentId}/appointments";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<Appointment>(webRequestResponse);
    }

    /// <summary>
    /// Adds an appointment to a specific treatment.
    /// </summary>
    /// <param name="treatmentId">The unique identifier of the treatment.</param>
    /// <param name="appointmentId">The unique identifier of the appointment to add.</param>
    /// <param name="sequence">The sequence number for the appointment.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> indicating the result of the operation.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="treatmentId"/> or <paramref name="appointmentId"/> is null or empty.</exception>
    /// <exception cref="Exception">Thrown if the request fails.</exception>
    public async Awaitable<IWebRequestReponse> AddAppointmentToTreatmentAsync(string treatmentId, string appointmentId, int sequence)
    {
        string route = $"/api/v1/treatments/{treatmentId}/appointments/{appointmentId}?sequence={sequence}";
        string data = "";

        // Returns 204 No Content on success so no need to parse response
        return await webClient.SendPostRequestAsync(route, data);
    }
}