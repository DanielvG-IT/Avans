using TMPro;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem.Controls;
using UnityEditor.Search;
using System.Linq;

public class DagboekScherm : MonoBehaviour
{
    [Header("LeftCanvas")]
    public TMP_Text entryTitle;
    public TMP_Text entryDescription;
    public TMP_Text entryFillDate;
    public TMP_Text entryRating;
    public Button backButton;

    [Header("RightCanvas")]
    public TMP_InputField inputTitle;
    public TMP_InputField inputDescription;
    public TMP_InputField inputRating;
    public Canvas popUpBeforeSend;

    [Header("Avatar")]
    public Image avatar;
    public Sprite Kat;
    public Sprite Hond;
    public Sprite Paard;
    public Sprite Vogel;
    public TMP_Text patientName;

    [Header("PopUp")]
    public TMP_Text userErrorMessageNL;
    public TMP_Text userErrorMessageEN;
    public Toggle doktorenToegang;
    public Text doktorenToegangText;
    public Toggle oudersToegang;
    public Text oudersToegangText;

    private PatientApiClient patientApiClient;
    private JournalApiClient journalApiClient;
    private Patient currentPatient;
    private List<JournalEntry> journalEntries;
    private Scenemanager sceneManager;
    private Animator animator;
    private int journalPage = 0;
    private string currentScene;
    private bool sentJournalEntry = false;
    private string currentAvatar;
    private string originGameScene;
    private bool updatingJournalEntry = false;
    private string updatingID;
    private int updatingPage;

    //TODO: Variabele voor de level clear tag (bij terug button click altijd zetten op false, bij game 'klaar knop' zet op true. Dan bij opsturen LevelCleared())
    public static int clearingLevel = 0;

    public async void Start()
    {
        originGameScene = ApiClientManager.Instance.CurrentTreatment.name == "Zonder Ziekenhuis Opname" ? "GameTrajectZonder" : "GameTrajectMet";
        popUpBeforeSend.gameObject.SetActive(false);
        sceneManager = gameObject.AddComponent<Scenemanager>();
        journalApiClient = ApiClientManager.Instance.JournalApiClient;
        patientApiClient = ApiClientManager.Instance.PatientApiClient;
        currentPatient = ApiClientManager.Instance.CurrentPatient;
        animator = backButton.GetComponent<Animator>();
        currentAvatar = currentPatient.avatar;

        await LoadJournalEntries();
        SetAvatar();
        ShowJournalEntry(journalPage);
        LoadToggles();
    }

    private async Task LoadJournalEntries()
    {
        var journalResponse = await journalApiClient.ReadJournalEntriesAsync(currentPatient.id);
        if (journalResponse is WebRequestError journalError)
        {
            Debug.LogWarning($"Failed to load journal entries: {journalError.ErrorMessage}");
            journalEntries = new();
            return;
        }
        else if (journalResponse is WebRequestData<List<JournalEntry>> journalEntry)
        {
            journalEntries = journalEntry.Data;
        }
    }

    public void ShowJournalEntry(int entryNumber)
    {
        if (journalEntries == null || !journalEntries.Any())
        {
            Debug.Log("No journal entries to load");
            return;
        }
        entryTitle.text = journalEntries[entryNumber].title;
        entryDescription.text = journalEntries[entryNumber].content;
        entryFillDate.text = $"Aangemaakt op: {journalEntries[entryNumber].date.Substring(0, 9)}";
        entryRating.text = $"Beoordeling: {journalEntries[entryNumber].rating}/10";
    }

    public async Task SendNewJournalEntry()
    {
        var newJournalEntry = new JournalEntry
        {
            title = inputTitle.text,
            content = inputDescription.text,
            rating = Convert.ToInt32(inputRating.text),
            date = DateTime.Now.ToString(),
            patientID = ApiClientManager.Instance.CurrentPatient.id
        };

        var journalCreateRespone = await journalApiClient.CreateJournalEntryAsync(newJournalEntry);
        if (journalCreateRespone is WebRequestError journalCreateError)
        {
            Debug.LogError($"Failed to create journal entry: {journalCreateError.ErrorMessage}");

            switch (journalCreateError.StatusCode)
            {
                case 400:
                    userErrorMessageNL.text = "Je hebt iets fout ingevuld. Vergeet niet om alle velden in te vullen.";
                    break;
                case 500:
                    userErrorMessageNL.text = "Er ging iets mis bij ons...";
                    break;
                default:
                    userErrorMessageNL.text = "Er is iets fout gegaan.";
                    break;
            }
            userErrorMessageEN.text = journalCreateError.ErrorMessage;
            return;
        }
        else if (journalCreateRespone is WebRequestData<JournalEntry> journalCreateSucces)
        {
            journalEntries.Add(journalCreateSucces.Data);
            journalPage = journalEntries.Count - 1;
            ShowJournalEntry(journalPage);
            userErrorMessageNL.text = string.Empty;
            userErrorMessageEN.text = string.Empty;
        }
    }

