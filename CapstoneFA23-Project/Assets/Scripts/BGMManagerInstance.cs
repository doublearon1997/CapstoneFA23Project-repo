using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManagerInstance : MonoBehaviour
{
    public BGMManager managerInstance;

    public void PlayBGM(AudioClip clip)
    {
        managerInstance.PlayBGM(clip);
    }
}
