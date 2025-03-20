using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the Start, Login, and Registration panels with smooth fade transitions.
/// </summary>
public class StartScreenManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private CanvasGroup startPanel;
    [SerializeField] private CanvasGroup loginPanel;
    [SerializeField] private CanvasGroup registerPanel;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.5f;

    private CanvasGroup currentPanel;

    private void Start()
    {
        InitializePanels();
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

    // Public UI Methods
    public void ShowLoginPanel() => ShowPanel(loginPanel);
    public void ShowRegisterPanel() => ShowPanel(registerPanel);
    public void BackToStart() => ShowPanel(startPanel);
    public void SwitchToLogin() => ShowPanel(loginPanel);
    public void SwitchToRegister() => ShowPanel(registerPanel);
}
