using TMPro;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PatientScherm : MonoBehaviour
{
    [Header("UI elementen")]
    public CanvasGroup selectionPanel;
    public CanvasGroup creationPanel;
    public TMP_Text guardianName;

    [Header("Profile elementen")]
    public Transform profilesContainer;
    public GameObject profilePrefab;
    public Sprite VogelAvatar;
    public Sprite PaardAvatar;
    public Sprite HondAvatar;
    public Sprite KatAvatar;

    [Header("Create elementen")]
    public TMP_InputField firstNameInput;
    public TMP_InputField lastNameInput;
    public TMP_Dropdown doctorDropdown;
    public TMP_Dropdown trajectDropdown;
    public TMP_Dropdown avatarDropdown;

    [Header("Animation Settings")]
    public float fadeDuration = 0.5f;

    // Private fields
    private DoctorApiClient doctorApiClient;
    private PatientApiClient patientApiClient;
    private GuardianApiClient guardianApiClient;
    private TreatmentApiClient treatmentApiClient;

    private Guardian guardian;
    private CanvasGroup currentPanel;

    private List<Doctor> doctors;
    private List<Patient> patients;
    private List<Treatment> treatments;

    public async void Start()
    {
        InitializePanels();

        doctorApiClient = ApiClientManager.Instance.DoctorApiClient;
        patientApiClient = ApiClientManager.Instance.PatientApiClient;
        guardianApiClient = ApiClientManager.Instance.GuardianApiClient;
        treatmentApiClient = ApiClientManager.Instance.TreatmentApiClient;

        guardian = ApiClientManager.Instance.CurrentGuardian;
        guardianName.text = $"{guardian.firstName} {guardian.lastName}";

        await LoadSequence();

        if (patients == null || patients.Count == 0)
        {
            ShowCreationPanel();
            patients = new();
        }
    }

    private async Task LoadSequence()
    {
        await LoadPatientData();
        await LoadDropdownData();
        ShowPatientsOnUI();
        PopulateDropdowns();
    }

    private void InitializePanels()
    {
        selectionPanel.gameObject.SetActive(true);
        creationPanel.gameObject.SetActive(false);

        selectionPanel.alpha = 1;
        creationPanel.alpha = 0;

        selectionPanel.blocksRaycasts = true;
        creationPanel.blocksRaycasts = false;

        currentPanel = selectionPanel;
    }

    public void ShowPanel(CanvasGroup targetPanel)
    {
        if (targetPanel == currentPanel) return;

        StartCoroutine(SwitchPanel(currentPanel, targetPanel));
        currentPanel = targetPanel;
    }

    private IEnumerator SwitchPanel(CanvasGroup from, CanvasGroup to)
    {
        yield return StartCoroutine(FadePanel(from, false));
        from.gameObject.SetActive(false);

        to.gameObject.SetActive(true);
        yield return StartCoroutine(FadePanel(to, true));
    }

    private IEnumerator FadePanel(CanvasGroup panel, bool fadeIn)
    {
        float startAlpha = fadeIn ? 0 : 1;
        float endAlpha = fadeIn ? 1 : 0;
        float elapsedTime = 0;

        panel.blocksRaycasts = fadeIn;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            panel.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        panel.alpha = endAlpha;
    }

    private async Task LoadPatientData()
    {
        var patientResult = await patientApiClient.ReadPatientsAsync();
        if (patientResult is WebRequestError patientError)
        {
            if (patientError.StatusCode == 404)
            {
                ShowCreationPanel();
                return;
            }

            Debug.LogError("Failed to read patients: " + patientError.ErrorMessage); // TODO: Show the user an error message
            return;
        }
        else if (patientResult is WebRequestData<List<Patient>> patientData)
        {
            patients = patientData.Data;
        }
    }

    private async Task LoadDropdownData()
    {
        var doctorResult = await doctorApiClient.ReadDoctorsAsync();
        if (doctorResult is WebRequestError doctorError)
        {
            Debug.LogError("Failed to read doctors: " + doctorError.ErrorMessage); // TODO: Show the user an error message
        }
        else if (doctorResult is WebRequestData<List<Doctor>> doctorList)
        {
            doctors = doctorList.Data;

        }

        var treatmentResult = await treatmentApiClient.ReadTreatmentsAsync();
        if (treatmentResult is WebRequestError treatmentError)
        {
            Debug.LogError("Failed to read treatments: " + treatmentError.ErrorMessage); // TODO: Show the user an error message
        }
        else if (treatmentResult is WebRequestData<List<Treatment>> treatmentList)
        {
            treatments = treatmentList.Data;
        } 
    }

    private void PopulateDropdowns()
    {
        if (doctorDropdown == null || avatarDropdown == null || trajectDropdown == null)
        {
            Debug.LogError("One of the dropdown is null");
            return;
        }

        // Populate the doctor dropdown
        var doctorOptions = new List<string>();
        foreach (var doctor in doctors)
        {
            doctorOptions.Add($"{doctor.firstName} {doctor.lastName}");
        }

        doctorDropdown.ClearOptions();
        doctorDropdown.AddOptions(doctorOptions);

        // Populate the avatar dropdown
        var avatarOptions = new List<string> { "Hond", "Kat", "Paard", "Vogel" };
        avatarDropdown.ClearOptions();
        avatarDropdown.AddOptions(avatarOptions);

        // Populate the traject dropdown
        var trajectOptions = new List<string>();
        foreach (var treatment in treatments)
        {
            trajectOptions.Add(treatment.name);
        }
        trajectDropdown.ClearOptions();
        trajectDropdown.AddOptions(trajectOptions);
    }

    private void ShowPatientsOnUI()
    {
        if (patients == null || patients.Count() == 0)
        {
            return;
        }

        foreach (var patient in patients)
        {
            var toInstanciateSprite = patient.avatar switch
            {
                "Hond" => HondAvatar,
                "Kat" => KatAvatar,
                "Paard" => PaardAvatar,
                "Vogel" => VogelAvatar,
                _ => HondAvatar,
            };

            GameObject profileCard = Instantiate(profilePrefab, profilesContainer);

            Image profileImage = profileCard.transform.Find("Avatar").GetComponent<Image>();
            profileImage.sprite = toInstanciateSprite;

            Text profileName = profileCard.transform.Find("Text").GetComponent<Text>();
            profileName.text = $"{patient.firstName} {patient.lastName}";

            Button btn = profileCard.GetComponent<Button>();
            btn.onClick.AddListener(() => SelectPatient(patient));
        }
    }

    public async void CreatePatient()
    {
        try
        {
            string selectedDoctorName = doctorDropdown.options[doctorDropdown.value].text;
            string selectedTreatmentName = trajectDropdown.options[trajectDropdown.value].text;

            var selectedDoctor = doctors.FirstOrDefault(d => $"{d.firstName} {d.lastName}" == selectedDoctorName);
            var selectedTreatment = treatments.FirstOrDefault(t => t.name == selectedTreatmentName);

            if (selectedDoctor == null || selectedTreatment == null)
            {
                Debug.LogError("Invalid selection: doctor or treatment not found");
                return;
            }

            var newPatient = new Patient
            {
                id = Guid.NewGuid().ToString(),
                guardianID = guardian.id,
                firstName = firstNameInput.text,
                lastName = lastNameInput.text,
                doctorID = selectedDoctor.id,
                treatmentID = selectedTreatment.id,
                avatar = avatarDropdown.options[avatarDropdown.value].text
            };

            var createResult = await patientApiClient.CreatePatientAsync(newPatient);
            if (createResult is WebRequestError createError)
            {
                Debug.LogError("Failed to create patient: " + createError.ErrorMessage);
                return;
            }
            else if (createResult is WebRequestData<Patient> createdPatient)
            {
                patients.Add(createdPatient.Data);
                await LoadSequence();
                ShowSelectionPanel();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to create patient: " + e.Message);
        }
    }

    public void Logout()
    {
        ApiClientManager.Instance.ClearData();
        SceneManager.LoadScene("StartScherm");
    }

    public void BackOnCreation()
    {
        if (patients.Count() != 0)
        {
            ShowSelectionPanel();
        }
        else
        {
            Logout();
        }
    }

    private async void SelectPatient(Patient patient)
    {
        try
        {
            // Verify the patient exists
            var verifyResult = await patientApiClient.ReadPatientByIdAsync(patient.id);
            if (verifyResult is WebRequestError verifyError)
            {
                Debug.LogError("Failed to verify patient: " + verifyError.ErrorMessage);
                return;
            }

            var treatmentResponse = await treatmentApiClient.ReadTreatmentByIdAsync(patient.treatmentID);
            if (treatmentResponse is WebRequestError treatmentError)
            {
                Debug.LogError("Failed to verify treatment: " + treatmentError.ErrorMessage);
                return;
            }

            ApiClientManager.Instance.SetCurrentPatient(patient);
            ApiClientManager.Instance.SetCurrentTreatment((treatmentResponse as WebRequestData<Treatment>).Data);

            await SceneManager.LoadSceneAsync("TussenScherm");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to set current patient: " + e.Message);
        }
    }

    public async void SelectParent()
    {
        try
        {
            // Verify the patient exists
            var verifyResult = await guardianApiClient.ReadGuardianById(guardian.id);
            if (verifyResult is WebRequestError verifyError)
            {
                Debug.LogError("Failed to verify patient: " + verifyError.ErrorMessage);
                return;
            }
            else if (verifyResult is WebRequestData<Guardian> parentData)
            {
                if (patients.Count() != 0)
                {
                    await SceneManager.LoadSceneAsync("MonitorScherm");
                }
                else
                {
                    ShowCreationPanel();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load parent: " + e.Message);
        }
    }

    // Public UI Methods
    public void ShowSelectionPanel() => ShowPanel(selectionPanel);
    public void ShowCreationPanel() => ShowPanel(creationPanel);
}
