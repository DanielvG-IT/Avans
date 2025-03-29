using TMPro;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages the Start, Login, and Registration panels with smooth fade transitions.
/// </summary>
public class StartScreen : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private CanvasGroup startPanel;
    [SerializeField] private CanvasGroup loginPanel;
    [SerializeField] private CanvasGroup registerPanel;
    [SerializeField] private TMP_InputField emailFieldLogin;
    [SerializeField] private TMP_InputField passwordFieldLogin;
    [SerializeField] private TMP_InputField emailFieldRegister;
    [SerializeField] private TMP_InputField passwordFieldRegister;
    [SerializeField] private TMP_InputField firstNameField;
    [SerializeField] private TMP_InputField lastNameField;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f;

    private CanvasGroup currentPanel;
    private UserApiClient userApiClient;
    private GuardianApiClient guardianApiClient;

    private void Start()
    {
        InitializePanels();
        userApiClient = ApiClientManager.Instance.UserApiClient;
        guardianApiClient = ApiClientManager.Instance.GuardianApiClient;
    }

    /// <summary>
    /// Initializes the UI by ensuring the Start panel is active and others are hidden.
    /// </summary>
    private void InitializePanels()
    {
        startPanel.gameObject.SetActive(true);
        loginPanel.gameObject.SetActive(false);
        registerPanel.gameObject.SetActive(false);

        startPanel.alpha = 1;
        loginPanel.alpha = 0;
        registerPanel.alpha = 0;

        startPanel.blocksRaycasts = true;
        loginPanel.blocksRaycasts = false;
        registerPanel.blocksRaycasts = false;

        currentPanel = startPanel;
    }

    /// <summary>
    /// Displays the specified panel with a fade transition.
    /// </summary>
    public void ShowPanel(CanvasGroup targetPanel)
    {
        if (targetPanel == currentPanel) return;

        StartCoroutine(SwitchPanel(currentPanel, targetPanel));
        currentPanel = targetPanel;
    }

    /// <summary>
    /// Switches from one panel to another with a smooth fade effect.
    /// </summary>
    private IEnumerator SwitchPanel(CanvasGroup from, CanvasGroup to)
    {
        yield return StartCoroutine(FadePanel(from, false));
        from.gameObject.SetActive(false);

        to.gameObject.SetActive(true);
        yield return StartCoroutine(FadePanel(to, true));
    }

    /// <summary>
    /// Fades a panel in or out over a duration.
    /// </summary>
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

    public async void RegisterAsync()
    {
        if (string.IsNullOrEmpty(emailFieldRegister.text) || string.IsNullOrEmpty(passwordFieldRegister.text) || string.IsNullOrEmpty(firstNameField.text) || string.IsNullOrEmpty(lastNameField.text))
        {
            Debug.LogError("All fields must be filled out.");
            return;
        }

        var user = new User { Email = emailFieldRegister.text, Password = passwordFieldRegister.text };
        var guardian = new Guardian { FirstName = firstNameField.text, LastName = lastNameField.text };

        try
        {
            var registrationResult = await userApiClient.Register(user);
            if (registrationResult is WebRequestError loginError)
            {
                Debug.LogError("Failed to register user: " + loginError.ErrorMessage); // TODO: Show the user an error message
                return;
            }

            var loginResult = await userApiClient.Login(user);
            if (loginResult is WebRequestError registerError)
            {
                Debug.LogError("Failed to login user: " + registerError.ErrorMessage); // TODO: Show the user an error message
                return;
            }

            var guardianResult = await guardianApiClient.CreateGuardian(guardian);
            if (guardianResult is WebRequestError guardianError)
            {
                Debug.LogError("Failed to create guardian: " + guardianError.ErrorMessage); // TODO: Show the user an error message
                return;
            }

            // Store the guardian data in the API client manager
            var guardianData = (guardianResult as WebRequestData<Guardian>).Data;
            ApiClientManager.Instance.SetCurrentGuardian(guardianData);

            Debug.Log("Registration successful."); // TODO: Show the user a success message
            // TODO: Load the patient creation scene
        }
        catch (Exception ex)
        {
            Debug.LogError("An error occurred during login: " + ex.Message); // TODO: Show the user an error message
        }
    }

    public async void LoginAsync()
    {
        if (string.IsNullOrEmpty(emailFieldLogin.text) || string.IsNullOrEmpty(passwordFieldLogin.text))
        {
            Debug.LogError("Email and password fields cannot be empty.");
            return;
        }

        var user = new User { Email = emailFieldLogin.text, Password = passwordFieldLogin.text };

        try
        {
            var loginResult = await userApiClient.Login(user);
            if (loginResult is WebRequestError registerError)
            {
                Debug.LogError("Failed to login user: " + registerError.ErrorMessage); // TODO: Show the user an error message
                return;
            }

            var guardianResult = await guardianApiClient.ReadGuardian();
            if (guardianResult is WebRequestError guardianError)
            {
                Debug.LogError("Failed to login user: " + guardianError.ErrorMessage); // TODO: Show the user an error message
                return;
            }

            var guardianData = (guardianResult as WebRequestData<Guardian>).Data;
            ApiClientManager.Instance.SetCurrentGuardian(guardianData);

            //var patientResults = await patientApiClient.ReadPatients();
            //if (patientResults is WebRequestError patientError)
            //{
            //    Debug.LogError("Failed to login user: " + patientError.ErrorMessage); // TODO: Show the user an error message
            //    return;
            //}
            //List<Patient> patientData = (patientResults as WebRequestData<Patient>).Data;

            //if (!patientData.Any())
            //{
            //    Debug.LogWarning("No patients found for this guardian.");
            //    // TODO: Load the patient scene (creation)
            //}

            Debug.Log("Login successful."); // TODO: Show the user a success message
            // TODO: Load the patient scene (selection
        }
        catch (Exception ex)
        {
            Debug.LogError("An error occurred during login: " + ex.Message);
        }
    }

    // Public UI Methods
    public void ShowLoginPanel() => ShowPanel(loginPanel);
    public void ShowRegisterPanel() => ShowPanel(registerPanel);
    public void BackToStart() => ShowPanel(startPanel);
    public void SwitchToLogin() => ShowPanel(loginPanel);
    public void SwitchToRegister() => ShowPanel(registerPanel);
}
