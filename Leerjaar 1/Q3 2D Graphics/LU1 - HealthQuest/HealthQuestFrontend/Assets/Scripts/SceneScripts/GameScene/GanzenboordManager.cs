using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class GanzenboordManager : MonoBehaviour
{
    public List<PersonalAppointments> personalAppointments;
    public List<Appointment> appointments;

    // ApiClientManagers
    private StickerApiClient stickerApiClient;
    private PatientApiClient patientApiClient;
    private AppointmentApiClient appointmentApiClient;

    public async Task Initialize()
    {
        stickerApiClient = ApiClientManager.Instance.StickerApiClient;
        patientApiClient = ApiClientManager.Instance.PatientApiClient;
        appointmentApiClient = ApiClientManager.Instance.AppointmentApiClient;

        await LoadAppointments();
        await LoadPersonalAppointments();
    }

    private async Task LoadAppointments()
    {
        try
        {
            var treatmentId = ApiClientManager.Instance.CurrentTreatment.id;
            var response = await appointmentApiClient.ReadAppointmentsByTreatmentIdAsync(treatmentId);

            if (response is WebRequestData<List<Appointment>> dataResponse)
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

    public async Task LoadAllAppointments()
    {
        try
        {
            var patientId = ApiClientManager.Instance.CurrentPatient.id;
            var personalAppointmentsResults = await patientApiClient.ReadPersonalAppointmentsFromPatientAsync(patientId);
            if (personalAppointmentsResults is WebRequestError error)
            {
                Debug.LogWarning("Error loading patient appointments: " + error);
                return;
            }
            else if (personalAppointmentsResults is WebRequestData<List<PersonalAppointments>> data)
            {
                personalAppointments.Clear();
                appointments.Clear();

                personalAppointments = data.Data;

                foreach (var appointment in data.Data)
                {
                    var result = await appointmentApiClient.ReadAppointmentByIdAsync(appointment.appointmentID);
                    if (result is WebRequestError errorke)
                    {
                        Debug.LogError("Errorke is niet goed gegaan: " + errorke.ErrorMessage);
                        return;
                    }
                    else if (result is WebRequestData<Appointment> appointmentData)
                    {
                        appointments.Add(appointmentData.Data);
                    }
                }
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
            //var appointment = GetAppointment(index);
            //var response = await apiClientManager.PatientApiClient.AddCompletedAppointmentsToPatientAsync(
            //    apiClientManager.CurrentPatient.id,
            //    appointment.id,
            //    DateTime.Now
            //);

            //if (response is WebRequestError error)
            //{
            //    Debug.LogError("Error: " + error.ErrorMessage);
            //    return false;
            //}

            //return true;
            return false; // TODO: Fix this methode
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
            var stickerResponse = await stickerApiClient.ReadStickerByNameAsync(stickerName);
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

            var addResponse = await patientApiClient.AddUnlockedStickerToPatientAsync(
                ApiClientManager.Instance.CurrentPatient.id, stickerSuccess.Data
            );

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
