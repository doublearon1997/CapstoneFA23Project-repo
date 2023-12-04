using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveMusic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BGMManager.instance.stopBGM();
    }

}
