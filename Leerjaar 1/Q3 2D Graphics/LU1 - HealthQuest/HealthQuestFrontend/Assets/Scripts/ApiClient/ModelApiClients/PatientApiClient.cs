using System;
using UnityEngine;

/// <summary>
/// Provides methods to interact with the Patient API, including CRUD operations for patients,
/// managing stickers, appointments, and journal entries.
/// </summary>
public class PatientApiClient : MonoBehaviour
{
    public WebClient webClient;

    /// <summary>
    /// Retrieves the list of all patients.
    /// </summary>
    /// <returns>
    /// An <see cref="IWebRequestReponse"/> containing the response data parsed as a list of <see cref="Patient"/>.
    /// </returns>
    public async Awaitable<IWebRequestReponse> ReadPatientsAsync()
    {
        string route = $"/api/v1/patient";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<Patient>(webRequestResponse);
    }

    /// <summary>
    /// Retrieves a specific patient by their unique identifier.
    /// </summary>
    /// <param name="patientId">The unique identifier of the patient.</param>
    /// <returns>
    /// An <see cref="IWebRequestReponse"/> containing the response data parsed as a <see cref="Patient"/>.
    /// </returns>
    public async Awaitable<IWebRequestReponse> ReadPatientByIdAsync(string patientId)
    {
        string route = $"/api/v1/patient/{patientId}";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseResponse<Patient>(webRequestResponse);
    }

    /// <summary>
    /// Creates a new patient record.
    /// </summary>
    /// <param name="patientData">The <see cref="Patient"/> object containing the patient's data.</param>
    /// <returns>
    /// An <see cref="IWebRequestReponse"/> containing the response data parsed as a <see cref="Patient"/>.
    /// </returns>
    public async Awaitable<IWebRequestReponse> CreatePatientAsync(Patient patientData)
    {
        string route = $"/api/v1/patient";
        string data = JsonUtility.ToJson(patientData);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequestAsync(route, data);
        return JsonHelper.ParseResponse<Patient>(webRequestResponse);
    }

    /// <summary>
    /// Updates an existing patient record.
    /// </summary>
    /// <param name="patientId">The unique identifier of the patient to update.</param>
    /// <param name="patientData">The <see cref="Patient"/> object containing the updated data.</param>
    /// <returns>
    /// An <see cref="IWebRequestReponse"/> containing the response data parsed as a <see cref="Patient"/>.
    /// </returns>
    public async Awaitable<IWebRequestReponse> UpdatePatient(string patientId, Patient patientData)
    {
        string route = $"/api/v1/patient/{patientId}";
        string data = JsonUtility.ToJson(patientData);

        IWebRequestReponse webRequestResponse = await webClient.SendPutRequestAsync(route, data);
        return JsonHelper.ParseResponse<Patient>(webRequestResponse);
    }

    /// <summary>
    /// Deletes a patient record by their unique identifier.
    /// </summary>
    /// <param name="patientId">The unique identifier of the patient to delete.</param>
    /// <returns>
    /// An <see cref="IWebRequestReponse"/> containing the response of the delete operation.
    /// </returns>
    public async Awaitable<IWebRequestReponse> DeletePatientAsync(string patientId)
    {
        string route = $"/api/v1/patient/{patientId}";
        return await webClient.SendDeleteRequestAsync(route);
    }

    /// <summary>
    /// Retrieves the list of unlocked stickers for a specific patient.
    /// </summary>
    /// <param name="patientId">The unique identifier of the patient.</param>
    /// <returns>
    /// An <see cref="IWebRequestReponse"/> containing the response data parsed as a list of <see cref="Sticker"/>.
    /// </returns>
    public async Awaitable<IWebRequestReponse> ReadUnlockedStickersFromPatientAsync(string patientId)
    {
        string route = $"/api/v1/patient/{patientId}/stickers";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<Sticker>(webRequestResponse);
    }

