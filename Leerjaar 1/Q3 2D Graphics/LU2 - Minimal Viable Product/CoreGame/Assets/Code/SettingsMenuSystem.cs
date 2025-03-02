using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SettingsMenuSystem : MonoBehaviour
{
    [Header("UI References")]
    public GameObject settingsMenu;
    public Button settingsButton;
    public TMP_InputField InputEnvName;
    public TMP_InputField InputEnvMaxHeight;
    public TMP_InputField InputEnvMaxLength;

    [Header("Scripts Dependencies")]
    public EnvironmentSystem environmentSystem;

    // Internal
    private Environment2D currentEnvironment;

    void Start()
    {
        currentEnvironment = GameManager.Instance.SelectedEnvironment;
        
        settingsMenu.SetActive(false);

        UpdateInputFields();
    }

    private void UpdateInputFields()
    {
        if (currentEnvironment == null) 
            GoBackToMainMenu();

        InputEnvName.text = currentEnvironment.name;
        InputEnvMaxHeight.text = currentEnvironment.maxHeight.ToString();
        InputEnvMaxLength.text = currentEnvironment.maxLength.ToString();
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
        
        // SceneManager.LoadScene("Environment");
    }

    public async void DeleteEnvironmentSettingsMenu()
    { 
        var success = await environmentSystem.DeleteEnvironment2D();

        if (success)
        {
            settingsMenu.SetActive(false);
            GoBackToMainMenu();
        }
    }

    public void ToggleSettingsMenu()
    {
        settingsMenu.SetActive(!settingsMenu.activeSelf);
    }
}
