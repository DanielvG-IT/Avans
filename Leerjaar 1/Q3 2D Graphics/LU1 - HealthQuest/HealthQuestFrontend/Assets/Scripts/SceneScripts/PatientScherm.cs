using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PatientScherm : MonoBehaviour
{
    [Header("UI elementen")]
    public GameObject schermPrefab;
    private CanvasGroup selectionPanel;
    private CanvasGroup creationPanel;

    [Header("Prefabs")]
    public GameObject HondProfilePrefab;
    public GameObject KatProfilePrefab;
    public GameObject PaardProfilePrefab;
    public GameObject VogelProfilePrefab;

    [Header("Animation Settings")]
    public float fadeDuration = 0.5f;

    private CanvasGroup currentPanel;

    private List<Patient> patients;
    private PatientApiClient PatientApiClient;

    public async void Start()
    {
        InitializePanels();

        PatientApiClient = ApiClientManager.Instance.PatientApiClient;

        var patientResult = await PatientApiClient.ReadPatientAsync();
        if (patientResult is WebRequestError patientError)
        {
            Debug.LogError("Failed to read patients: " + patientError.ErrorMessage); // TODO: Show the user an error message
            return;
        }
        else if (patientResult is WebRequestData<List<Patient>> patientData)
        {
            patients = patientData.Data;
        }

        ShowPatientsOnUI();
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

    private void ShowPatientsOnUI()
    {
        foreach (var patient in patients)
        {
            switch (patient.avatar)
            {
                    // TODO: Add buttons to prefab select this patient
                case "Hond":
                    Instantiate(HondProfilePrefab, schermPrefab.transform);
                    break;
                case "Kat":
                    Instantiate(KatProfilePrefab, schermPrefab.transform);
                    break;
                case "Paard":
                    Instantiate(PaardProfilePrefab, schermPrefab.transform);
                    break;
                case "Vogel":
                    Instantiate(VogelProfilePrefab, schermPrefab.transform);
                    break;
                default:
                    Debug.LogWarning("Unknown avatar type: " + patient.avatar);
                    break;
            }
            // TODO: Set the patient's name and other details on the profile prefab
            // TODO: Check if the transform values need to be adjusted
        }
    }

    private async void SelectPatient(Patient patient)
    {
        try
        {
            // Verify the patient exists
            var verifyResult = await PatientApiClient.ReadPatientByIdAsync(patient.id);
            if (verifyResult is WebRequestError verifyError)
            {
                Debug.LogError("Failed to verify patient: " + verifyError.ErrorMessage);
                return;
            }

            ApiClientManager.Instance.SetCurrentPatient(patient);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to set current patient: " + e.Message);
        }
    }

    // Public UI Methods
    public void ShowSelectionPanel() => ShowPanel(selectionPanel);
    public void ShowCreationPanel() => ShowPanel(creationPanel);
}
