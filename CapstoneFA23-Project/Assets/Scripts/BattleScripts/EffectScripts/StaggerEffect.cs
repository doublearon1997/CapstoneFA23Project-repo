using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Defines Stagger Effects, a status effect that causes the next hit the battler takes to be a critical hit.
[CreateAssetMenu(fileName = "NewStaggerEffect", menuName = "StaggerEffect")]
public class StaggerEffect : StatusEffect
{
    public override bool ApplyEffect(Battler user, Battler target, Skill skill, BattleSystem battle)
    {
        if(UnityEngine.Random.Range(0.0f, 1.0f) <= (chance * (1.0-target.GetCurrStaggerResistance())) && target.TryApplyStatusEffect(this))
        {
            target.nextHitCrit = true;
            return true;
        }
        else 
            return false;
    }

    public override void RemoveStatusEffect(Battler target, BattleSystem battle)
    {
        target.nextHitCrit = false;
    }

    public override string GetEffectStatsString()
    {
        return "Stagger (" + chance*100 + "%)";
    }

}
