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
            SEManager.instance.PlaySE("buttonReturn");

            battle.SkillTargetReturn();

            if (battle.IsButtonSkillsPressed())
            {
                battle.DisplayMessage("Select a skill.");
                battle.hotkeyManager.AddComponent<SkillsButtonSelectedHotkeys>().Initialize(battle);
            }
            else if(battle.IsButtonTacticsPressed())
            {
                battle.DisplayMessage("Select a tactic.");
                battle.hotkeyManager.AddComponent<TacticsButtonSelectedHotkeys>().Initialize(battle);
            }

            stop = true;
        }
            

    }
}
