using TMPro;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DagboekScherm : MonoBehaviour
{
    [Header("UI elements")]
    public Transform journalListContent;
    public GameObject journalListItemPrefab;
    public Button backButton;

    [Header("Avatar")]
    public Image avatar;
    public Sprite Kat;
    public Sprite Hond;
    public Sprite Paard;
    public Sprite Vogel;
    public TMP_Text patientName;

    private PatientApiClient patientApiClient;
    private JournalApiClient journalApiClient;
    private Patient currentPatient;
    private List<JournalEntry> journalEntries;
    private Scenemanager sceneManager;
    private Animator animator;
    private string currentScene;

    //TODO: Variabele voor de level clear tag (bij terug button click altijd zetten op false, bij game 'klaar knop' zet op true. Dan bij opsturen LevelCleared())
    public static bool clearingLevel = false;

    public async void Start()
    {
        sceneManager = new Scenemanager();
        journalApiClient = ApiClientManager.Instance.JournalApiClient;
        patientApiClient = ApiClientManager.Instance.PatientApiClient;
        currentPatient = ApiClientManager.Instance.CurrentPatient;
        animator = backButton.GetComponent<Animator>();


        await LoadJournalEntries();
    }

    private async Task LoadJournalEntries()
    {
        var journalResponse = await journalApiClient.ReadJournalEntriesAsync(currentPatient.id);
        if (journalResponse is WebRequestError journalError)
        {
            Debug.LogError($"Failed to load journal entries: {journalError.ErrorMessage}");
            return;
        }
        else if (journalResponse is WebRequestData<List<JournalEntry>> journalEntry)
        {

            journalEntries = journalEntry.Data;
        }
    }

    public async Task CreateJournalEntry()
    { 
        var journalEntry = new JournalEntry
        {
            patientID = currentPatient.id,
            guardianID = currentPatient.guardianID,
            date = DateTime.Now.ToString(),
            title = "titlefield.text", // TODO: replace with actual title field
            content = "contentfield.text", // TODO: replace with actual content field
            rating = int.Parse("ratingfield.text") // TODO: replace with actual rating field
        };

        var journalCreateResponse = await journalApiClient.CreateJournalEntryAsync(journalEntry);
        if (journalCreateResponse is WebRequestError journalCreateError)
        {
            Debug.LogError($"Failed to create journal entry: {journalCreateError.ErrorMessage}");
            return;
        }

        journalEntries.Add(journalCreateResponse as JournalEntry);
    }

    public void OnBackButtonClick()
    {
        clearingLevel = false;
        var gameOriginScene = ApiClientManager.Instance.CurrentTreatment.name;
        gameOriginScene = gameOriginScene == "Zonder Ziekenhuis Opname" ? "GameTrajectZonder" : "GameTrajectMet";



        animator.Play("RedButton");
        StartCoroutine(SwitchSceneAfterDelay(gameOriginScene));
    }

    private IEnumerator SwitchSceneAfterDelay(string scene)
    {
        yield return new WaitForSeconds(0.3f);
        sceneManager.SwitchScene(scene);
    }
}
