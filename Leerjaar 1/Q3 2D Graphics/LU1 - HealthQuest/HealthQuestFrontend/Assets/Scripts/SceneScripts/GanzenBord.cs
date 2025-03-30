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

    private bool appoinmentPopUpActive = true;
    private static int selectedLevel = 1;
    private SpriteRenderer levelColorChanger;

    private string[] appointmentTitles = new string[] { "Afspraak 1", "Afspraak 2", "Afpsraak 3" };
    private string[] appointmentDescriptions = new string[] { "Description 1", "Description 2", "Description 3" };
    private float[] cameraPositions = new float[] { 283, 316, 467 };
    private bool[] unlockedLevels = new bool[] { true, false, false };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        ToggleSelection();
    }

    // Update is called once per frame
    private void Update()
    {
        switch (mainCamera.transform.position.x)
        {
            case < 283:
                mainCamera.transform.position = new Vector3(283, mainCamera.transform.position.y, mainCamera.transform.position.z);
                return;
            case > 500:
                mainCamera.transform.position = new Vector3(500, mainCamera.transform.position.y, mainCamera.transform.position.z);
                return;
        }

        // Get the horizontal input (left/right)
        var horizontalInput = Input.GetAxis("Horizontal");

        // Move the camera only along the X-axis (right/left)
        var movement = mainCamera.transform.right * horizontalInput;

        // Update the camera's position by adding the movement to the current position, but keep the Y and Z unchanged
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x + movement.x, mainCamera.transform.position.y, mainCamera.transform.position.z);
        
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
        if(appoinmentPopUpActive)
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
        mainCamera.transform.position = new Vector3(posX, mainCamera.transform.position.y, mainCamera.transform.position.z);
    }

    public void DoneWithSelectedLevel()
    {
        if(selectedLevel - 1 >= 0)
            if (unlockedLevels[selectedLevel-1])
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
