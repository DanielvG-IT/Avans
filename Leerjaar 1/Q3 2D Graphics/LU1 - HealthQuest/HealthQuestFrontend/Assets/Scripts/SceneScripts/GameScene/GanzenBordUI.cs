using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;

public class GanzenBordUI : MonoBehaviour
{
    [Header("References")]
    public GanzenboordManager boardManager;
    public GameObject popupPrefab;
    public Transform goose;
    public Camera mainCamera;
    public GameObject Ganzenboard;
    public Transform levelsParent;
    public GameObject UserHUD;

    [Header("Settings")]
    public float cameraSpeed = 5f;
    public float gooseSpeed = 125f;
    public Color completedColor = Color.green;
    public bool debugMode = false;

    //private readonly float cameraY = -1.496704f;
    //private readonly float cameraZ = -10f;

    private GameObject currentPopup;
    private List<GameObject> levelButtons = new();
    private List<Transform> levelRoots = new();
    private Vector3 cameraTarget;
    private int currentLevel = 0;
    private Vector3 gooseOriginalScale;

    private async void Start()
    {
        await boardManager.Initialize();

        InitializeLevels();
        InitializeGoose();

        if (DagboekScherm.clearingLevel != 0)
        {
            CompleteLevel(DagboekScherm.clearingLevel - 1); // Compensate for counting from 1
        }
    }

    private void InitializeLevels()
    {
        Debug.Log("Initializing...");
        Debug.Log($"Total levels: {boardManager.TotalLevels}");
        Debug.Log($"Completed levels: {boardManager.CompletedLevels}");

        //levelButtons.Clear();
        //levelRoots.Clear();

        foreach (Transform level in levelsParent)
        {
            string buttonName = "Button" + level.name;
            Transform buttonTransform = level.Find(buttonName);
            if (buttonTransform != null)
            {
                GameObject buttonObj = buttonTransform.gameObject;
                levelButtons.Add(buttonObj);
                levelRoots.Add(level);

                int index = levelButtons.Count - 1;
                if (buttonObj.TryGetComponent<Button>(out var btn))
                {
                    btn.onClick.AddListener(() => OnLevelClicked(index));
                }

                if (boardManager.IsLevelCompleted(index))
                {
                    SetLevelColor(index, completedColor);
                }
            }
        }
    }

    private void InitializeGoose()
    {
        if (levelButtons.Count == 0)
        {
            Debug.LogError("No levels found under levelsParent!");
            return;
        }

        if (boardManager.CompletedLevels > 0)
        {
            int lastIndex = boardManager.CompletedLevels - 1;
            goose.position = levelButtons[lastIndex].transform.position;
            currentLevel = lastIndex;

            cameraTarget = new Vector3(
                levelButtons[lastIndex].transform.position.x,
                mainCamera.transform.position.y,
                mainCamera.transform.position.z
            );
        }
        else
        {
            goose.position = levelButtons[0].transform.position;
            cameraTarget = mainCamera.transform.position;
        }

        gooseOriginalScale = goose.localScale;
        mainCamera.transform.position = cameraTarget;
    }

    private void Update()
    {
        float input = Input.GetAxis("Horizontal");
        if (input == 0f)
        {
            return;
        }

        if (Ganzenboard == null)
        {
            Debug.LogWarning("GanzenBoard is not assign in Unity editor!");
        }

        RectTransform rect = Ganzenboard.GetComponent<RectTransform>();

        float boardWidth = rect.rect.width * rect.lossyScale.x;
        float boardCenter = rect.position.x;
        float minX = boardCenter - boardWidth / 2f;
        float maxX = boardCenter + boardWidth / 2f;

        cameraTarget.x += input * (cameraSpeed * 50) * Time.deltaTime;
        cameraTarget.x = Mathf.Clamp(cameraTarget.x, minX, maxX);

        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraTarget, Time.deltaTime * cameraSpeed);
    }

    private void OnLevelClicked(int index)
    {
        if (currentPopup || !boardManager.IsLevelUnlocked(index))
        {
            Debug.Log(currentPopup ? "Popup is active" : "Level is locked.");
            return;
        }

        cameraTarget = new Vector3(levelButtons[index].transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);
        StartCoroutine(MoveGooseToLevel(index));
    }

    private IEnumerator MoveGooseToLevel(int targetIndex)
    {
        int direction = targetIndex > currentLevel ? 1 : -1;

        for (int i = currentLevel + direction; i != targetIndex + direction; i += direction)
        {
            Vector3 targetPos = levelButtons[i].transform.position;

            while (Vector3.Distance(goose.position, targetPos) > 0.2f)
            {
                // Update goose scale based on movement direction
                goose.localScale = new Vector3(
                    Mathf.Sign(targetPos.x - goose.position.x) * Mathf.Abs(gooseOriginalScale.x),
                    gooseOriginalScale.y,
                    gooseOriginalScale.z
                );

                // Move goose towards target position
                goose.position = Vector3.MoveTowards(goose.position, targetPos, Time.deltaTime * gooseSpeed);
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);
        }

        goose.localScale = gooseOriginalScale;
        currentLevel = targetIndex;
        ShowPopup(targetIndex);
    }

    private void ShowPopup(int index)
    {
        // Destroy previous popup if it exists
        if (currentPopup)
        {
            Destroy(currentPopup);
        }

        // Instantiate new popup
        currentPopup = Instantiate(popupPrefab, UserHUD.transform);
        currentPopup.transform.SetAsLastSibling();

        // Get appointment details
        Appointment appointment = boardManager.GetAppointment(index);
        if (appointment == null)
        {
            Debug.LogError($"No appointment found for index {index}");
            return;
        }

        // Set up the popup
        if (currentPopup.TryGetComponent<PopUpUI>(out var popup))
        {
            popup.Setup(appointment.name, appointment.description,
                () => RedirectToDagboek(index), () => Destroy(currentPopup));
        }
    }

    private void RedirectToDagboek(int index)
    {
        DagboekScherm.clearingLevel = index + 1; // Compensate for counting from 1
        Debug.Log($"Redirecting to Dagboek scherm with index {index + 1}");
        SceneManager.LoadScene("DagboekScherm");
    }

    private async void CompleteLevel(int index)
    {
        if (await boardManager.MarkLevelCompleted(index))
        {
            SetLevelColor(index, completedColor);
            Debug.Log($"Level {index + 1} completed.");

            StartCoroutine(MoveGooseToLevel(index + 1));
        }
        else
        {
            Debug.LogError("Failed to complete level.");
        }
    }

    private void SetLevelColor(int index, Color color)
    {
        // Try to get the SpriteRenderer directly from the level
        var spriteRenderer = levelRoots[index].GetComponentInChildren<SpriteRenderer>();

        // If found, set the color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }
}