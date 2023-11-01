using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsButtonSelectedHotkeys : MonoBehaviour
{
    private BattleSystem battle;
    private bool stop = false;

    public void Initialize(BattleSystem battle)
    {
        this.battle = battle;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && !stop)
        {
            SEManager.instance.PlaySE("buttonReturn");
            battle.TacticsButtonReturn();
            battle.DisplayMessage("" + battle.currentlyActingBattler.battlerName + "'s turn.");
            stop = true;
        }
            
    }
}
