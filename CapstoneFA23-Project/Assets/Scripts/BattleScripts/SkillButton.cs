using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    private Skill skill;
    private PlayerBattler user;
    private BattleSystem battle;
    public void Initialize(Skill skill, PlayerBattler user, BattleSystem battle)
    {
        this.skill = skill;
        this.user = user;
        this.battle = battle;

        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = skill.portrait100;
    }

    public void SkillButtonPress()
    {
        if(skill.isOffensive)  
            ((OffensiveSkill)skill).ChooseTarget(user, battle);
    }

}
