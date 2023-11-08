using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SEManager : MonoBehaviour
{
    private static AudioSource audioSource;

    public static SEManager instance = null;

    public AudioClip[] seList;
    public float[] defaultVolumeList;

    private static Dictionary<string, AudioClip> soundEffects = new Dictionary<string, AudioClip>();
    private static Dictionary<AudioClip, float> soundEffectVolumes = new Dictionary<AudioClip, float>();

    private static HashSet<AudioClip> currentlyPlayingSEs = new HashSet<AudioClip>();

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.GetComponent<AudioSource>();

            for (int i = 0; i < seList.Length; i++)
            {
                soundEffects.TryAdd(seList[i].name, seList[i]);
                soundEffectVolumes.TryAdd(seList[i], defaultVolumeList[i]);
            }
            return;
        }
        if (instance == this)
            return;

        Destroy(gameObject);
    }

    public void OnLevelWasLoaded()
    {
        if (GameObject.Find("SEManagerInstance") != null)
            GameObject.Find("SEManagerInstance").GetComponent<SEManagerInstance>().managerInstance = instance;
    }

    public void PlaySE(string fileName, float volume)
    {
        if (soundEffects[fileName] != null)
        {
            audioSource.PlayOneShot(soundEffects[fileName], volume * PlayerPrefs.GetFloat("SEVolume"));
        }
    }

    public void PlaySE(string fileName)
    {
        if (!soundEffects.ContainsKey(fileName))
            throw new ArgumentException("Invalid filename or file not included in SEManager");

        if(soundEffectVolumes.ContainsKey(soundEffects[fileName]))
            audioSource.PlayOneShot(soundEffects[fileName], soundEffectVolumes[soundEffects[fileName]] * PlayerPrefs.GetFloat("SEVolume"));
        else
            audioSource.PlayOneShot(soundEffects[fileName], PlayerPrefs.GetFloat("SEVolume"));
    }

    public void PlaySE(AudioClip clip)
    {
        if (clip != null)
        {
            if (soundEffectVolumes.ContainsKey(clip)) 
                audioSource.PlayOneShot(clip, soundEffectVolumes[clip] * PlayerPrefs.GetFloat("SEVolume"));
            else
                audioSource.PlayOneShot(clip, PlayerPrefs.GetFloat("SEVolume"));
        }
    }

    //Plays a Soundeffect only if the same clip is not currently playing.
    public IEnumerator PlaySEOnlyOnce(string fileName)
    {
        if (!soundEffects.ContainsKey(fileName))
            throw new ArgumentException("Invalid filename or file not included in SEManager");

        AudioClip clip = soundEffects[fileName];

        if(!(currentlyPlayingSEs.Contains(clip)))
        {   
            currentlyPlayingSEs.Add(clip);

            if(soundEffectVolumes.ContainsKey(clip))
                audioSource.PlayOneShot(clip, soundEffectVolumes[clip] * PlayerPrefs.GetFloat("SEVolume"));
            else
                audioSource.PlayOneShot(clip, PlayerPrefs.GetFloat("SEVolume"));

            yield return new WaitForSeconds(clip.length);

            currentlyPlayingSEs.Remove(clip);
        }

    }

    public IEnumerator PlaySEOnlyOnce(AudioClip clip)
    {
        if(!(currentlyPlayingSEs.Contains(clip)))
        {   
            currentlyPlayingSEs.Add(clip);

            if(soundEffectVolumes.ContainsKey(clip))
                audioSource.PlayOneShot(clip, soundEffectVolumes[clip] * PlayerPrefs.GetFloat("SEVolume"));
            else
                audioSource.PlayOneShot(clip, PlayerPrefs.GetFloat("SEVolume"));

            yield return new WaitForSeconds(clip.length);

            currentlyPlayingSEs.Remove(clip);
        }

    }

}
