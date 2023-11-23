using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewClearCooldownsEffect", menuName = "ClearCooldownsEffect")]
public class ClearCooldownsEffect : Effect
{
    public override bool ApplyEffect(Battler user, Battler target, Skill skill, BattleSystem battle)
    {
        if(target.isPlayer)
        {
            PlayerBattler player = (PlayerBattler)target;
            foreach(Skill s in new List<Skill>(player.skillCooldownDict.Keys))
            {
                if(player.skillCooldownDict[s] > 0)
                    player.skillCooldownDict[s] = 0;
            }
            return true;
        }

        return false;
    }

    public override string GetEffectStatsString()
    {
        return "Clear Cooldowns";
    }
}
