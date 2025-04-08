using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

public class AudioManager : MonoBehaviour
{
    [Header("Music Tracks")]
    public AudioClip initializeBackgroundMusic;

    [Header("Sound Effects")]
    public List<AudioClip> initializeSoundEffects;
    
    public static AudioSource audioSource;
    public static AudioClip backgroundMusic;
    public static List<AudioClip> soundEffects = new();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();

        backgroundMusic = initializeBackgroundMusic;
        foreach (var item in initializeSoundEffects)
        {
            soundEffects.Add(item);
        }
    }

    private void Start()
    {
        audioSource.clip = backgroundMusic;
        audioSource.Play();
    }

}
