using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using System.Collections; 

public class StickerBoeken : MonoBehaviour
{
    [Header("Stickers")]
    public GameObject[] stickers;

    public GameObject poofPrefab;
    

    public static string[] newUnlockedStickers = {"Ziekenhuis"}; //Wordt gebruikt om eventuele animatie af te spelen bij stickers die nieuw unlocked zijn.
    public static string[] oldUnlockedStickers = { "Ambulance", "Bed"}; //Wordt gebruikt om eventuele animatie af te spelen bij stickers die nieuw unlocked zijn.

    private Transform animationCanvas;


    void Start()
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
