using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Defines KO Effects, a state a player battler reaches when they are at 0 hp. When all playerbattlers are KO'ed, it is game over. This effect should not be added to skills.
[CreateAssetMenu(fileName = "KOEffect", menuName = "KOEffect")]
public class KOEffect : StatusEffect
{
    public override bool ApplyEffect(Battler user, Battler target, Skill skill, BattleSystem battle)
    {
        if(target.isPlayer)
        {
            ((PlayerBattler)target).isKO = true;
            return true;
        }
        else 
            return false;
    }

    public override void RemoveStatusEffect(Battler target, BattleSystem battle)
    {
        ((PlayerBattler)target).isKO = false;
    }

    public override string GetEffectStatsString()
    {
        return "Knocked Out";
    }

}