    public async void DeleteEntry()
    {
        if (!journalEntries.Any())
        {
            Debug.LogWarning("No Journal entries to remove!");
            return;
        }
        else if (journalEntries.Count == 1)
        {
            await journalApiClient.DeleteJournalEntryAsync(journalEntries[journalPage].id);
            journalEntries.RemoveAt(0);
            entryTitle.text = string.Empty;
            entryDescription.text = string.Empty;
            entryFillDate.text = string.Empty;
            entryRating.text = string.Empty;
            journalPage = 0;
        }
        else if (journalPage == 0 && journalEntries.Count > 1)
        {
            await journalApiClient.DeleteJournalEntryAsync(journalEntries[journalPage].id);
            journalEntries.RemoveAt(journalPage);
            ShowJournalEntry(journalPage);
        }
        else
        {
            var removeResponse = await journalApiClient.DeleteJournalEntryAsync(journalEntries[journalPage].id);
            if (removeResponse is WebRequestError removeError)
            {
                Debug.Log($"Failed to remove Journal Entry: {removeError.ErrorMessage}");
            }
            else if (removeResponse is WebRequestData<string> removeSucces)
            {
                Debug.Log($"Removed succesfully: { removeSucces.Data} - {removeSucces.StatusCode}");
                journalEntries.RemoveAt(journalPage);
                journalPage--;
                ShowJournalEntry(journalPage);
            }
        }
    }

    public void CycleJournalPage(bool goingForward)
    {
        if (journalEntries == null || !journalEntries.Any())
        {
            Debug.Log("No journal entries to load");
            return;
        }
        if (goingForward)
        {
            if (journalPage < journalEntries.Count - 1)
            {
                journalPage++;
                ShowJournalEntry(journalPage);
            }
        }
        else
        {
            if (journalPage > 0)
            {
                journalPage--;
                ShowJournalEntry(journalPage);
            }
        }
    }

