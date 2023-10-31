using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTargetHotkeys : MonoBehaviour
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
            battle.SkillTargetReturn();
            battle.DisplayMessage("Select a skill.");

            battle.hotkeyManager.AddComponent<SkillsButtonSelectedHotkeys>().Initialize(battle);
            stop = true;
        }
            

    }
}
