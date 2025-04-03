using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using System.Linq;

public class StickerBoeken : MonoBehaviour
{
    [Header("Stickers")]
    public GameObject[] stickers;

    public GameObject poofPrefab;
    public TMP_Text patientName;
    public TMP_Text patientUnlockedStickerAmount;

    [Header("Avatar")]
    public Image avatar;
    public Sprite Kat;
    public Sprite Hond;
    public Sprite Paard;
    public Sprite Vogel;

    public static List<string> newUnlockedStickers = new(); //Gebruikt om poof te spawnen
    private List<string> oldUnlockedStickers = new(); //Wordt gebruikt om eventuele animatie af te spelen bij stickers die nieuw unlocked zijn.

    private Transform animationCanvas;
    private Patient currentPatient;
    private PatientApiClient patientApiClient;


    //private readonly StickerApiClient stickerApiClient = ApiClientManager.Instance.StickerApiClient;

    private async Task GetPatientUnlockedStickers(Patient patient)
    {
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

                    patientUnlockedStickerAmount.text = dataResponse.Data.Count().ToString();

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
        currentPatient = ApiClientManager.Instance.CurrentPatient;
        patientApiClient = ApiClientManager.Instance.PatientApiClient;


        await GetPatientUnlockedStickers(currentPatient);

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

        patientName.text = $"{currentPatient.firstName} {currentPatient.lastName}";
        var currentAvatar = currentPatient.avatar;
        
        
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

    public void ClearNewlyUnlockedStickers()
    {
        newUnlockedStickers.Clear();
    }

}
