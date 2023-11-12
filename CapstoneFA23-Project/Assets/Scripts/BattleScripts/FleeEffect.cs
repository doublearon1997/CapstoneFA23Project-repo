using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Defines Healing Effects, used to restore battlers' hp. Can heal based on a percentage of that battler's hp, a flat value, or a value based on the stats of the user.
[CreateAssetMenu(fileName = "FleeEffect", menuName = "FleeEffect")]
public class Fleeffect : Effect
{
    
    public override bool ApplyEffect(Battler user, Battler target, Skill skill, BattleSystem battle)
    {
        user.Flee(battle);
        
        if(target.isPlayer)
        {
            battle.playerBattlers.Remove((PlayerBattler)target);
        }
        else 
        {
            battle.enemyBattlers.Remove((EnemyBattler)target);
        }
            
        battle.DisplayMessage("" + target.battlerName + " fled.");

        return true;
    }

    public override string GetEffectStatsString()
    {
        return "Flee!";
    }

}