    /// <summary>
    /// Adds an unlocked sticker to a specific patient.
    /// </summary>
    /// <param name="patientId">The unique identifier of the patient.</param>
    /// <param name="sticker">The <see cref="Sticker"/> object to add.</param>
    /// <returns>
    /// An <see cref="IWebRequestReponse"/> containing the response data parsed as a <see cref="Sticker"/>.
    /// </returns>
    public async Awaitable<IWebRequestReponse> AddUnlockedStickerToPatientAsync(string patientId, Sticker sticker)
    {
        string route = $"/api/v1/patient/{patientId}/stickers?stickerId={sticker.id}";
        string data = JsonUtility.ToJson(sticker);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequestAsync(route, data);
        return JsonHelper.ParseResponse<Sticker>(webRequestResponse);
    }

    /// <summary>
    /// Retrieves the list of appointments for a specific patient.
    /// </summary>
    /// <param name="patientId">The unique identifier of the patient.</param>
    /// <returns>
    /// An <see cref="IWebRequestReponse"/> containing the response data parsed as a list of <see cref="PersonalAppointments"/>.
    /// </returns>
    public async Awaitable<IWebRequestReponse> ReadPersonalAppointmentsFromPatientAsync(string patientId)
    {
        string route = $"/api/v1/patient/{patientId}/appointments";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<PersonalAppointments>(webRequestResponse);
    }

    /// <summary>
    /// Generates the personal appointments for a specific patient.
    /// </summary>
    /// <param name="patientId">The unique identifier of the patient.</param>
    /// <returns>
    /// An <see cref="IWebRequestReponse"/>.
    /// </returns>
    public async Awaitable<IWebRequestReponse> GeneratePersonalAppointmentsForPatientAsync(string patientId)
    {
        string route = $"/api/v1/patient/{patientId}/appointments";
        string data = "";
        return await webClient.SendPostRequestAsync(route, data);
    }

    /// <summary>
    /// Updateds a personal appointment for a specific patient.
    /// </summary>
    /// <param name="patientId">The unique identifier of the patient.</param>
    /// <param name="personalAppointment">The new appointment.</param>
    /// <returns>
    /// An <see cref="IWebRequestReponse"/> containing the response data parsed as a <see cref="PersonalAppointments"/>.
    /// </returns>
    public async Awaitable<IWebRequestReponse> UpdatePersonalAppointmentFromPatientAsync(string patientId, string appointmentId, PersonalAppointments personalAppointment)
    {
        string route = $"/api/v1/patient/{patientId}/appointments/{appointmentId}";
        string data = JsonUtility.ToJson(personalAppointment);

        IWebRequestReponse webRequestResponse = await webClient.SendPutRequestAsync(route, data);
        return JsonHelper.ParseResponse<PersonalAppointments>(webRequestResponse);
    }

    /// <summary>
    /// Updateds a personal appointment for a specific patient.
    /// </summary>
    /// <param name="patientId">The unique identifier of the patient.</param>
    /// <param name="personalAppointment">The new appointment.</param>
    /// <returns>
    /// An <see cref="IWebRequestReponse"/> containing the response data.
    /// </returns>
    public async Awaitable<IWebRequestReponse> CompletePersonalAppointmentFromPatientAsync(string patientId, string appointmentId)
    {
        string route = $"/api/v1/patient/{patientId}/appointments/{appointmentId}/complete";
        string data = "";

        return await webClient.SendPutRequestAsync(route, data);
    }

    /// <summary>
    /// Retrieves the list of journal entries for a specific patient.
    /// </summary>
    /// <param name="patientId">The unique identifier of the patient.</param>
    /// <returns>
    /// An <see cref="IWebRequestReponse"/> containing the response data parsed as a list of <see cref="JournalEntry"/>.
    /// </returns>
    public async Awaitable<IWebRequestReponse> ReadJournalEntriesFromPatientAsync(string patientId)
    {
        string route = $"/api/v1/patient/{patientId}/journal-entries";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<JournalEntry>(webRequestResponse);
    }
}