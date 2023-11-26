using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewClearDebuffsEffect", menuName = "ClearDebuffsEffect")]
public class ClearDebuffsEffect : Effect
{
    public override bool ApplyEffect(Battler user, Battler target, Skill skill, BattleSystem battle)
    {
        bool atLeastOneDebuff = false;
        List<BuffEffect> removeEffects = new List<BuffEffect>();

        foreach(BuffEffect effect in target.buffEffects.Keys)
        {
            if(effect.isDebuff)
            {
                atLeastOneDebuff = true;
                target.DecayBuffStat(effect.buffStat, 9999);
                removeEffects.Add(effect);
            }
        }

        foreach(BuffEffect effect in removeEffects)
            target.buffEffects.Remove(effect);
        
        return atLeastOneDebuff;
    }

    public override string GetEffectStatsString()
    {
        return "Clear Debuffs";
    }
}
