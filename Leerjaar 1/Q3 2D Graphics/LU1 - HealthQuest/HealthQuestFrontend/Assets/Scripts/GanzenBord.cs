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
    private float[] cameraPositions = new float[] { 283, 500, 600 };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ToggleSelection();
    }

    // Update is called once per frame
    void Update()
    {
        
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
