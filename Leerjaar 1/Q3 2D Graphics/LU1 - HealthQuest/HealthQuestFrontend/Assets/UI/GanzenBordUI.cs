using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GanzenBordUI : MonoBehaviour
{
    [Header("Instellingen")]
    public TextAsset afsprakenJSON;
    public GameObject popUpPrefab;
    public Transform goose;
    public Camera mainCamera;
    public Transform levelsParent;
    public float cameraSpeed = 5f;
    public bool debugMode = false;

    [Header("Gans")]
    public float gooseSnelheid = 125f;

    [Header("Level status kleur")]
    public Color afgerondKleur = Color.green;

    private GameObject currentPopUp;
    private GameObject canvas;
    private List<GameObject> levelButtons = new List<GameObject>();
    private Afspraak[] afspraken;
    private bool[] unlockedLevels;
    private Vector3 cameraTarget;
    private int currentLevel = 0;
    private Vector3 gooseOriginalScale;

    void Start()
    {
        canvas = GameObject.Find("UserHUD");
        if (canvas == null)
        {
            Debug.LogError("❌ Canvas genaamd 'UserHUD' niet gevonden!");
            return;
        }

        // JSON inladen
        var data = JsonUtility.FromJson<AfspraakList>("{\"afspraken\":" + afsprakenJSON.text + "}");
        afspraken = data.afspraken;

        // Levels unlocken
        unlockedLevels = new bool[afspraken.Length];
        unlockedLevels[0] = true;

        if (debugMode)
        {
            for (int i = 0; i < unlockedLevels.Length; i++)
                unlockedLevels[i] = true;
        }

        // Knoppen ophalen en listeners toevoegen
        foreach (Transform level in levelsParent)
        {
            string buttonName = "Button" + level.name;
            Transform btn = level.Find(buttonName);
            if (btn != null)
            {
                GameObject buttonObj = btn.gameObject;
                levelButtons.Add(buttonObj);

                int index = levelButtons.Count - 1;
                Button b = buttonObj.GetComponent<Button>();
                if (b != null)
                    b.onClick.AddListener(() => OnLevelClicked(index));
            }
        }

        goose.position = levelButtons[0].transform.position;
        gooseOriginalScale = goose.localScale;
        cameraTarget = mainCamera.transform.position;
    }

    void Update()
    {
        // Smooth camera naar doelpositie (alleen bij klik)
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraTarget, Time.deltaTime * cameraSpeed);
    }

    void OnLevelClicked(int index)
    {
        if (!unlockedLevels[index])
        {
            Debug.Log("🔒 Level is gelocked.");
            return;
        }

        cameraTarget = new Vector3(levelButtons[index].transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);
        StartCoroutine(MoveGooseToLevel(index));
    }

    IEnumerator MoveGooseToLevel(int targetIndex)
    {
        for (int i = currentLevel + 1; i <= targetIndex; i++)
        {
            Vector3 targetPos = levelButtons[i].transform.position;

            while (Vector3.Distance(goose.position, targetPos) > 0.05f)
            {
                Vector3 richting = targetPos - goose.position;

                if (richting.x > 0.01f)
                    goose.localScale = new Vector3(Mathf.Abs(gooseOriginalScale.x), gooseOriginalScale.y, gooseOriginalScale.z);
                else if (richting.x < -0.01f)
                    goose.localScale = new Vector3(-Mathf.Abs(gooseOriginalScale.x), gooseOriginalScale.y, gooseOriginalScale.z);

                goose.position = Vector3.MoveTowards(goose.position, targetPos, Time.deltaTime * gooseSnelheid);
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);
        }

        goose.localScale = gooseOriginalScale;
        currentLevel = targetIndex;
        ShowPopup(targetIndex);
    }

    void ShowPopup(int index)
    {
        if (currentPopUp) Destroy(currentPopUp);

        currentPopUp = Instantiate(popUpPrefab, canvas.transform);
        currentPopUp.transform.SetAsLastSibling();

        PopUpUI ui = currentPopUp.GetComponent<PopUpUI>();
        if (ui != null)
        {
            ui.Setup(
                afspraken[index].title,
                afspraken[index].description,
                () => CompleteLevel(index),
                () => Destroy(currentPopUp)
            );
        }
    }

    void CompleteLevel(int index)
    {
        // Specifiek de child "KnopCirkel" zoeken en van kleur veranderen
        Transform cirkel = levelButtons[index].transform.Find("KnopCirkel");
        if (cirkel != null)
        {
            Image img = cirkel.GetComponent<Image>();
            if (img != null)
            {
                img.color = afgerondKleur;
            }
        }

        // Volgende level unlocken
        if (index + 1 < unlockedLevels.Length)
            unlockedLevels[index + 1] = true;

        Destroy(currentPopUp);
        Debug.Log($"✅ Level {index + 1} is nu unlocked!");
    }
}
