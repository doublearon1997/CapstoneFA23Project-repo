using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Defines Deathblow effects, which instantly kill battlers. 
[CreateAssetMenu(fileName = "NewDeathblowEffect", menuName = "DeathblowEffect")]
public class DeathblowEffect : Effect
{
    public int maxLevelToDeathblow = -1;

    public override bool ApplyEffect(Battler user, Battler target, Skill skill, BattleSystem battle)
    {
        if(maxLevelToDeathblow == -1 || target.level <= maxLevelToDeathblow)
        {
            battle.StartCoroutine(DelayKill(target, skill, battle));
            return true;
        }
        else
            return false;
        
    }

    public override string GetEffectStatsString()
    {
        if (maxLevelToDeathblow == -1)
            return "Deathblow";
        else
            return "Deathblow (Max Lvl " + maxLevelToDeathblow + ")";
        
    }

    public IEnumerator DelayKill(Battler target, Skill skill, BattleSystem battle)
    {
        yield return new WaitForSeconds(skill.soundEffectHitDelay);

        target.KillBattler(battle);
    }

}
