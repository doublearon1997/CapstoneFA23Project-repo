using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuff", menuName = "Buff")]
public class Buff : BuffEffect
{
    public override string GetEffectStatsString()
    {
        string returnString = "+" + this.value*100 + "% " + ((this.buffStat.ToString()).Substring(0, this.buffStat.ToString().Length - 4)).ToUpper();

        if (this.decayValue < 0)
            returnString += " (" + this.decayValue * 100 + "%/turn)";
        else
            returnString += " (" + this.duration * 100 + "turns";

        return returnString;
    }

    public override bool ApplyEffect(Battler user, Battler target, Skill skill, BattleSystem battle)
    {
        if(target.TryApplyBuffEffect(this, battle))
        {
            target.SetBuffStat(this.buffStat, this.value);

            if(user == target)
                user.AddCurrentTurnEffect(this);

            return true;
        }
        return false;
    }

    public override void ApplyDecay(Battler target, BattleSystem battle)
    {
        target.DecayBuffStat(this.buffStat, this.decayValue);
    }
}
