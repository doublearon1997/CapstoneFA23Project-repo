using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsButtonSelectedHotkeys : MonoBehaviour
{
    private BattleSystem battle;
    private bool stop = false;

    public void Initialize(BattleSystem battle)
    {
        this.battle = battle;
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.Mouse1) && !stop)
        {
            battle.SkillsButtonReturn();
            stop = true;
        }
            
    }
}