    public void OnBackButtonClick()
    {
        if (!sentJournalEntry)
        {
            clearingLevel = 0;
        }


        animator.Play("RedButton");
        StartCoroutine(SwitchSceneAfterDelay(originGameScene));
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

    public async void OnSentButtonClick()
    {
        if(updatingJournalEntry)
        {
            await UpdateJournal();
            ClearInputFields();
            popUpBeforeSend.gameObject.SetActive(false);
        }
        else
        {
            await SendNewJournalEntry();
            ClearInputFields();
            popUpBeforeSend.gameObject.SetActive(false);

            if (clearingLevel != 0)
            {
                SceneManager.LoadScene(originGameScene);
            }
        }
    }
    public async Task UpdateJournal()
    {
        var updatedJournalEntry = new JournalEntry
        {
            id = updatingID,
            title = inputTitle.text,
            content = inputDescription.text,
            rating = Convert.ToInt32(inputRating.text),
            date = DateTime.Now.ToString(),
            patientID = ApiClientManager.Instance.CurrentPatient.id
        };

        var journalUpdateReturn = await journalApiClient.UpdateJournalEntryAsync(updatingID, updatedJournalEntry);
        if (journalUpdateReturn is WebRequestError journalUpdateError)
        {
            Debug.LogWarning("Updating journal failed: " + journalUpdateError.ErrorMessage);
            userErrorMessageNL.text = journalUpdateError.StatusCode switch
            {
                404 => "Geen dagboek verhaal gevonden om aan te passen.",
                400 => "Je hebt iets fout ingevuld. Vergeet niet om alle velden in te vullen.",
                403 => "Je mag niet een ander iemand zijn dagboek aanpassen!",
                500 => "Er ging iets mis bij ons...",
                _ => "Er is iets fout gegaan.",
            };
            userErrorMessageEN.text = journalUpdateError.ErrorMessage;
            return;
        }
        else if (journalUpdateReturn is WebRequestData<JournalEntry> journalUpdateSuccess)
        {
            updatingJournalEntry = false;
            journalEntries.RemoveAt(updatingPage);
            journalEntries.Add(journalUpdateSuccess.Data);
            journalPage = journalEntries.Count - 1;
            ShowJournalEntry(journalPage);
            userErrorMessageNL.text = string.Empty;
            userErrorMessageEN.text = string.Empty;
        }
    }

    public void StartUpdatingJournal()
    {
        if (journalEntries[journalPage] == null)
        {
            Debug.LogWarning("Journal entries empty, nothing to update");
            return;
        }
        else
        {
            updatingJournalEntry = true;
            inputTitle.text = journalEntries[journalPage].title;
            inputDescription.text = journalEntries[journalPage].content;
            inputRating.text = journalEntries[journalPage].rating.ToString();
            updatingID = journalEntries[journalPage].id;
            updatingPage = journalPage;
        }
    }
    public void OpenPopUp()
    {
        popUpBeforeSend.gameObject.SetActive(true);
    }
    
    public void ClosePopUp()
    {
        popUpBeforeSend.gameObject.SetActive(false);
    }
    
    public void ClearInputFields()
    {
        inputTitle.text = string.Empty;
        inputRating.text = string.Empty;
        inputDescription.text = string.Empty;
        updatingJournalEntry = false;
    }

    public void LoadToggles()
    {
        if (currentPatient.doctorAccessJournal)
        {
            doktorenToegang.isOn = true;
            doktorenToegangText.text = "Wel toegang";
            doktorenToegangText.color = new Color(122f / 255f, 1f, 0f);
        }
        else
        {
            doktorenToegang.isOn = false;
            doktorenToegangText.text = "Geen toegang";
            doktorenToegangText.color = new Color(1f, 0f, 13f / 255f);
        }

        if (currentPatient.guardianAccessJournal)
        {
            oudersToegang.isOn = true;
            oudersToegangText.text = "Wel toegang";
            oudersToegangText.color = new Color(122f / 255f, 1f, 0f);
        }
        else
        {
            oudersToegang.isOn = false;
            oudersToegangText.text = "Geen toegang";
            oudersToegangText.color = new Color(1f, 0f, 13f / 255f);
        }
    }

    public async void OnToggleChanged(bool oudersChanged)
    {
        if(oudersChanged)
        {
            currentPatient.guardianAccessJournal = oudersToegang.isOn;
            var resultGuardian = await patientApiClient.UpdatePatient(currentPatient.id, currentPatient);
            if(resultGuardian is WebRequestError error)
            {
                Debug.LogWarning("Error toggling guardian journal access: " + error.ErrorMessage);
                return;
            }
            else if (resultGuardian is WebRequestData<Patient> patient)
            {
                Debug.Log("Updated current patient:" + patient.Data);
            }

            if (currentPatient.guardianAccessJournal)
            {
                oudersToegangText.text = "Wel toegang";
                oudersToegangText.color = new Color(122f / 255f, 1f, 0f);
            }
            else
            {
                oudersToegangText.text = "Geen toegang";
                oudersToegangText.color = new Color(1f, 0f, 13f / 255f);
            }
        }
        else
        {
            currentPatient.doctorAccessJournal = doktorenToegang.isOn;
            var resultDoctor = await patientApiClient.UpdatePatient(currentPatient.id, currentPatient);
            if (resultDoctor is WebRequestError error)
            {
                Debug.LogWarning("Error toggling doctor journal access: " + error.ErrorMessage);
                return;
            }
            else if (resultDoctor is WebRequestData<Patient> patient)
            {
                Debug.Log("Updated current patient:" + patient);
            }

            if (currentPatient.doctorAccessJournal)
            {
                doktorenToegangText.text = "Wel toegang";
                doktorenToegangText.color = new Color(122f / 255f, 1f, 0f);
            }
            else
            {
                doktorenToegangText.text = "Geen toegang";
                doktorenToegangText.color = new Color(1f, 0f, 13f / 255f);
            }
        }
    }
}
