using TMPro;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenuSystem : MonoBehaviour
{
    [Header("UI References")]
    public GameObject settingsMenu;
    public TMP_Text SettingsName;
    public TMP_InputField InputEnvName;
    public TMP_InputField InputEnvMaxHeight;
    public TMP_InputField InputEnvMaxLength;

    [Header("Scripts Dependencies")]
    public EnvironmentSystem environmentSystem;
    public GameObject player;

    // Internal
    private Environment2D currentEnvironment;

    void Start()
    {
        currentEnvironment = GameManager.Instance.SelectedEnvironment;

        settingsMenu.SetActive(false);

        UpdateSettingsFields();
    }

    private void UpdateSettingsFields()
    {
        SettingsName.text = currentEnvironment.name;
        InputEnvName.text = currentEnvironment.name;
        InputEnvMaxHeight.text = currentEnvironment.maxHeight.ToString();
        InputEnvMaxLength.text = currentEnvironment.maxLength.ToString();
    }

    private void UpdatePlayer()
    {
        // Update player boundaries for walking
        var playerScript = player.GetComponent<Player>();
        playerScript.ResetMovementValues();
    }

    public void GoBackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public async void UpdateEnvironment()
    {
        var updatedEnvironment = new Environment2D
        {
            id = currentEnvironment.id,
            name = InputEnvName.text,
            maxHeight = Convert.ToInt32(InputEnvMaxHeight.text),
            maxLength = Convert.ToInt32(InputEnvMaxLength.text),
        };

        await environmentSystem.UpdateEnvironment2D(updatedEnvironment);

        currentEnvironment = GameManager.Instance.SelectedEnvironment;

        UpdatePlayer();
        UpdateSettingsFields();
    }

    public async void DeleteEnvironmentSettingsMenu()
    {
        var success = await environmentSystem.DeleteEnvironment2D();

        if (success)
        {
            settingsMenu.SetActive(false);

            // Ensure data updates before returning to the main menu
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }

    public void ToggleSettingsMenu()
    {
        settingsMenu.SetActive(!settingsMenu.activeSelf);
    }
}
