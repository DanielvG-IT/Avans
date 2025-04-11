using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TussenScherm : MonoBehaviour
{
    private Patient currentPatient;
    private Treatment currentTreatment;

    private string currentAvatar;
    private string traject;
    private string patientGameScene;

    public TMP_Text patientName;
    public Button logoutButton;
    public Button startGame;
    public Button gameCompleted;

    [Header("Avatar")]
    public Image avatar;
    public Sprite Kat;
    public Sprite Hond;
    public Sprite Paard;
    public Sprite Vogel;

    public void Start()
    {
        //TODO: Gametraject ended later miss bij herkansing (voor reminders na traject etc.)
        gameCompleted.gameObject.SetActive(false);

        currentPatient = ApiClientManager.Instance.CurrentPatient;
        currentTreatment = ApiClientManager.Instance.CurrentTreatment;

        patientGameScene = currentTreatment.name == "Zonder Ziekenhuis Opname" ? "GameTrajectZonder" : "GameTrajectMet";

        patientName.text = currentPatient.firstName + " " + currentPatient.lastName;
        currentAvatar = currentPatient.avatar;
        traject = currentPatient.treatmentID;

        switch (currentAvatar)
        {
            case "Kat":
                avatar.sprite = Kat;
                break;
            case "Hond":
                avatar.sprite = Hond;
                break;
            case "Paard":
                avatar.sprite = Paard;
                break;
            case "Vogel":
                avatar.sprite = Vogel;
                break;
            default:
                Debug.LogWarning("Avatar not found.");
                break;
        }

        logoutButton.onClick.AddListener(Logout);
        startGame.onClick.AddListener(() => LoadGameScene(patientGameScene));
    }

    public void Logout()
    {
        SceneManager.LoadScene("StartScherm");
        ApiClientManager.Instance.ClearData();
    }

    public void LoadGameScene(string trajectScene)
    {
        SceneManager.LoadScene(trajectScene);
    }
}