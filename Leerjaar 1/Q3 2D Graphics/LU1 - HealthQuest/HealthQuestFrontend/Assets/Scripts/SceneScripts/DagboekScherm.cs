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
    public TMP_Text entryTitle;
    public TMP_Text entryDescription;
    public TMP_Text entryFillDate;
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
    private bool sentJournalEntry = false;
    private string currentAvatar;

    //TODO: Variabele voor de level clear tag (bij terug button click altijd zetten op false, bij game 'klaar knop' zet op true. Dan bij opsturen LevelCleared())
    public static int clearingLevel = 0;

    public async void Start()
    {
        sceneManager = new Scenemanager();
        journalApiClient = ApiClientManager.Instance.JournalApiClient;
        patientApiClient = ApiClientManager.Instance.PatientApiClient;
        currentPatient = ApiClientManager.Instance.CurrentPatient;
        animator = backButton.GetComponent<Animator>();
        currentAvatar = currentPatient.avatar;


        await LoadJournalEntries();
        SetAvatar();
        ShowJournalEntry(0);
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

    public void ShowJournalEntry(int entryNumber)
    {
        entryTitle.text = journalEntries[entryNumber].title;
        entryDescription.text = journalEntries[entryNumber].content;
        entryFillDate.text = journalEntries[entryNumber].date;
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
        else
        {
            sentJournalEntry = true;
            journalEntries.Add(journalCreateResponse as JournalEntry);
        }

    }

    public void OnBackButtonClick()
    {
        if (!sentJournalEntry)
        {
            clearingLevel = 0;
        }
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

    public void SetAvatar()
    {
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

        patientName.text = $"{currentPatient.firstName} {currentPatient.lastName}";
    }
}
