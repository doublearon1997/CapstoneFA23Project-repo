using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportTargetButton : MonoBehaviour
{
    public Battler user;
    public List<Battler> targets;
    public SupportSkill skill;
    public BattleSystem battle;

    public void Initialize(SupportSkill skill, Battler user, List<Battler> targets, BattleSystem battle)
    {
        this.skill = skill;
        this.user = user;
        this.targets = targets;
        this.battle = battle;
    }

    public void TargetButtonHover()
    {
        SEManager.instance.PlaySE("buttonHover", 1);
    }

    public void TargetButtonPress()
    {
        battle.PlayerActionSelected();
        skill.UseSkill(battle.currentlyActingBattler, targets, battle);
    }

}
