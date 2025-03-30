using System.Numerics;
using UnityEngine;

/// <summary>
/// Handles API interactions related to doctors.
/// Provides methods to perform CRUD operations on doctors.
/// </summary>
public class DoctorApiClient : MonoBehaviour
{
    /// <summary>
    /// The WebClient instance used to send HTTP requests.
    /// </summary>
    public WebClient webClient;

    /// <summary>
    /// Retrieves a list of all doctors from the API.
    /// </summary>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing a list of <see cref="Doctor"/> objects.
    /// </returns>
    /// <exception cref="WebRequestException">Thrown if the GET request fails.</exception>
    public async Awaitable<IWebRequestReponse> ReadDoctorsAsync()
    {
        string route = $"/api/v1/doctors";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<Doctor>(webRequestResponse);
    }

    /// <summary>
    /// Retrieves a specific doctor by their unique identifier.
    /// </summary>
    /// <param name="doctorId">The unique identifier of the doctor to retrieve.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing a <see cref="Doctor"/> object.
    /// </returns>
    /// <exception cref="WebRequestException">Thrown if the GET request fails.</exception>
    public async Awaitable<IWebRequestReponse> ReadDoctorByIdAsync(string doctorId)
    {
        string route = $"/api/v1/doctors/{doctorId}";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseResponse<Doctor>(webRequestResponse);
    }

    /// <summary>
    /// Creates a new doctor in the system.
    /// </summary>
    /// <param name="doctorData">The <see cref="Doctor"/> object containing the data for the new doctor.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing the created <see cref="Doctor"/> object.
    /// </returns>
    /// <exception cref="WebRequestException">Thrown if the POST request fails.</exception>
    public async Awaitable<IWebRequestReponse> CreateDoctorAsync(Doctor doctorData)
    {
        string route = $"/api/v1/doctors";
        string data = JsonUtility.ToJson(doctorData);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequestAsync(route, data);
        return JsonHelper.ParseResponse<Doctor>(webRequestResponse);
    }

    /// <summary>
    /// Updates an existing doctor's information.
    /// </summary>
    /// <param name="doctorId">The unique identifier of the doctor to update.</param>
    /// <param name="doctorData">The <see cref="Doctor"/> object containing the updated data.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing the updated <see cref="Appointment"/> object.
    /// </returns>
    /// <exception cref="WebRequestException">Thrown if the PUT request fails.</exception>
    public async Awaitable<IWebRequestReponse> UpdateDoctorAsync(string doctorId, Doctor doctorData)
    {
        string route = $"/api/v1/doctors/{doctorId}";
        string data = JsonUtility.ToJson(doctorData);

        IWebRequestReponse webRequestResponse = await webClient.SendPutRequestAsync(route, data);
        return JsonHelper.ParseResponse<Appointment>(webRequestResponse);
    }

    /// <summary>
    /// Deletes a doctor from the system.
    /// </summary>
    /// <param name="doctorId">The unique identifier of the doctor to delete.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> indicating the result of the operation.
    /// </returns>
    /// <exception cref="WebRequestException">Thrown if the DELETE request fails.</exception>
    public async Awaitable<IWebRequestReponse> DeleteDoctorAsync(string doctorId)
    {
        string route = $"/api/v1/doctors/{doctorId}";
        return await webClient.SendDeleteRequestAsync(route);
    }

    /// <summary>
    /// Retrieves the doctor associated with a specific patient.
    /// </summary>
    /// <param name="patientId">The unique identifier of the patient.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing the associated <see cref="Doctor"/> object.
    /// </returns>
    /// <exception cref="WebRequestException">Thrown if the GET request fails.</exception>
    public async Awaitable<IWebRequestReponse> ReadDoctorFromPatientAsync(string patientId)
    {
        string route = $"/api/v1/doctors/by-patient/{patientId}";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseResponse<Doctor>(webRequestResponse);
    }

    /// <summary>
    /// Retrieves a list of patients associated with a specific doctor.
    /// </summary>
    /// <param name="doctorId">The unique identifier of the doctor.</param>
    /// <returns>
    /// An awaitable task that resolves to an <see cref="IWebRequestReponse"/> containing a list of <see cref="Patient"/> objects.
    /// </returns>
    /// <exception cref="WebRequestException">Thrown if the GET request fails.</exception>
    public async Awaitable<IWebRequestReponse> ReadPatientsFromDoctorAsync(string doctorId)
    {
        string route = $"/api/v1/doctors/{doctorId}/patients";

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequestAsync(route);
        return JsonHelper.ParseListResponse<Patient>(webRequestResponse);
    }
}