using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Globalization;
using System;
using UnityEditor.Rendering;

public class ArtsScherm : MonoBehaviour
{
    [Header("Top Field")]
    public TMP_Text doctorName;
    public TMP_Text patientName;
    public TMP_Text patientZorgTraject;
    public TMP_Text afspraakTitel;
    public TMP_Text afspraakBeschrijving;
    public TMP_Text appointmentNr;
    public TMP_InputField dateSetter;

    [Header("Side Bar")]
    public GameObject doctorPatientPrefab;
    public Transform doctorPatientsContainer;

    [Header("Bottom Field")]
    public GameObject BottomPanel; 
    public GameObject journalPrefab;
    public Transform journalContainer;
    public TMP_Text journalTitle;
    public TMP_Text journalDescription;
    public TMP_Text journalDate;
    public TMP_Text journalRating;
    public TMP_Text journalAverageRating;

    [Header("LockedOut")]
    public GameObject lockedOutPanel;
    public TMP_Text lockedOutPatient;

    //Api clients
    private DoctorApiClient doctorApiClient;
    private PatientApiClient patientApiClient;
    private TreatmentApiClient treatmentApiClient;
    private AppointmentApiClient appointmentApiClient;
    private JournalApiClient journalApiClient;

    private Doctor currentDoctor;
    private List<Patient> doctorPatients = new();
    private List<PersonalAppointments> patientPersonalAppointments = new();
    private List<Appointment> patientAppointments = new();
    private Patient selectedPatient;
    private Treatment selectedPatientTreatment;
    private int appointmentPage = 0;
    private double averageRating = 0;

    private async void Start()
    {
        doctorApiClient = ApiClientManager.Instance.DoctorApiClient;
        patientApiClient = ApiClientManager.Instance.PatientApiClient;
        treatmentApiClient = ApiClientManager.Instance.TreatmentApiClient;
        appointmentApiClient = ApiClientManager.Instance.AppointmentApiClient;
        journalApiClient = ApiClientManager.Instance.JournalApiClient;

        await LoadCurrentDoctor();
        await LoadDoctorPatientsInWindow();
    }

    public void OnLogoutButtonClicked()
    {
        SceneManager.LoadScene("StartScherm");
    }

    public async Task LoadCurrentDoctor()
    {
        var doctorResult = await ApiClientManager.Instance.DoctorApiClient.ReadDoctorByUserID();
        if (doctorResult is WebRequestError error)
        {
            Debug.Log($"Jammer ouwe : {error.ErrorMessage}");
            return;
        }
        else if (doctorResult is WebRequestData<Doctor> doctorData)
        {
            currentDoctor = doctorData.Data;
            doctorName.text = $"{currentDoctor.firstName} {currentDoctor.lastName}";
        }
    }

    public async Task LoadDoctorPatientsInWindow()
    {
        var patientResult = await doctorApiClient.ReadPatientsFromDoctorAsync(currentDoctor.id);
        if (patientResult is WebRequestError patientError)
        {
            Debug.LogError("Failed to read patients: " + patientError.ErrorMessage); // TODO: Show the user an error message
            return;
        }
        else if (patientResult is WebRequestData<List<Patient>> patientData)
        {
            doctorPatients = patientData.Data;
        }

        foreach (Transform child in doctorPatientsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var patient in doctorPatients)
        {

            GameObject patientCard = Instantiate(doctorPatientPrefab, doctorPatientsContainer);
            TMP_Text patientCardText = patientCard.GetComponentInChildren<TMP_Text>();

            if (patientCardText != null)
            {
                patientCardText.text = patient.firstName + " " + patient.lastName;

                Button btnComponent = patientCard.GetComponent<Button>();
                if (btnComponent is not null)
                {
                    btnComponent.onClick.AddListener(() => OnSelectedPatient(patient));
                }
            }
        }
    }

    private void ImplementPatientPrivacySettings(bool doctorHasAccess)
    {
        if (!doctorHasAccess)
        {
            BottomPanel.gameObject.SetActive(false);
            lockedOutPanel.SetActive(true);
            lockedOutPatient.text = $"{selectedPatient.firstName} {selectedPatient.lastName}";
        }
        else
        {
            BottomPanel.gameObject.SetActive(true);
            lockedOutPanel.SetActive(false);
            lockedOutPatient.text = string.Empty;
        }
    }

    public async void OnSelectedPatient(Patient patient)
    {
        selectedPatient = patient;

        ImplementPatientPrivacySettings(selectedPatient.doctorAccessJournal);
        ClearInfoTextFields();
        await LoadTreatment();
        await LoadPatientAppointments();
        await LoadPatientJournalEntries();
        patientName.text = $"{patient.firstName} {patient.lastName}";
    }

    public void OnSelectedJournalEntry(JournalEntry journalEntry)
    {
        journalTitle.text = journalEntry.title;
        journalDescription.text = journalEntry.content;
        journalDate.text = journalEntry.date;
        journalRating.text = "Cijfer: " + journalEntry.rating.ToString();
        journalAverageRating.text = "Gemiddeld cijfer: " + averageRating.ToString();
    }

