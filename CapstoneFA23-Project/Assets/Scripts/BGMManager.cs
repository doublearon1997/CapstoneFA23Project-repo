using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BGMManager : MonoBehaviour
{
    private static AudioSource audioSource;
    public static BGMManager instance = null;

    public AudioClip[] bgmList;
    public float[] defaultVolumeList;

    private static Dictionary<string, AudioClip> bgms = new Dictionary<string, AudioClip>();
    private static Dictionary<AudioClip, float> bgmVolumes = new Dictionary<AudioClip, float>();

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.GetComponent<AudioSource>();

            for (int i = 0; i < bgmList.Length; i++)
            {
                bgms.TryAdd(bgmList[i].name, bgmList[i]);
                bgmVolumes.TryAdd(bgmList[i], defaultVolumeList[i]);
            }
            return;
        }
        if (instance == this)
        {
            return;
        }
      
        Destroy(gameObject);

    }

    public void OnLevelWasLoaded()
    {
        if(GameObject.Find("BGMManagerInstance") != null)
            GameObject.Find("BGMManagerInstance").GetComponent<BGMManagerInstance>().managerInstance = instance;
    }

    public void PlayBGM(string fileName, float volume)
    {
        if (!bgms.ContainsKey(fileName))
            throw new ArgumentException("Invalid filename or file not included in BGMManager");

        audioSource.clip = bgms[fileName];

        audioSource.volume = PlayerPrefs.GetFloat("BGMVolume") * volume;

        audioSource.Play();
    }

    public void PlayBGM(string fileName)
    {
        if (!bgms.ContainsKey(fileName))
            throw new ArgumentException("Invalid filename or file not included in BGMManager");

        audioSource.clip = bgms[fileName];

        if (bgmVolumes.ContainsKey(bgms[fileName]))
            audioSource.volume = bgmVolumes[bgms[fileName]] * PlayerPrefs.GetFloat("BGMVolume");
        else
            audioSource.volume = PlayerPrefs.GetFloat("BGMVolume");

        audioSource.Play();
    }

    public void PlayBGM(AudioClip clip, float time = 0)
    {
        if(audioSource.clip == null || clip != audioSource.clip)
        {
            audioSource.Stop();

            if(time != 0)
                StartCoroutine(FadeInBGM(clip, 1.0f, time));
            else 
            {
                audioSource.clip = clip;
                audioSource.time = 0;
                if (bgmVolumes.ContainsKey(clip))
                    audioSource.volume = bgmVolumes[clip] * PlayerPrefs.GetFloat("BGMVolume");
                else
                    audioSource.volume = PlayerPrefs.GetFloat("BGMVolume");
                audioSource.Play();
            }
        }
    }

    public IEnumerator FadeOutBGM(float fadeDuration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeDuration;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    public IEnumerator FadeInBGM(AudioClip clip, float fadeDuration, float startTime = 0)
    {
        float endVolume;

        if (bgmVolumes.ContainsKey(clip))
            endVolume = bgmVolumes[clip] * PlayerPrefs.GetFloat("BGMVolume");
        else
            endVolume = PlayerPrefs.GetFloat("BGMVolume");

        audioSource.clip = clip;
        audioSource.time = startTime;

        audioSource.Play();

        audioSource.volume = 0;

        while(audioSource.volume < endVolume)
        {
            audioSource.volume += endVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }
    }

    public void stopBGM()
    {  
        audioSource.Stop();
    }

    public float GetCurrentBGMTime()
    {
        return audioSource.time;
    }

}
