using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Defines Healing Effects, used to restore battlers' hp. Can heal based on a percentage of that battler's hp, a flat value, or a value based on the stats of the user.
public class HealEffect : Effect
{
    private double healPercentage;
    private int healAmount;
    private double healPower;
    private PowerType powerType;

    public void Initialize(double healPercentage = 0, int healAmount = 0, int healPower = 0, PowerType powerType = PowerType.Will)
    {
        this.healPercentage = healPercentage;
        this.healAmount = healAmount;
        this.healPower = healPower;
        this.powerType = powerType;
    }

    public override void ApplyEffects(Battler user, Battler target, BattleSystem battle)
    {
        double hpHealed = 0;
        if(healPercentage > 0)
            hpHealed += healPercentage * (double)target.mhp;
        if(healAmount > 0)
            hpHealed += healAmount;
        if(healPower > 0)
        {
            if(powerType == PowerType.Will)
                hpHealed += healPower * (double)user.will;
            else 
                hpHealed += healPower * (double)user.str;
        }

        target.HealBattler((int)hpHealed, battle);
    }

}
