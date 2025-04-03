using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class GanzenboordManager : MonoBehaviour
{
    private int completedAppointments;
    public List<Appointment> appointments = new();
    private ApiClientManager apiClientManager => ApiClientManager.Instance;

    public int TotalLevels => appointments.Count;
    public int CompletedLevels => completedAppointments;

    public List<Appointment> Appointments => appointments;
    
    public async void Awake()
    {
        await LoadAppointments();
        await LoadCompletedAppointments();
    }

    private async Task LoadAppointments()
    {
        try
        {
            var treatmentId = ApiClientManager.Instance.CurrentPatient.id;
            var response = await apiClientManager.AppointmentApiClient.ReadAppointmentsByTreatmentIdAsync(apiClientManager.CurrentTreatment.id);
            switch (response)
            {
                case WebRequestData<List<AppointmentWithNr>> dataResponse:
                    {
                        foreach (var appointment in dataResponse.Data)
                        {
                            appointments.Add(new Appointment
                            {
                                name = appointment.name,
                                description = appointment.description
                            });
                        }
                        break;
                    }
                case WebRequestError errorResponse:
                    {
                        Debug.LogError("Error: " + errorResponse.ErrorMessage);
                        break;
                    }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to load appointments from API: " + ex.Message);
        }
    }

    private async Task LoadCompletedAppointments()
    {
        try
        {
            var patientId = ApiClientManager.Instance.CurrentPatient.id;
            var response = await apiClientManager.PatientApiClient.ReadCompletedAppointmentsFromPatientAsync(patientId);
            switch (response)
            {
                case WebRequestData<List<Appointment>> dataResponse:
                    {
                        completedAppointments = dataResponse.Data.Count;
                        break;
                    }

                case WebRequestError errorResponse:
                    {
                        Debug.LogError("Error: " + errorResponse.ErrorMessage);
                        break;
                    }
            }
            Debug.LogWarning(completedAppointments.ToString());
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to load completed appointments from API: " + ex.Message);
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
