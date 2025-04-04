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

            if (response is WebRequestData<List<Appointment>> data)
            {
                completedAppointments = data.Data.Count;
            }
            else if (response is WebRequestError error)
            {
                Debug.LogError($"Error: {error.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load completed appointments: {ex.Message}");
        }
    }

    public async Task<bool> MarkLevelCompleted(int index)
    {
        if (!IsValidIndex(index)) return false;

        try
        {
            var appointment = GetAppointment(index);
            var response = await apiClientManager.PatientApiClient.AddCompletedAppointmentsToPatientAsync(
                apiClientManager.CurrentPatient.id,
                appointment.id,
                DateTime.Now
            );

            if (response is WebRequestError error)
            {
                Debug.LogError("Error: " + error.ErrorMessage);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error marking level {index} as completed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> MarkStickerCompleted(string stickerName)
    {
        try
        {
            var stickerResponse = await apiClientManager.StickerApiClient.ReadStickerByNameAsync(stickerName);
            if (stickerResponse is WebRequestError stickerError)
            {
                Debug.LogError($"Error: {stickerError.ErrorMessage}");
                return false;
            }

            if (stickerResponse is not WebRequestData<Sticker> stickerSuccess)
            {
                Debug.LogError("Unexpected response when reading sticker.");
                return false;
            }

            var addResponse = await apiClientManager.PatientApiClient.AddUnlockedStickerToPatientAsync(
                apiClientManager.CurrentPatient.id, stickerSuccess.Data);

            if (addResponse is WebRequestError addError)
            {
                Debug.LogError($"Error: {addError.ErrorMessage}");
                return false;
            }

            if (addResponse is WebRequestData<Sticker>)
            {
                Debug.Log($"Sticker {stickerName} marked as completed.");
                return true;
            }

            Debug.LogError("Unexpected response when adding sticker.");
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception marking sticker {stickerName} as completed: {ex.Message}");
            return false;
        }
    }

    public bool IsLevelUnlocked(int index) => IsValidIndex(index) && index <= CompletedLevels;
    public bool IsLevelCompleted(int index) => IsValidIndex(index) && index < CompletedLevels;
    public Appointment GetAppointment(int index) => IsValidIndex(index) ? appointments[index] : null;

    private bool IsValidIndex(int index) => index >= 0 && index < TotalLevels;
}
