using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEManagerInstance : MonoBehaviour
{
    public SEManager managerInstance;

    public void PlaySE(AudioClip clip)
    {
        managerInstance.PlaySE(clip);
    }

}
