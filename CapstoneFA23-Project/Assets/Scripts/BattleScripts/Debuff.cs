using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDebuff", menuName = "Debuff")]
public class Debuff : BuffEffect
{
    public override string GetEffectStatsString()
    {
        string returnString = this.value * 100 + "% " + ((this.buffStat.ToString()).Substring(0, this.buffStat.ToString().Length-6)).ToUpper();

        if (this.decayValue > 0)
            returnString += " (+" + this.decayValue * 100 + "%/turn)";
        else
            returnString += " (" + this.duration * 100 + "turns)";

        return returnString;
    }

    public override void ApplyEffect(Battler user, Battler target, BattleSystem battle)
    {
        if (!target.buffEffects.ContainsKey(this)) //don't reapply buff. change later.
        {
            target.buffEffects.Add(this, this.duration);
            target.SetBuffStat(this.buffStat, this.value);
        }
    }

    public override void ApplyDecay(Battler target, BattleSystem battle)
    {
        target.DecayBuffStat(this.buffStat, this.decayValue);
    }
}
