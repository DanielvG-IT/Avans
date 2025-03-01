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

    void Start()
    {
        settingsMenu.SetActive(false);
    }

    public async void GoBackToMainMenu()
    {
        await SceneManager.LoadSceneAsync("MainMenu");
    }

    public async void UpdateEnvironment()
    {
        var updatedEnvironment = new Environment2D
        {
            id = GameManager.Instance.SelectedEnvironment.id,
            name = InputEnvName.text,
            maxHeight = Convert.ToInt32(InputEnvMaxHeight.text),
            maxLength = Convert.ToInt32(InputEnvMaxLength.text),
        };

        await environmentSystem.UpdateEnvironment2D(updatedEnvironment);
        await ReloadEnvironment();
    }

    public async Task ReloadEnvironment()
    {
        await SceneManager.LoadSceneAsync("Environment");
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
