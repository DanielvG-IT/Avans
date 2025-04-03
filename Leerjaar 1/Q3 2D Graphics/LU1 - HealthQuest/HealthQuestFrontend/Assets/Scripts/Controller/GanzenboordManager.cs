using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class GanzenboordManager : MonoBehaviour
{
    public int CompletedLevelCount => completedLevels;
    public int TotalLevels => totalLevelsCount;
    public bool IsReady => isReady;

    // Private vars
    private AppointmentApiClient appointmentApiClient;
    private PatientApiClient patientApiClient;
    private List<Appointment> appointments;
    private List<Appointment> completedAppointments;
    private Treatment currentTreatment;
    private Patient currentPatient;
    private int totalLevelsCount;
    private int completedLevels;
    private bool isReady;

    public async void Start()
    {
        await InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        currentPatient = ApiClientManager.Instance.CurrentPatient;
        currentTreatment = ApiClientManager.Instance.CurrentTreatment;

        patientApiClient = ApiClientManager.Instance.PatientApiClient;
        appointmentApiClient = ApiClientManager.Instance.AppointmentApiClient;

        totalLevelsCount = 0;
        completedLevels = 0;

        await LoadAppointments();
        await LoadCompletedAppointments();
        isReady = true;
    }

    private async Task LoadAppointments()
    {
        if (currentTreatment == null)
        {
            Debug.LogError("Current treatment is null.");
            return;
        }

        var appointmentResult = await appointmentApiClient.ReadAppointmentsByTreatmentIdAsync(currentTreatment.id);
        if (appointmentResult is WebRequestData<List<Appointment>> appointmentSuccess)
        {
            appointments = appointmentSuccess.Data;
            totalLevelsCount = appointments.Count;
        }
        else if (appointmentResult is WebRequestError appointmentError)
        {
            Debug.LogError($"Failed to retrieve appointments: {appointmentError.ErrorMessage}");
        }
    }

    private async Task LoadCompletedAppointments()
    {
        if (currentPatient == null)
        {
            Debug.LogError("Current patient is null.");
            return;
        }

        var completedResult = await patientApiClient.ReadCompletedAppointmentsFromPatientAsync(currentPatient.id);
        if (completedResult is WebRequestData<List<Appointment>> completedSuccess)
        {
            completedAppointments = completedSuccess.Data;
            completedLevels = completedAppointments.Count;
        }
        else if (completedResult is WebRequestError appointmentError)
        {
            Debug.LogError($"Failed to retrieve completed appointments: {appointmentError.ErrorMessage}");
        }
    }

    public void MarkLevelCompleted(int index)
    {
        if (!IsValidIndex(index)) return;
        if (index >= completedLevels) completedLevels = index + 1;
    }

    public bool IsLevelUnlocked(int index) => IsValidIndex(index) && index <= completedLevels;
    public bool IsLevelCompleted(int index) => IsValidIndex(index) && index < completedLevels;
    public Appointment GetAppointment(int index) => IsValidIndex(index) ? appointments[index] : null;
    public void SetCompletedLevelCount(int count) => completedLevels = Mathf.Clamp(count, 0, TotalLevels);

    private bool IsValidIndex(int index) => index >= 0 && index < TotalLevels;
}
