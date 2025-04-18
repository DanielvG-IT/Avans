using TMPro;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

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
    [SerializeField] private TMP_InputField firstNameField;
    [SerializeField] private TMP_InputField lastNameField;
    [SerializeField] private TMP_InputField emailFieldRegister;
    [SerializeField] private TMP_InputField passwordFieldRegister;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f;

    private CanvasGroup currentPanel;
    private UserApiClient userApiClient;
    private GuardianApiClient guardianApiClient;

    private void Start()
    {
        AudioManager.Instance.StartMusic();
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
            Debug.LogError("All fields must be filled out."); // TODO: Show the user an error message
            return;
        }

        var user = new User { email = emailFieldRegister.text, password = passwordFieldRegister.text };
        var guardian = new Guardian { firstName = firstNameField.text, lastName = lastNameField.text };

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

            var guardianResult = await guardianApiClient.CreateGuardianAsync(guardian);
            if (guardianResult is WebRequestError guardianError)
            {
                Debug.LogError("Failed to create guardian: " + guardianError.ErrorMessage); // TODO: Show the user an error message
                return;
            }
            else if (guardianResult is WebRequestData<Guardian> guardianData)
            {
                Debug.Log("Guardian created successfully."); // TODO: Show the user a success message
                ApiClientManager.Instance.SetCurrentGuardian(guardianData.Data);
            }

            Debug.Log("Registration successful."); // TODO: Show the user a success message
            await SceneManager.LoadSceneAsync("PatientScherm");
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

        var user = new User { email = emailFieldLogin.text, password = passwordFieldLogin.text };

        try
        {
            var loginResult = await userApiClient.Login(user);
            if (loginResult is WebRequestError registerError)
            {
                Debug.LogError("Failed to login user: " + registerError.ErrorMessage); // TODO: Show the user an error message
                return;
            }

            // Check roles
            var rolesResult = await userApiClient.GetRole();
            var rolesData = rolesResult as WebRequestData<string>;
            if (rolesData != null && rolesData.Data != null)
            {
                if (rolesData.Data.Contains("[\"Doctor\"]"))
                {
                    AudioManager.Instance.StopMusic();
                    await SceneManager.LoadSceneAsync("ArtsScherm");
                    return;
                }
            }

            var guardianResult = await guardianApiClient.ReadGuardianAsync();
            if (guardianResult is WebRequestError guardianError)
            {
                Debug.LogError("Failed to login user: " + guardianError.ErrorMessage); // TODO: Show the user an error message
                return;
            }

            var guardianData = (guardianResult as WebRequestData<Guardian>)?.Data;
            if (guardianData is null)
            {
                Debug.LogError("Guardian data is null."); // TODO: Show the user an error message
                return;
            }

            ApiClientManager.Instance.SetCurrentGuardian(guardianData);

            if (string.IsNullOrEmpty(guardianData.firstName) || string.IsNullOrEmpty(guardianData.lastName))
            {
                Debug.LogWarning("Guardian data is incomplete."); // TODO: Show the user a warning message
            }
            else
            {
                Debug.Log($"{guardianData.firstName}, {guardianData.lastName}");
            }

            Debug.Log("Login successful."); // TODO: Show the user a success message
            AudioManager.Instance.audioSource.PlayOneShot(AudioManager.Instance.soundEffects[0]);
            await SceneManager.LoadSceneAsync("PatientScherm");
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