    public void ClearInfoTextFields()
    {
        journalTitle.text = string.Empty;
        journalDescription.text = string.Empty;
        journalDate.text = string.Empty;
        journalRating.text = string.Empty;
        journalAverageRating.text = string.Empty;

        afspraakTitel.text = string.Empty;
        afspraakBeschrijving.text = string.Empty;
        appointmentNr.text = string.Empty;
        patientZorgTraject.text = string.Empty;

        appointmentPage = 0;
        patientAppointments.Clear();

        foreach (Transform child in journalContainer)
        {
            Destroy(child.gameObject);
        }
    }

    private async Task LoadPatientAppointments()
    {
        var personalAppointmentsResults = await patientApiClient.ReadPersonalAppointmentsFromPatientAsync(selectedPatient.id);
        if (personalAppointmentsResults is WebRequestError error)
        {
            Debug.LogWarning("Error loading patient appointments: " + error);
            return;
        }
        else if (personalAppointmentsResults is WebRequestData<List<PersonalAppointments>> data)
        {
            patientPersonalAppointments.Clear();
            patientAppointments.Clear();

            patientPersonalAppointments = data.Data;

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
                    patientAppointments.Add(appointmentData.Data);
                }
            }
            appointmentNr.text = $"{appointmentPage + 1}/{patientAppointments.Count}";
            afspraakTitel.text = patientAppointments[appointmentPage].name;
            afspraakBeschrijving.text = patientAppointments[appointmentPage].description;
            patientZorgTraject.text = selectedPatientTreatment.name;
            if(patientPersonalAppointments[appointmentPage].appointmentDate is not null)
                dateSetter.text = patientPersonalAppointments[appointmentPage].appointmentDate;
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
            selectedPatientTreatment = treatment.Data;
        }
    }

    public void CycleAppointments(bool goingForward)
    {
        if (patientAppointments.Count == 0)
        {
            return;
        }
        if (goingForward)
        {
            if (appointmentPage + 1 <= patientAppointments.Count - 1)
            {
                appointmentPage++;
                afspraakTitel.text = patientAppointments[appointmentPage].name;
                afspraakBeschrijving.text = patientAppointments[appointmentPage].description;
                appointmentNr.text = $"{appointmentPage + 1}/{patientAppointments.Count}";
                if (patientPersonalAppointments[appointmentPage].appointmentDate is not null)
                    dateSetter.text = patientPersonalAppointments[appointmentPage].appointmentDate;
            }
        }
        else
        {
            if (appointmentPage - 1 >= 0)
            {
                appointmentPage--;
                afspraakTitel.text = patientAppointments[appointmentPage].name;
                afspraakBeschrijving.text = patientAppointments[appointmentPage].description;
                appointmentNr.text = $"{appointmentPage + 1}/{patientAppointments.Count}";
                if (patientPersonalAppointments[appointmentPage].appointmentDate is not null)
                    dateSetter.text = patientPersonalAppointments[appointmentPage].appointmentDate;            }
        }
    }

    private async Task LoadPatientJournalEntries()
    {
        var result = await journalApiClient.ReadJournalEntriesAsync(selectedPatient.id);
        if (result is WebRequestError error)
        {
            Debug.LogError("Failed to load journal entries: " + error.ErrorMessage);
            return;
        }
        else if (result is WebRequestData<List<JournalEntry>> journalEntries)
        {
            averageRating = 0;

            foreach (Transform child in journalContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (var entry in journalEntries.Data)
            {
                averageRating += entry.rating;
                GameObject journalCard = Instantiate(journalPrefab, journalContainer);
                TMP_Text journalCardText = journalCard.GetComponentInChildren<TMP_Text>();

                if (journalCardText != null)
                {
                    journalCardText.text = entry.title;
                    Button btnComponent = journalCard.GetComponent<Button>();
                    if (btnComponent is not null)
                    {
                        btnComponent.onClick.AddListener(() => OnSelectedJournalEntry(entry));
                    }
                }
            }
            averageRating /= journalEntries.Data.Count;
            averageRating = System.Math.Round(averageRating, 1);
        }
    }

    public async void SetAppointmentDate()
    {
        string filledInDate = dateSetter.text;
        string format = "yyyy-MM-dd";

        CultureInfo provider = CultureInfo.InvariantCulture;

        if (DateTime.TryParseExact(filledInDate, format, provider, DateTimeStyles.None, out DateTime result))
        {
            patientPersonalAppointments[appointmentPage].appointmentDate = result.ToString();
            var resultUpdatedAppointment = await patientApiClient.UpdatePersonalAppointmentFromPatientAsync(selectedPatient.id, patientPersonalAppointments[appointmentPage].id, patientPersonalAppointments[appointmentPage]);

            if(resultUpdatedAppointment is WebRequestError error)
            {
                Debug.LogWarning("Error updating appointment date: " + error.ErrorMessage);
            }
            else if (resultUpdatedAppointment is WebRequestData<PersonalAppointments> updatedAppointment)
            {
                Debug.Log("Appointment date updated successfully.");
            }
        }
        else
        {
            Debug.LogWarning("Invalid date format.");
        }
    }
}