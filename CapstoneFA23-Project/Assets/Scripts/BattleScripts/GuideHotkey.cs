using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideHotkey : MonoBehaviour
{
    private BattleSystem battle;
    private bool stop = false;
    private bool helpOn = false;

    public void Initialize(BattleSystem battle)
    {
        this.battle = battle;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab) && !stop)
        {
            battle.helpPanel.SetActive(true);
            helpOn = true;
            stop = true;
        }
        if(helpOn && Input.GetKeyUp(KeyCode.Tab))
        {
            battle.helpPanel.SetActive(false);
            helpOn = false;
            stop = false;
        }
    }
}
