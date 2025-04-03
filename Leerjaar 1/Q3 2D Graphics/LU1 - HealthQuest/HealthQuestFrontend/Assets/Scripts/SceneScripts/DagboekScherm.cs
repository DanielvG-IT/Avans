using TMPro;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class DagboekScherm : MonoBehaviour
{
    [Header("UI elements")]
    public Transform journalListContent;
    public GameObject journalListItemPrefab;

    private PatientApiClient patientApiClient;
    private JournalApiClient journalApiClient;
    private Patient currentPatient;
    private List<JournalEntry> journalEntries;

    //Variabele voor de level clear tag (bij terug button click altijd zetten op false, bij game 'klaar knop' zet op true. Dan bij opsturen LevelCleared())
    public static bool clearingLevel = false;

    public async void Start()
    {
        journalApiClient = ApiClientManager.Instance.JournalApiClient;
        patientApiClient = ApiClientManager.Instance.PatientApiClient;
        currentPatient = ApiClientManager.Instance.CurrentPatient;

        await LoadJournalEntries();
        ShowJournalsOnUI();
    }

    private async Task LoadJournalEntries()
    { 
        var journalResponse = await journalApiClient.ReadJournalEntriesAsync(currentPatient.id);
        if (journalResponse is WebRequestError journalError)
        {
            Debug.LogError($"Failed to load journal entries: {journalError.ErrorMessage}");
            return;
        }

        journalEntries = journalResponse as List<JournalEntry>;
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

    private void ShowJournalsOnUI()
    {
        if (journalEntries == null)
        {
            return;
        }

        foreach (var journalEntry in journalEntries)
        {
            var journalListItem = Instantiate(journalListItemPrefab, journalListContent);
            var titleText = journalListItem.transform.Find("Text (TMP)").GetComponent<TMP_Text>();
            titleText.text = journalEntry.title;

            // TODO: Add click event to open journal entry
        }
    }
}
