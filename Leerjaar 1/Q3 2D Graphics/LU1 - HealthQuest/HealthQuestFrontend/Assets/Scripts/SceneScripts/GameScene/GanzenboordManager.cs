using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class GanzenboordManager : MonoBehaviour
{
    public int CompletedLevels => completedAppointments;
    public int TotalLevels => appointments.Count;
    public List<AppointmentWithNr> Appointments => appointments;

    private int completedAppointments;
    private ApiClientManager apiClientManager;
    private List<AppointmentWithNr> appointments;

    //public async void Awake() // May change to start (what is better?)
    //{
    //    await Initialize();
    //}

    public async Task Initialize()
    {
        apiClientManager = ApiClientManager.Instance;

        await LoadAppointments();
        await LoadCompletedAppointments();
    }

    private async Task LoadAppointments()
    {
        try
        {
            var treatmentId = apiClientManager.CurrentTreatment.id;
            var response = await apiClientManager.AppointmentApiClient.ReadAppointmentsByTreatmentIdAsync(treatmentId);

            if (response is WebRequestData<List<AppointmentWithNr>> dataResponse)
            {
                appointments = dataResponse.Data;
            }
            else if (response is WebRequestError errorResponse)
            {
                Debug.LogError($"Error: {errorResponse.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load appointments from API: {ex.Message}");
        }
    }

    private async Task LoadCompletedAppointments()
    {
        try
        {
            var patientId = ApiClientManager.Instance.CurrentPatient.id;
            var response = await apiClientManager.PatientApiClient.ReadCompletedAppointmentsFromPatientAsync(patientId);

            if (response is WebRequestData<List<Appointment>> dataResponse)
            {
                completedAppointments = dataResponse.Data.Count;
            }
            else if (response is WebRequestError errorResponse)
            {
                Debug.LogError($"Error: {errorResponse.ErrorMessage}");
            }

            Debug.LogWarning(completedAppointments.ToString());
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load completed appointments from API: {ex.Message}");
        }
    }


    public async Task<bool> MarkLevelCompleted(int index)
    {
        try
        {
            if (!IsValidIndex(index))
            {
                return false;
            }

            var addResponse = await apiClientManager.PatientApiClient.AddCompletedAppointmentsToPatientAsync(
                apiClientManager.CurrentPatient.id,
                GetAppointment(index).id,
                DateTime.Now
            );

            if (addResponse is WebRequestError errorResponse)
            {
                Debug.LogError("Error: " + errorResponse.ErrorMessage);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to mark level {index} as completed: " + ex.Message);
            return false;
        }
    }

    public bool IsLevelUnlocked(int index) => IsValidIndex(index) && index <= CompletedLevels;
    public bool IsLevelCompleted(int index) => IsValidIndex(index) && index < CompletedLevels;
    public Appointment GetAppointment(int index) => IsValidIndex(index) ? appointments[index] : null;

    private bool IsValidIndex(int index) => index >= 0 && index < TotalLevels;
}
