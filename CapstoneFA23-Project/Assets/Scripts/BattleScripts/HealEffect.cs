using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Defines Healing Effects, used to restore battlers' hp. Can heal based on a percentage of that battler's hp, a flat value, or a value based on the stats of the user.
[CreateAssetMenu(fileName = "NewHealEffect", menuName = "HealEffect")]
public class HealEffect : Effect
{
    public double healPercentage = 0;
    public int healAmount = 0;
    public double healPower = 0;
    public PowerType powerType = PowerType.Will;

    public override void ApplyEffect(Battler user, Battler target, BattleSystem battle)
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
        DisplayHealText((int)hpHealed, target, battle);
    }

    public override string GetEffectStatsString()
    {
        string powerString = "";
        string percentString = "";
        string amountString = "";
        string returnString = "Heal (";

        if(healPower > 0)
        {
            powerString = "x" + healPower + "";
            returnString += powerString + ", +";
        }

        if(healPercentage > 0)
        {
            percentString = "" + healPercentage * 100 + "%";
            returnString += percentString + ", +";
        }
            
        if(healAmount > 0)
        {
            amountString = "" + healAmount;
            returnString += amountString + ", +";
        }

        return returnString.Substring(0, returnString.Length-3) + ")";
        
    }

    public void DisplayHealText(int hpHealed, Battler target, BattleSystem battle)
    {
        string displayString = "" + hpHealed;

        GameObject damageTextContainer = Instantiate(battle.healTextPopup, target.gameObject.transform);
        damageTextContainer.transform.GetChild(0).GetComponent<TMP_Text>().text = displayString;
    }

}
