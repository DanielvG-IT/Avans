using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the Start, Login, and Registration panels with smooth fade transitions.
/// </summary>
public class StartScreenManager : MonoBehaviour
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
    private OuderVoogdApiClient apiClient;

    private void Start()
    {
        InitializePanels();
        apiClient = ApiClientManager.Instance.OuderVoogdApiClient;
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

        var user = new OuderVoogd
        {
            firstName = firstNameField.text,
            lastName = lastNameField.text,
            email = emailFieldRegister.text,
            password = passwordFieldRegister.text
        };

        try
        {
            var result = await apiClient.Register(user);
            if (result is WebRequestData<string> data)
            {
                Debug.Log("Registration successful: " + data.Data);
                ShowPanel(loginPanel);
            }
            else
            {
                Debug.LogError("Registration failed: " + result);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("An error occurred during registration: " + ex.Message);
        }
    }

    public async void LoginAsync()
    {
        if (string.IsNullOrEmpty(emailFieldLogin.text) || string.IsNullOrEmpty(passwordFieldLogin.text))
        {
            Debug.LogError("Email and password fields cannot be empty.");
            return;
        }

        var user = new OuderVoogd
        {
            email = emailFieldLogin.text,
            password = passwordFieldLogin.text
        };

        try
        {
            var result = await apiClient.Login(user);
            if (result is WebRequestData<string> data)
            {
                Debug.Log("Login successful: " + data.Data);
                await SceneManager.LoadSceneAsync("GameScherm");
            }
            else
            {
                Debug.LogError("Login failed: " + result);
            }
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
