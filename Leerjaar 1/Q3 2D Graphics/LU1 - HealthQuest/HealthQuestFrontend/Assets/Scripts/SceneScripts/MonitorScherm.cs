using TMPro;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using System;
using System.Threading.Tasks;

public class MonitorScherm : MonoBehaviour
{
    [Header("TopField")]
    public TMP_Text dagboekEntries;
    public TMP_Text dagboekGemRating;
    public TMP_Text zorgtraject;
    public TMP_Text zorgtrajectProgressie;
    public TMP_Text patientName;

    [Header("BottomField")]
    public TMP_Text journalTitle;
    public TMP_Text journalDescription;

    public TMP_Dropdown childSelector;

    private PatientApiClient patientApiClient;
    private TreatmentApiClient treatmentApiClient;
    private List<Patient> patients;
    private Patient selectedPatient;
    private Treatment selectedTreatment;
    private List<Appointment> completedAppointments;


    public async void Start()
    {
        patientApiClient = ApiClientManager.Instance.PatientApiClient;
        treatmentApiClient = ApiClientManager.Instance.TreatmentApiClient;

        await LoadPatients();
        await LoadProgress();
        await LoadTreatment();
        await LoadJournalEntriesAsync();
        UpdatePatientUI();
    }

    private async void LoadSequence()
    {
        await LoadTreatment();
        await LoadProgress();
        await LoadJournalEntriesAsync();
        UpdatePatientUI();
    }

    private async Task LoadPatients()
    {
        var patientResult = await patientApiClient.ReadPatientsAsync();

        if (patientResult is WebRequestError patientError)
        {
            Debug.LogError(patientError.ErrorMessage);
            return;
        }
        else if (patientResult is WebRequestData<List<Patient>> kinderenVanVoogd)
        {
            patients = kinderenVanVoogd.Data;
            selectedPatient = patients.FirstOrDefault();
            PopulateDropdown();
        }
    }

    private async Task LoadTreatment()
    {
        var treatmentResponse = await treatmentApiClient.ReadTreatmentByIdAsync(selectedPatient.treatmentID);
        if (treatmentResponse is WebRequestError treatmentError)
        {
            Debug.LogError(treatmentError.ErrorMessage);
            return;
        }
        else if (treatmentResponse is WebRequestData<Treatment> treatment)
        {
            selectedTreatment = treatment.Data;
        }
    }

    private async Task LoadProgress()
    {
        var progressResponse = await patientApiClient.ReadCompletedAppointmentsFromPatientAsync(selectedPatient.id);
        if (progressResponse is WebRequestError progressError)
        {
            Debug.LogError(progressError.ErrorMessage);
            return;
        }
        else if (progressResponse is WebRequestData<List<Appointment>> progress)
        {
            completedAppointments = progress.Data;
        }
    }

    private void UpdatePatientUI()
    {
        if (selectedPatient != null)
        {
            patientName.text = $"{selectedPatient.firstName} {selectedPatient.lastName}";
            zorgtraject.text = selectedTreatment.name.Replace("Ziekenhuis ", "");

            int completedCount;
            if (completedAppointments == null)
                completedCount = 0; 
            else
                completedCount = completedAppointments.Count();

            int totalAppointments = selectedTreatment.name == "Met Ziekenhuis Opname" ? 14 : 10;
            zorgtrajectProgressie.text = $"{completedCount}/{totalAppointments}";
        }
    }

    private async Task LoadJournalEntriesAsync()
    {
        if (selectedPatient == null) return;

        var journalEntriesResult = await patientApiClient.ReadJournalEntriesFromPatientAsync(selectedPatient.id);

        if (journalEntriesResult is WebRequestError error)
        {
            Debug.LogError(error.ErrorMessage);
        }
        else if (journalEntriesResult is WebRequestData<List<JournalEntry>> journalResponse)
        {
            dagboekEntries.text = journalResponse.Data.Count().ToString();
            //TODO: gemiddelde cijfer berekenen van child dagboek entries.
        }
    }


    private void PopulateDropdown()
    {
        if (patients == null || childSelector == null)
        {
            Debug.LogError("Patient list or dropdown is null");
            return;
        }

        childSelector.ClearOptions();

        List<string> patientNames = patients.Select(p => $"{p.firstName} {p.lastName}").ToList();
        childSelector.AddOptions(patientNames);

        // Ensure selection event is only assigned once 
        childSelector.onValueChanged.RemoveAllListeners();
        childSelector.onValueChanged.AddListener(OnPatientSelected);

        Debug.Log("Dropdown populated and listener added.");
    }

    // Called when a new patient is selected from the dropdown
    private void OnPatientSelected(int index)
    {
        if (index < 0 || index >= patients.Count) return;

        selectedPatient = patients[index];
        Debug.Log($"Selected patient: {selectedPatient.firstName} {selectedPatient.lastName}");

        LoadSequence();
    }
}
