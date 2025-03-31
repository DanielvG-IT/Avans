using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class StickerBoeken : MonoBehaviour
{
    [Header("Stickers")]
    public GameObject[] stickers;

    public GameObject poofPrefab;


    public static List<string> newUnlockedStickers = new() { "Ziekenhuis", "Auto" }; //Gebruikt om poof te spawnen
    public static List<string> oldUnlockedStickers = new(); //Wordt gebruikt om eventuele animatie af te spelen bij stickers die nieuw unlocked zijn.

    private Transform animationCanvas;
    private readonly PatientApiClient patientApiClient = ApiClientManager.Instance.PatientApiClient;
    private readonly StickerApiClient stickerApiClient = ApiClientManager.Instance.StickerApiClient;

    // Sample code to show how to use the ApiClient classes
    private async Task GetPatientUnlockedStickers()
    {
        // Get all unlocked stickers for the current patient
        var patient = ApiClientManager.Instance.CurrentPatient;
        var unlockedStickersResponse = await patientApiClient.ReadUnlockedStickersFromPatientAsync(patient.id.ToString());

        switch (unlockedStickersResponse)
        {
            case WebRequestData<List<Sticker>> dataResponse:
                {
                    foreach (var sticker in dataResponse.Data)
                    {
                        Debug.Log(dataResponse.Data);
                        oldUnlockedStickers.Add(sticker.name);
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

    public async void Start()
    {
        await GetPatientUnlockedStickers();

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
        GameObject poof = Instantiate(poofPrefab, sticker.transform.position, Quaternion.identity);

        poof.transform.SetParent(animationCanvas);

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
