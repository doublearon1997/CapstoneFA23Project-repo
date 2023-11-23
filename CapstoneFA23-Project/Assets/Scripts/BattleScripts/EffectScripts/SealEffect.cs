using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Defines Healing Effects, used to restore battlers' hp. Can heal based on a percentage of that battler's hp, a flat value, or a value based on the stats of the user.
[CreateAssetMenu(fileName = "NewSealEffect", menuName = "SealEffect")]
public class SealEffect : StatusEffect
{
    public override bool ApplyEffect(Battler user, Battler target, Skill skill, BattleSystem battle)
    {
        if(UnityEngine.Random.Range(0.0f, 1.0f) <= (chance * (1.0-target.GetCurrSealResistance())) && target.TryApplyStatusEffect(this))
        {
            target.willEnabled = false;
            return true;
        }
        else 
            return false;
    }

    public override void RemoveStatusEffect(Battler target, BattleSystem battle)
    {
        target.willEnabled = true;
    }

    public override string GetEffectStatsString()
    {
        return "Curse (" + chance*100 + "%, " + duration + ")";
    }

}
