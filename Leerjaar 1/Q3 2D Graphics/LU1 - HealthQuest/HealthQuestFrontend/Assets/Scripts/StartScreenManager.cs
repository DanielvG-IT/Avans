using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIManager handles the switching between Start, Login, and Registration panels with smooth fade-in and fade-out animations.
/// </summary>
public class StartScreenManager : MonoBehaviour
{
    [Header("UI Panels")]
    public CanvasGroup startPanel;
    public CanvasGroup loginPanel;
    public CanvasGroup registerPanel;

    [Header("Animation Settings")]
    public float fadeDuration = 0.5f;

    /// <summary>
    /// Initializes the UI by ensuring the start panel is active and others are hidden.
    /// </summary>
    private void Start()
    {
        // Ensure only the Start Panel is visible at launch
        SetPanelActive(startPanel, true, false);
        SetPanelActive(loginPanel, false, false);
        SetPanelActive(registerPanel, false, false);
    }

    /// <summary>
    /// Transitions from the Start Panel to the Login Panel.
    /// </summary>
    public void ShowLoginPanel()
    {
        StartCoroutine(SwitchPanel(startPanel, loginPanel));
    }

    /// <summary>
    /// Transitions from the Start Panel to the Registration Panel.
    /// </summary>
    public void ShowRegisterPanel()
    {
        StartCoroutine(SwitchPanel(startPanel, registerPanel));
    }

    /// <summary>
    /// Returns to the Start Panel from either Login or Registration Panel.
    /// </summary>
    public void BackToStart()
    {
        if (loginPanel.gameObject.activeSelf)
            StartCoroutine(SwitchPanel(loginPanel, startPanel));
        else if (registerPanel.gameObject.activeSelf)
            StartCoroutine(SwitchPanel(registerPanel, startPanel));
    }

    /// <summary>
    /// Switches from Registration Panel to Login Panel.
    /// </summary>
    public void SwitchToLogin()
    {
        StartCoroutine(SwitchPanel(registerPanel, loginPanel));
    }

    /// <summary>
    /// Switches from Login Panel to Registration Panel.
    /// </summary>
    public void SwitchToRegister()
    {
        StartCoroutine(SwitchPanel(loginPanel, registerPanel));
    }

    /// <summary>
    /// Handles the transition between two panels with fade-in and fade-out effects.
    /// </summary>
    /// <param name="from">The panel to fade out.</param>
    /// <param name="to">The panel to fade in.</param>
    private IEnumerator SwitchPanel(CanvasGroup from, CanvasGroup to)
    {
        yield return StartCoroutine(FadePanel(from, false)); // Fade out current panel
        SetPanelActive(from, false, true); // Deactivate previous panel

        SetPanelActive(to, true, false); // Activate new panel
        yield return StartCoroutine(FadePanel(to, true)); // Fade in new panel
    }

    /// <summary>
    /// Fades a panel in or out over time.
    /// </summary>
    /// <param name="panel">The panel to fade.</param>
    /// <param name="fadeIn">If true, the panel fades in; otherwise, it fades out.</param>
    private IEnumerator FadePanel(CanvasGroup panel, bool fadeIn)
    {
        float startAlpha = fadeIn ? 0 : 1; // Initial alpha value
        float endAlpha = fadeIn ? 1 : 0;   // Target alpha value
        float elapsedTime = 0; // Timer

        panel.gameObject.SetActive(true); // Ensure panel is active
        panel.blocksRaycasts = fadeIn; // Enable or disable interaction

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            panel.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        panel.alpha = endAlpha; // Ensure final alpha is correctly set
        if (!fadeIn)
            panel.gameObject.SetActive(false); // Deactivate panel when faded out
    }

    /// <summary>
    /// Activates or deactivates a panel instantly, without animations.
    /// </summary>
    /// <param name="panel">The panel to modify.</param>
    /// <param name="active">If true, the panel is shown; otherwise, it is hidden.</param>
    /// <param name="instant">If true, the panel switches instantly without fading.</param>
    private void SetPanelActive(CanvasGroup panel, bool active, bool instant)
    {
        panel.gameObject.SetActive(active);
        panel.alpha = active ? 1 : 0;
        panel.blocksRaycasts = active;
        panel.interactable = active;
    }
}
