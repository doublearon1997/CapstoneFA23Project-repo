using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsButtonSelectedHotkeys : MonoBehaviour
{
    private BattleSystem battle;

    public void Initialize(BattleSystem battle)
    {
        this.battle = battle;
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.Mouse1))
            battle.SkillsButtonReturn();
    }
}
