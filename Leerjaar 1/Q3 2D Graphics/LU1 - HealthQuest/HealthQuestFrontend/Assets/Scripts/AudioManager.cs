using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

public class AudioManager : MonoBehaviour
{
    [Header("Music Tracks")]
    public AudioClip initializeBackgroundMusic;

    [Header("Sound Effects")]
    public List<AudioClip> initializeSoundEffects;
    
    public AudioSource audioSource;
    public AudioClip backgroundMusic;
    public List<AudioClip> soundEffects = new();
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

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
    }

    public void StartMusic()
    {
        audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }
}