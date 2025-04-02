using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Navigation : MonoBehaviour
{
    private Patient currentPatient;
    private Treatment currentTreatment;

    private string currentAvatar;
    private string patientStickerScene;

    public Button Sticker;
    // private Button Dagboek;

    void Start()
    {
        currentPatient = ApiClientManager.Instance.CurrentPatient;
        currentTreatment = ApiClientManager.Instance.CurrentTreatment;
        if (currentTreatment.name == "Zonder Ziekenhuis Opname") patientStickerScene = "StickerBoek_TrajectZonder" ?? "StickerBoek_TrajectMet";

        Sticker.onClick.AddListener(OnStickerClick);
    }

    void OnStickerClick()
    {
        if (currentPatient != null && !string.IsNullOrEmpty(patientStickerScene))
        {
            SceneManager.LoadScene(patientStickerScene);
        }
    }

    // void OnDagboekClick()
    // {
    //     SceneManager.LoadScene("TargetSceneName");
    // }
}
