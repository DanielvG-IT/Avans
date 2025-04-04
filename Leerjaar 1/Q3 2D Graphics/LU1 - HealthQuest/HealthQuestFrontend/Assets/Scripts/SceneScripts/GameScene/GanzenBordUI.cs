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

    [Header("Settings")]
    public float cameraSpeed = 5f;
    public float gooseSpeed = 125f;
    public Color completedColor = Color.green;
    public bool debugMode = false;

    private readonly float cameraY = -1.496704f;
    private readonly float cameraZ = -10f;

    private GameObject currentPopup;
    private GameObject canvas;
    private List<GameObject> levelButtons = new();
    private List<Transform> levelRoots = new();
    private Vector3 cameraTarget;
    private int currentLevel = 0;
    private Vector3 gooseOriginalScale;

    private async void Awake()
    {
        canvas = GameObject.Find("UserHUD");

        int savedLevel = DagboekScherm.clearingLevel;
        if (savedLevel != 0)
        {
            CompleteLevel(savedLevel);
        }

        await boardManager.Initialize();
        await InitializeGame();
    }

    private async Task InitializeGame()
    {
        Debug.Log($"Total levels: {boardManager.TotalLevels}");
        Debug.Log($"Completed levels: {boardManager.CompletedLevels}");

        levelButtons.Clear();
        levelRoots.Clear();

        // Set up level buttons and their interactions
        foreach (Transform level in levelsParent)
        {
            var buttonTransform = level.Find("Button" + level.name);
            if (buttonTransform != null)
            {
                GameObject buttonObj = buttonTransform.gameObject;
                levelButtons.Add(buttonObj);
                levelRoots.Add(level);

                int index = levelButtons.Count - 1;
                buttonObj.TryGetComponent<Button>(out var btn);
                btn?.onClick.AddListener(() => OnLevelClicked(index));

                if (boardManager.IsLevelCompleted(index))
                {
                    SetLevelColor(index, completedColor);
                }
            }
        }

        // Ensure level buttons are set up correctly
        if (levelButtons.Count == 0)
        {
            Debug.LogError("No level buttons found!");
            return;
        }

        // Wait until Unity finishes layout to get correct positions
        while (levelButtons[0].transform.position == Vector3.zero)
        {
            await Task.Yield();
        }

        // Set camera and goose position based on completed levels
        int targetIndex = boardManager.CompletedLevels > 0 && boardManager.CompletedLevels < levelButtons.Count
            ? boardManager.CompletedLevels
            : 0;

        cameraTarget = new Vector3(levelButtons[targetIndex].transform.position.x, cameraY, cameraZ);
        goose.position = levelButtons[targetIndex].transform.position;
        gooseOriginalScale = goose.localScale;
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
            return;
        }

        RectTransform rect = Ganzenboard.GetComponent<RectTransform>();
        float boardWidth = rect.rect.width * rect.lossyScale.x;
        float boardCenter = Ganzenboard.transform.position.x;
        float minX = boardCenter - boardWidth / 2f;
        float maxX = boardCenter + boardWidth / 2f;

        // Update camera target position and clamp it within bounds
        cameraTarget.x += input * cameraSpeed * 50 * Time.deltaTime;
        cameraTarget.x = Mathf.Clamp(cameraTarget.x, minX, maxX);

        // Smoothly move the camera to the target position
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

            while (Vector3.Distance(goose.position, targetPos) > 0.05f)
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
        if (currentPopup) Destroy(currentPopup);

        // Instantiate new popup
        currentPopup = Instantiate(popupPrefab, canvas.transform);
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
                () => CompleteLevel(index), () => Destroy(currentPopup));
        }
    }

    private void RedirectToDagboek(int index)
    {
        DagboekScherm.clearingLevel = index;
        SceneManager.LoadScene("DagboekScherm");
    }

    private async void CompleteLevel(int index)
    {
        if (await boardManager.MarkLevelCompleted(index))
        {
            SetLevelColor(index, completedColor);
            Debug.Log($"Level {index + 1} completed.");

            // Set camera and goose position based on completed levels
            int targetIndex = boardManager.CompletedLevels > 0 && boardManager.CompletedLevels < levelButtons.Count
                ? boardManager.CompletedLevels
                : 0;

            cameraTarget = new Vector3(levelButtons[targetIndex].transform.position.x, cameraY, cameraZ);
            goose.position = levelButtons[targetIndex].transform.position;
            gooseOriginalScale = goose.localScale;
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