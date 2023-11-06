using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffensiveTargetButton : MonoBehaviour
{
    public Battler user;
    public List<Battler> targets;
    public OffensiveSkill skill;
    public BattleSystem battle;

    public void Initialize(OffensiveSkill skill, Battler user, List<Battler> targets, BattleSystem battle)
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
