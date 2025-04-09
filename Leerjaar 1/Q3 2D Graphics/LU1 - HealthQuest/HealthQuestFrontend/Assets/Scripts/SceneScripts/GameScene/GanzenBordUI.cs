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
    private readonly List<GameObject> levelButtons = new();
    private readonly List<Transform> levelRoots = new();
    private Vector3 cameraTarget;
    private int currentLevel = 0;
    private Vector3 gooseOriginalScale;
    private List<PersonalAppointments> personalAppointments = new();

    private async void Start()
    {
        await boardManager.Initialize();

        InitializeLevels();
        InitializeGoose();

        if (DagboekScherm.clearingLevel != 0)
        {
            CompleteLevel(DagboekScherm.clearingLevel - 1); // Compensate for counting from 1
            DagboekScherm.clearingLevel = 0;
        }
    }

    private void InitializeLevels()
    {
        Debug.Log("Initializing...");

        personalAppointments = boardManager.personalAppointments;
        Debug.Log($"Total appointments: {boardManager.appointments.Count}");

        levelButtons.Clear();
        levelRoots.Clear();

        for (int i = 0; i < levelsParent.childCount && i < boardManager.appointments.Count; i++)
        {
            Transform level = levelsParent.GetChild(i);
            string buttonName = "Button" + level.name;
            Transform buttonTransform = level.Find(buttonName);
            if (buttonTransform == null)
            {
                Debug.LogWarning($"No button found in level {level.name}");
                continue;
            }

            GameObject buttonObj = buttonTransform.gameObject;
            levelButtons.Add(buttonObj);
            levelRoots.Add(level);

            int index = i;

            if (buttonObj.TryGetComponent<Button>(out var btn))
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnLevelClicked(index));
            }

            // Set level color based on completion status
            if (index < personalAppointments.Count && !string.IsNullOrEmpty(personalAppointments[index].completedDate))
            {
                SetLevelColor(index, completedColor); // Level is completed
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

        int lastCompletedIndex = -1;

        // Loop through personal appointments to find last completed one
        for (int i = 0; i < boardManager.personalAppointments.Count; i++)
        {
            var personalAppointment = boardManager.personalAppointments[i];
            if (!string.IsNullOrEmpty(personalAppointment.completedDate))
            {
                lastCompletedIndex = i; // Update to the last completed index
            }
        }

        // If clearingLevel is set (returning from Dagboek), override the position
        if (DagboekScherm.clearingLevel > 0)
        {
            lastCompletedIndex = DagboekScherm.clearingLevel - 1;
        }

        // Set goose position based on last completed level, or start at beginning
        int targetIndex = Mathf.Clamp(lastCompletedIndex + 1, 0, levelButtons.Count - 1);
        goose.position = levelButtons[targetIndex].transform.position;
        currentLevel = targetIndex;

        cameraTarget = new Vector3(
            levelButtons[targetIndex].transform.position.x,
            mainCamera.transform.position.y,
            mainCamera.transform.position.z
        );

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
        if (currentPopup)
            Destroy(currentPopup);

        currentPopup = Instantiate(popupPrefab, UserHUD.transform);
        currentPopup.transform.SetAsLastSibling();

        Appointment appointment = boardManager.GetAppointment(index);
        PersonalAppointments personal = index < personalAppointments.Count ? personalAppointments[index] : null;

        if (appointment == null)
        {
            Debug.LogError($"No appointment found for index {index}");
            return;
        }

        string description = appointment.description;

        if (personal != null)
        {
            description += $"\n\n• Datum: {(string.IsNullOrEmpty(personal.appointmentDate) ? "Vraag aan je arts." : personal.appointmentDate)}";
            description += $"\n• Voltooid: {(string.IsNullOrEmpty(personal.completedDate) ? "Nee" : "Ja")}";
        }

        if (currentPopup.TryGetComponent<PopUpUI>(out var popup))
        {
            popup.Setup(
                appointment.name,
                description,
                () => RedirectToDagboek(index),
                () => Destroy(currentPopup)
            );
        }
    }

    private void RedirectToDagboek(int index)
    {
        if (currentPopup)
        {
            Destroy(currentPopup);
        }

        if (boardManager.IsLevelCompleted(index))
        {
            Debug.Log("Level is already completed.");
            return;
        }

        DagboekScherm.clearingLevel = index + 1; // Compensate for counting from 1
        Debug.Log($"Redirecting to Dagboek scherm with index {index + 1}");
        Destroy(this);
        SceneManager.LoadScene("DagboekScherm");
    }

    private async void CompleteLevel(int index)
    {
        if (await boardManager.MarkLevelCompleted(index))
        {
            SetLevelColor(index, completedColor);

            await UnlockSticker(index);
            await boardManager.LoadAllAppointments();

            StartCoroutine(MoveGooseToLevel(index + 1));
            Debug.Log($"Level {index + 1} completed.");
        }
        else
        {
            Debug.LogError("Failed to complete level.");
        }
    }

    private async Task UnlockSticker(int index)
    {
        string[] stickers = ApiClientManager.Instance.CurrentTreatment.name switch
        {
            "Zonder Ziekenhuis Opname" => new[] { "Dokter", "Ziekenhuis", "Pleister", "Microscope", "Hart", "Medicijn", "Syringe", "Brood", "Auto", "Troffee" },
            "Met Ziekenhuis Opname" => new[] { "Ambulance", "Stethoscope", "Ziekenhuis", "Syringe", "Bed", "Microscope", "Hart", "Informatie", "Bloedcellen", "Brood", "Smiley", "Medicijnen", "Auto", "Troffee" },
            _ => null
        };

        if (stickers == null)
        {
            Debug.LogWarning("No stickers available for this treatment.");
            return;
        }

        if (index < 0 || index >= stickers.Length)
        {
            Debug.LogError($"Index {index} is out of range for stickers.");
            return;
        }

        string stickerName = stickers[index];
        if (await boardManager.MarkStickerCompleted(stickerName))
        {
            Debug.Log($"Unlocking sticker: {stickerName}");
            StickerBoeken.newUnlockedStickers.Add(stickerName);
            Debug.Log($"Sticker {stickerName} unlocked.");
        }
        else
        {
            Debug.LogError($"Failed to unlock sticker: {stickerName}");
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