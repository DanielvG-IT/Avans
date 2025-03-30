using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StickerBoeken : MonoBehaviour
{
    [Header("Stickers")]
    public GameObject[] stickers;

    public GameObject poofPrefab;
    

    public static string[] newUnlockedStickers = {"Ziekenhuis", "Auto"}; //Wordt gebruikt om eventuele animatie af te spelen bij stickers die nieuw unlocked zijn.
    public static string[] oldUnlockedStickers = { "Ambulance", "Hart"}; //Wordt gebruikt om eventuele animatie af te spelen bij stickers die nieuw unlocked zijn.

    private Transform animationCanvas;
    private PatientApiClient patientApiClient = ApiClientManager.Instance.PatientApiClient;
    private StickerApiClient stickerApiClient = ApiClientManager.Instance.StickerApiClient;

    // Sample code to show how to use the ApiClient classes
    private async void SamleMethode1()
    {
        // Get all unlocked stickers for the current patient
        var patient = ApiClientManager.Instance.CurrentPatient;
        var unlockedStickersResponse = await patientApiClient.ReadUnlockedStickersFromPatientAsync(patient.ID.ToString());

        switch (unlockedStickersResponse)
        {
            case WebRequestData<List<Sticker>> dataResponse:
                {
                    foreach (var sticker in dataResponse.Data)
                    {
                        Debug.Log("Unlocked sticker: " + sticker.name);
                    }
                    break;
                }

            case WebRequestError errorResponse:
                {
                    Debug.Log("Error: " + errorResponse.ErrorMessage);
                    break;
                }
        }
    }

    private async void SamleMethode2(string? stickerName /*, Sticker? stickerObject */) // Can also be sticker object
    {
        // Add new unlocked sticker to current patient
        var patient = ApiClientManager.Instance.CurrentPatient;

        // If you have the sticker name:
        var searchStickerResponse = await stickerApiClient.ReadStickerByNameAsync(stickerName);
        var sticker = (searchStickerResponse as WebRequestData<Sticker>).Data;
        var newUnlockedResponse = await patientApiClient.AddUnlockedStickerToPatientAsync(patient.ID.ToString(), sticker);

        // If you have the sticker object:
        //var newUnlockedResponse = await patientApiClient.AddUnlockedStickerToPatientAsync(patient.ID.ToString(), stickerObject);

        switch (newUnlockedResponse)
        {
            case WebRequestData<List<Sticker>> dataResponse:
                {
                    Debug.Log("Sticker unlocked: " + stickerName);
                    // Optionally, play animation for new sticker and make it visible
                    // InstantiatePoof(stickerObject);
                    break;
                }

            case WebRequestError errorResponse:
                {
                    Debug.Log("Error: " + errorResponse.ErrorMessage);
                    break;
                }
        }
    }
    // End of sample code

    public void Start()
    {
        animationCanvas = GameObject.Find("Animation_Canvas").transform;

        foreach (var old in oldUnlockedStickers)
        {
            foreach (var sticker in stickers)
            {
                if (sticker.name == old)
                {
                    Image stickerImage = sticker.GetComponent<Image>();
                    stickerImage.color = Color.white;
                }
            }
        }

        foreach (var newSticker in newUnlockedStickers)
        {
            foreach (var sticker in stickers)
            {
                if (sticker.name == newSticker)
                {
                    InstantiatePoof(sticker);
                    Image stickerImage = sticker.GetComponent<Image>();
                    stickerImage.color = Color.white;
                }
            }
        }

    }
    public void InstantiatePoof(GameObject sticker)
    {
        // Instantiate the poofPrefab at the sticker's position
        GameObject poof = Instantiate(poofPrefab, sticker.transform.position, Quaternion.identity);

        // Set the parent of the poof to be the "Animation Canvas"
        poof.transform.SetParent(animationCanvas);

        // Optionally, adjust the poof's position relative to the canvas if needed
        poof.transform.position = sticker.transform.position;

        DestroyPoofAfterDelay(poof, 2f);
    }

    // Method to destroy the poof object after a specified delay
    private void DestroyPoofAfterDelay(GameObject poof, float delay)
    {
        // Start the coroutine to destroy the poof
        StartCoroutine(DestroyPoofCoroutine(poof, delay));
    }

    // Coroutine to handle the delayed destruction of the poof
    private IEnumerator DestroyPoofCoroutine(GameObject poof, float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Destroy the poof object
        Destroy(poof);
    }

}
