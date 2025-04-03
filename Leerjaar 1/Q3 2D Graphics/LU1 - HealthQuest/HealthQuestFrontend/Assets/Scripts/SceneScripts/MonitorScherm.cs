using TMPro;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using System;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MonitorScherm : MonoBehaviour
{
    [Header("TopField")]
    public TMP_Text dagboekEntries;
    public TMP_Text dagboekGemRating;
    public TMP_Text zorgtraject;
    public TMP_Text zorgtrajectProgressie;
    //public TMP_Text patientName;
    public TMP_Text afspraakTitel;
    public TMP_Text afspraakBeschrijving;

    [Header("BottomField")]
    public TMP_Text journalTitle;
    public TMP_Text journalDescription;
    public TMP_Text journalRating;
    public TMP_Text journalEntryDate;

    [Header("LeftBar")]
    public Transform journalView;
    public Transform appointmentView;
    public GameObject ButtonPrefab;

    [Header ("TopBar")]
    public TMP_Dropdown childSelector;

    private PatientApiClient patientApiClient;
    private TreatmentApiClient treatmentApiClient;
    private AppointmentApiClient appointmentApiClient;
    private List<Patient> patients;
    private Patient selectedPatient;
    private Treatment selectedTreatment;
    private List<Appointment> completedAppointments;
    private double gemiddeldeRating;


    public async void Start()
    {
        patientApiClient = ApiClientManager.Instance.PatientApiClient;
        treatmentApiClient = ApiClientManager.Instance.TreatmentApiClient;
        appointmentApiClient = ApiClientManager.Instance.AppointmentApiClient;

        await LoadPatients();
        await LoadProgress();
        await LoadTreatment();
        await LoadAppointments();
        await LoadJournalEntriesAsync();
        UpdatePatientUI();
    }

    private async void LoadSequence()
    {
        ClearData();
        await LoadTreatment();
        await LoadProgress();
        await LoadAppointments();
        await LoadJournalEntriesAsync();
        UpdatePatientUI();
    }

    public void Back()
    {
        SceneManager.LoadScene("PatientScherm");
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

    private async Task LoadAppointments()
    {
        if (selectedTreatment == null)
        {
            Debug.LogError("Current treatment is null, cannot retrieve appointments");
            return;
        }
        else
        {
            var appointmentResult = await appointmentApiClient.ReadAppointmentsByTreatmentIdAsync(selectedTreatment.id);
            if(appointmentResult is WebRequestError appointmentError)
            {
                Debug.LogError(appointmentError.ErrorMessage);
                return;
            }
            else if (appointmentResult is WebRequestData<List<AppointmentWithNr>> appointmentData)
            {
                foreach (Transform child in appointmentView)
                {
                    Destroy(child.gameObject);
                }

                foreach (var appointment in appointmentData.Data)
                {
                    Debug.Log("Creating button: appointment" + appointment.description);

                    GameObject newButtonAppointment = Instantiate(ButtonPrefab, appointmentView);
                    TMP_Text buttonText = newButtonAppointment.GetComponentInChildren<TMP_Text>();

                    if (buttonText != null)
                    {
                        buttonText.text = appointment.name;

                        Button btnComponent = newButtonAppointment.GetComponent<Button>();
                        if (btnComponent != null)
                        {
                            btnComponent.onClick.AddListener(() => OnAppointmentSelected(appointment));
                        }
                    }
                }    
            }
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
            //patientName.text = $"{selectedPatient.firstName} {selectedPatient.lastName}";
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
        gemiddeldeRating = 0;
        if (selectedPatient == null) return;

        var journalEntriesResult = await patientApiClient.ReadJournalEntriesFromPatientAsync(selectedPatient.id);

        if (journalEntriesResult is WebRequestError error)
        {
            Debug.LogError(error.ErrorMessage);
        }
        else if (journalEntriesResult is WebRequestData<List<JournalEntry>> journalResponse)
        {
            var sortedList = journalResponse.Data.OrderByDescending(a => a.date);

            dagboekEntries.text = journalResponse.Data.Count().ToString();
            foreach (Transform child in journalView)
            {
                Destroy(child.gameObject);
            }

            foreach (var journal in sortedList)
            {
                Debug.Log("Creating button journal: " + journal.content.ToString());
                gemiddeldeRating += journal.rating;

                GameObject newButtonJournal = Instantiate(ButtonPrefab, journalView);
                TMP_Text buttonText = newButtonJournal.GetComponentInChildren<TMP_Text>();

                if (buttonText != null)
                {
                    buttonText.text = journal.title;

                    Button btnComponent = newButtonJournal.GetComponent<Button>();
                    if (btnComponent != null)
                    {
                        btnComponent.onClick.AddListener(() => OnJournalSelected(journal));
                    }
                }


            }
            if (journalResponse.Data.Count() == 0)
            {
                gemiddeldeRating = 0;
                dagboekGemRating.text = gemiddeldeRating.ToString();
            }
            else
            {
                gemiddeldeRating /= journalResponse.Data.Count();
                gemiddeldeRating = Math.Round(gemiddeldeRating, 1);
                dagboekGemRating.text = gemiddeldeRating.ToString();
            }
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

    private void OnJournalSelected(JournalEntry journalEntry)
    { 
        journalTitle.text = journalEntry.title;
        journalDescription.text = journalEntry.content;
        journalRating.text = $"{journalEntry.rating.ToString()}/10";
        Debug.Log("Datetime bij afspraak: " + journalEntry.date);
        journalEntryDate.text = journalEntry.date;
    }

    private void OnAppointmentSelected(Appointment appointment)
    {
        afspraakTitel.text = appointment.name;
        afspraakBeschrijving.text = appointment.description;
    }

    private void ClearData()
    {
        dagboekEntries.text = "";
        dagboekGemRating.text = "";
        zorgtraject.text = "";
        zorgtrajectProgressie.text = "";
        afspraakTitel.text = "";
        afspraakBeschrijving.text = "";
        journalTitle.text = "";
        journalRating.text = "";
        journalDescription.text = "";
        journalEntryDate.text = "";
    }
}
