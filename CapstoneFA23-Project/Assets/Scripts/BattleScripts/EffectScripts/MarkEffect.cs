using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Defines Mark Effects, a status effect that doubles the chances for an enemy to attack a player. Has no effect on the player, so don't use it for skills that target an enemy.
[CreateAssetMenu(fileName = "NewMarkEffect", menuName = "MarkEffect")]
public class MarkEffect : StatusEffect
{
    public override bool ApplyEffect(Battler user, Battler target, Skill skill, BattleSystem battle)
    {
        if(UnityEngine.Random.Range(0.0f, 1.0f) <= chance && target.TryApplyStatusEffect(this))
        {
            ((PlayerBattler)target).targetRatio = 2.0;
            return true;
        }
        else 
            return false;
    }

    public override void RemoveStatusEffect(Battler target, BattleSystem battle)
    {
         ((PlayerBattler)target).targetRatio = 1.0;
    }

    public override string GetEffectStatsString()
    {
        return "Mark (" + chance*100 + "%)";
    }

}
