using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;

public class ArtsScherm : MonoBehaviour
{
    [Header("Top Field")]
    public TMP_Text doctorName;
    public TMP_Text patientName;
    public TMP_Text patientZorgTraject;
    public TMP_Text afspraakTitel;
    public TMP_Text afspraakBeschrijving;
    public TMP_Text appointmentNr;

    [Header("Side Bar")]
    public GameObject doctorPatientPrefab;
    public Transform doctorPatientsContainer;

    //Api clients
    private DoctorApiClient doctorApiClient;
    private PatientApiClient patientApiClient;
    private TreatmentApiClient treatmentApiClient;
    private AppointmentApiClient appointmentApiClient;

    private Doctor currentDoctor;
    private List<Patient> doctorPatients = new();
    private List<PersonalAppointments> patientPersonalAppointments = new();
    private List<Appointment> patientAppointments = new();
    private Patient selectedPatient;
    private Treatment selectedPatientTreatment;
    private int appointmentPage = 0;

    private async void Start()
    {
        doctorApiClient = ApiClientManager.Instance.DoctorApiClient;
        patientApiClient = ApiClientManager.Instance.PatientApiClient;
        treatmentApiClient = ApiClientManager.Instance.TreatmentApiClient;
        appointmentApiClient = ApiClientManager.Instance.AppointmentApiClient;

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

    public async void OnSelectedPatient(Patient patient)
    {
        afspraakTitel.text = string.Empty;
        afspraakBeschrijving.text = string.Empty;
        patientZorgTraject.text = string.Empty;

        selectedPatient = patient;
        await LoadTreatment();
        await LoadPatientAppointments();
        patientName.text = $"{patient.firstName} {patient.lastName}";
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
        if (goingForward)
        {
            if (appointmentPage + 1 <= patientAppointments.Count - 1)
            {
                appointmentPage++;
                afspraakTitel.text = patientAppointments[appointmentPage].name;
                afspraakBeschrijving.text = patientAppointments[appointmentPage].description;
                appointmentNr.text = $"{appointmentPage + 1}/{patientAppointments.Count}";
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
            }
        }
    }
}
