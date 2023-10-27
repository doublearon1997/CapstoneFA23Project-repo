using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButtonSelectedHotkeys : MonoBehaviour
{
    private BattleSystem battle;
    private bool stop = false;

    public void Initialize(BattleSystem battle)
    {
        this.battle = battle;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1) && !stop)
        {
            battle.seManager.PlaySE("buttonReturn");
            battle.AttackButtonReturn();
            battle.DisplayMessage("" + battle.currentlyActingBattler.battlerName + "'s turn.");
            stop = true;
        }
            
    }
}
