using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class GanzenboordManager : MonoBehaviour
{
    public List<PersonalAppointments> personalAppointments = new();
    public List<Appointment> appointments = new();

    // ApiClientManagers
    private StickerApiClient stickerApiClient;
    private PatientApiClient patientApiClient;
    private AppointmentApiClient appointmentApiClient;

    public async Task Initialize()
    {
        stickerApiClient = ApiClientManager.Instance.StickerApiClient;
        patientApiClient = ApiClientManager.Instance.PatientApiClient;
        appointmentApiClient = ApiClientManager.Instance.AppointmentApiClient;

        await LoadAllAppointments();
    }

    public async Task LoadAllAppointments()
    {
        var patientId = ApiClientManager.Instance.CurrentPatient.id;
        try
        {

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
            Debug.LogError($"Failed to load appointments: {ex.Message}");
        }
    }

    public async Task<bool> MarkLevelCompleted(int index)
    {
        if (!IsValidIndex(index)) return false;

        try
        {
            var personalAppointment = GetPersonalAppointment(index);
            personalAppointment.completedDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            // Update the personal appointment
            var response = await patientApiClient.UpdatePersonalAppointmentFromPatientAsync(
                ApiClientManager.Instance.CurrentPatient.id,
                personalAppointment.id,
                personalAppointment
            );

            // Handle any errors that occurred during the request
            if (response is WebRequestError error)
            {
                Debug.LogError($"Error updating appointment: {error.ErrorMessage}");
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

    public bool IsLevelUnlocked(int index)
    {
        if (!IsValidIndex(index))
            return false;

        // Level 0 is always unlocked
        if (index == 0)
            return true;

        // Previous level must be completed
        return !string.IsNullOrEmpty(personalAppointments[index - 1].completedDate);
    }

    public bool IsLevelCompleted(int index)
    {
        return IsValidIndex(index) && !string.IsNullOrEmpty(personalAppointments[index].completedDate);
    }

    public Appointment GetAppointment(int index)
    {
        return IsValidIndex(index) ? appointments[index] : null;
    }

    public PersonalAppointments GetPersonalAppointment(int index)
    {
        return IsValidIndex(index) ? personalAppointments[index] : null;
    }

    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < personalAppointments.Count;
    }

}