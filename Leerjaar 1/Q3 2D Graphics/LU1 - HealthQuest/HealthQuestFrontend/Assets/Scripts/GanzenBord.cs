using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Ganzenbord : MonoBehaviour
{
    [Header("PopUp")]
    public Image backdrop;
    public Button exitButton;
    public TMP_Text appointmentTitle;
    public TMP_Text appointmentDescription;

    public Camera mainCamera;

    private bool appoinmentPopUpActive = true;
    private string[] appointmentTitles = new string[] { "Title 1", "Title 2", "Title 3" };
    private string[] appointmentDescriptions = new string[] { "Description 1", "Description 2", "Description 3" };
    private float[] cameraPositions = new float[] { 283, 500, 700 };

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
            backdrop.gameObject.SetActive(false);
            exitButton.gameObject.SetActive(false);
            appointmentTitle.gameObject.SetActive(false);
            appointmentDescription.gameObject.SetActive(false);
            appoinmentPopUpActive = false;
        }
        else
        {
            backdrop.gameObject.SetActive(true);
            exitButton.gameObject.SetActive(true);
            appointmentTitle.gameObject.SetActive(true);
            appointmentDescription.gameObject.SetActive(true);
            appoinmentPopUpActive = true;
        }
    }

    public void MoveMainCamera(float posX)
    {
        mainCamera.transform.position = new Vector3(posX, mainCamera.transform.position.y, mainCamera.transform.position.z);
    }
}
