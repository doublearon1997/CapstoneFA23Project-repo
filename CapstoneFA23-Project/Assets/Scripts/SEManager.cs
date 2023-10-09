using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEManager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip[] soundEffectsList;

    private Dictionary<string, AudioClip> soundEffects = new Dictionary<string, AudioClip>();

    public void Awake()
    {
        foreach (AudioClip clip in soundEffectsList)
        {
            soundEffects.Add(clip.name, clip);
        }
    }

    public void PlaySE(string fileName, float volume)
    {
        if (soundEffects[fileName] != null)
        {
            audioSource.PlayOneShot(soundEffects[fileName], volume);
        }
    }

    public void PlaySE(string fileName)
    {
        if (soundEffects[fileName] != null)
        {
            audioSource.PlayOneShot(soundEffects[fileName], 1.0f);
        }
    }

    public void PlaySE(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, 1.0f);
        }
    }

}
