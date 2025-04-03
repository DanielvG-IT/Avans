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
    void Awake()
    {
        LoadAppointments();
        LoadCompletedAppointments();
    }

    private async void LoadAppointments()
    {
        try
        {
            var response = await apiClientManager.AppointmentApiClient.ReadAppointmentsAsync();

            switch (response)
            {
                case WebRequestData<List<Appointment>> dataResponse:
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

    private async void LoadCompletedAppointments()
    {
        try
        {
            var treatmentId = ApiClientManager.Instance.CurrentTreatment.id;
            var response = await apiClientManager.PatientApiClient.ReadCompletedAppointmentsFromPatientAsync(treatmentId);

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
        }
        catch
        {

            Debug.LogError("Failed to load completed appointments from API.");
        }
    }

    public async Task<bool> MarkLevelCompleted(int index)
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

    public bool IsLevelUnlocked(int index) => IsValidIndex(index) && index <= CompletedLevels;
    public bool IsLevelCompleted(int index) => IsValidIndex(index) && index < CompletedLevels;
    public Appointment GetAppointment(int index) => IsValidIndex(index) ? appointments[index] : null;

    private bool IsValidIndex(int index) => index >= 0 && index < TotalLevels;
}
