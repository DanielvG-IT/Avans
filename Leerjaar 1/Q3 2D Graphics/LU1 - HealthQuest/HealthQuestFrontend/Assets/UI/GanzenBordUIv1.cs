using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ganzenbord : MonoBehaviour
{
    [Header("PopUpAfspraak")]
    public Image backdrop;
    public Button exitButton;
    public Button doneButton;
    public TMP_Text appointmentTitle;
    public TMP_Text appointmentDescription;
    public TMP_Text appointmentErrorMessage;
    public GameObject[] levelButtons;

    public Camera mainCamera;
    public float cameraSpeed = 5f;

    private bool appoinmentPopUpActive = true;
    private static int selectedLevel = 1;
    private SpriteRenderer levelColorChanger;

    private string[] appointmentTitles = new string[] { "Afspraak 1", "Afspraak 2", "Afspraak 3" };
    private string[] appointmentDescriptions = new string[] { "Description 1", "Description 2", "Description 3" };
    private float[] cameraPositions = new float[] { 283, 316, 467 };
    private bool[] unlockedLevels = new bool[] { true, false, false };

    private Vector3 targetPosition;

    private void Start()
    {
        targetPosition = mainCamera.transform.position;
        ToggleSelection();
    }

    private void Update()
    {
        // Clamp de X-waarde van de target positie
        targetPosition.x = Mathf.Clamp(targetPosition.x, 283f, 500f);

        // Smooth movement van camera naar target positie
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * cameraSpeed);

        // Input voor horizontale beweging (pijltjestoetsen of A/D)
        float horizontalInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            targetPosition += Vector3.right * horizontalInput * 2f;
        }
    }

    public void OnLevelSelected(int level)
    {
        appointmentTitle.text = appointmentTitles[level];
        appointmentDescription.text = appointmentDescriptions[level];
        selectedLevel = level;
        ToggleSelection();
        MoveMainCamera(cameraPositions[level]);
    }

    public void ButtonTest()
    {
        Debug.Log("ButtonTest");
    }

    public void ToggleSelection()
    {
        if (appoinmentPopUpActive)
        {
            appointmentErrorMessage.text = "";
            backdrop.gameObject.SetActive(false);
            exitButton.gameObject.SetActive(false);
            appointmentTitle.gameObject.SetActive(false);
            appointmentDescription.gameObject.SetActive(false);
            doneButton.gameObject.SetActive(false);
            appoinmentPopUpActive = false;
        }
        else
        {
            appointmentErrorMessage.text = "";
            backdrop.gameObject.SetActive(true);
            exitButton.gameObject.SetActive(true);
            appointmentTitle.gameObject.SetActive(true);
            appointmentDescription.gameObject.SetActive(true);
            doneButton.gameObject.SetActive(true);
            appoinmentPopUpActive = true;
        }
    }

    public void MoveMainCamera(float posX)
    {
        targetPosition = new Vector3(posX, mainCamera.transform.position.y, mainCamera.transform.position.z);
    }

    public void DoneWithSelectedLevel()
    {
        if (selectedLevel - 1 >= 0)
        {
            if (unlockedLevels[selectedLevel - 1])
            {
                unlockedLevels[selectedLevel] = true;
                levelColorChanger = levelButtons[selectedLevel].GetComponent<SpriteRenderer>();
                levelColorChanger.color = new Color(0.1548149f, 0.4622642f, 0.1599829f, 1);
                ToggleSelection();
            }
            else
            {
                appointmentErrorMessage.text = "Je moet eerst het vorige level voltooien!";
            }
        }
    }
}
