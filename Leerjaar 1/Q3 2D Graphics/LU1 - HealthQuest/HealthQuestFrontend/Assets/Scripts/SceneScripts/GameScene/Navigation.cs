using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Navigation : MonoBehaviour
{
    private Patient currentPatient;
    private Treatment currentTreatment;

    private string currentAvatar;
    private string patientStickerScene;

    public Button StickerButton;
    public Button DagboekButton;

    void Start()
    {
        currentPatient = ApiClientManager.Instance.CurrentPatient;
        currentTreatment = ApiClientManager.Instance.CurrentTreatment;

        patientStickerScene = currentTreatment.name == "Zonder Ziekenhuis Opname" ? "StickerBoek_TrajectZonder" : "StickerBoek_TrajectMet";

        StickerButton.onClick.AddListener(OnStickerClick);
        DagboekButton.onClick.AddListener(OnDagboekClick);
    }

    public void OnStickerClick()
    {
        if (currentPatient != null && !string.IsNullOrEmpty(patientStickerScene))
        {
            SceneManager.LoadScene(patientStickerScene);
        }
    }

    public void OnDagboekClick()
    {
        SceneManager.LoadScene("DagboekScherm");
    }
}
