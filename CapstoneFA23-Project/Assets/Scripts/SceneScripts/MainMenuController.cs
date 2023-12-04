using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    void Start()
    {
        BGMManager.instance.PlayBGM("thePirateShip");
    }
    public void exitMainMenu()
    {
        BGMManager.instance.stopBGM();
    }
    
}