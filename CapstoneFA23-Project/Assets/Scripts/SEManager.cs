using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SEManager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip[] soundEffectsList;
    public float[] defaultVolumeList;

    private Dictionary<string, AudioClip> soundEffects = new Dictionary<string, AudioClip>();
    private Dictionary<AudioClip, float> soundEffectVolume = new Dictionary<AudioClip, float>();

    public void Awake()
    {
        for(int i = 0; i< soundEffectsList.Length; i++)
        {
            soundEffects.Add(soundEffectsList[i].name, soundEffectsList[i]);
            soundEffectVolume.Add(soundEffectsList[i], defaultVolumeList[i]);
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
        if (!soundEffects.ContainsKey(fileName))
            throw new ArgumentException("Invalid filename or file not included in SEManager");

        if(soundEffectVolume.ContainsKey(soundEffects[fileName]))
            audioSource.PlayOneShot(soundEffects[fileName], soundEffectVolume[soundEffects[fileName]]);
        else
            audioSource.PlayOneShot(soundEffects[fileName], 1.0f);
    }

    public void PlaySE(AudioClip clip)
    {
        if (clip != null)
        {
            if (soundEffectVolume.ContainsKey(clip))
                audioSource.PlayOneShot(clip, soundEffectVolume[clip]);
            else
                audioSource.PlayOneShot(clip, 1.0f);
        }
    }

}
