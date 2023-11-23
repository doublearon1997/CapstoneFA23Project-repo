using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Defines Healing Effects, used to restore battlers' hp. Can heal based on a percentage of that battler's hp, a flat value, or a value based on the stats of the user.
[CreateAssetMenu(fileName = "NewCurseEffect", menuName = "CurseEffect")]
public class CurseEffect : StatusEffect
{
    public override bool ApplyEffect(Battler user, Battler target, Skill skill, BattleSystem battle)
    {
        if(UnityEngine.Random.Range(0.0f, 1.0f) <= (chance * (1.0-target.GetCurrCurseResistance())) && target.TryApplyStatusEffect(this))
        {
            target.physicalEnabled = false;
            return true;
        }
        else 
            return false;
    }

    public override void RemoveStatusEffect(Battler target, BattleSystem battle)
    {
        target.physicalEnabled = true;
    }

    public override string GetEffectStatsString()
    {
        return "Curse (" + chance*100 + "%, " + duration + ")";
    }

}
