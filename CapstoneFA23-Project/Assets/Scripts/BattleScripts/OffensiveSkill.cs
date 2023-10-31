using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "NewOffensiveSkill", menuName = "OffensiveSkill")]
public class OffensiveSkill : Skill
{
    public double dmgMod;

    // After a player has selected an offensive skill to use, this sets up the screen for the player to click on a target to use the skill.
    // TODO: Add ability for AOE.
    public void ChooseTarget(PlayerBattler user, BattleSystem battle)
    {
        if (this.targetType == TargetType.Single)
        {
            foreach (EnemyBattler battler in battle.enemyBattlers)
            {
                GameObject targetButton = Instantiate(battle.offensiveTarget100, battler.gameObject.transform.parent.transform) as GameObject;
                targetButton.GetComponent<OffensiveTargetButton>().Initialize(this, user, battler, battle);
                battle.currentTargetingObjects.Add(targetButton);
            }
        }
    }

    // When an enemy battler has chosen an offensive skill, this method calculates which player it will attack it with. Currently this will only work with single target attacks.
    // TODO: For AOE, need to change the return to a list of playerbattler targets.
    public PlayerBattler ChooseTarget(EnemyBattler user, BattleSystem battle)
    {
        if (this.targetType == TargetType.Single)
        {
            double cumulativeChance = 0;
            PlayerBattler target = null;
            List<double> chances = new List<double>();

            foreach (PlayerBattler battler in battle.playerBattlers)
            {
                cumulativeChance += battler.targetRatio;
                chances.Add(cumulativeChance);
            }

            double roll = (double)UnityEngine.Random.Range(0.0f, (float)cumulativeChance);

            for (int i = 0; i < battle.playerBattlers.Count && target == null; i++)
            {
                if (chances[i] > roll)
                    target = battle.playerBattlers[i];
            }

            if (target == null)
                target = battle.playerBattlers[0];

            return target;

        }

        return null;
    }

    // currently just deals damage. More may be added later, like hit or crit chance. Also need to turn the target into an array for aoe.
    public void UseSkill(Battler user, Battler target, BattleSystem battle)
    {
        // Current damage formula: (user's str * skill's dmgMod * (1- target's defense)) * random 0.9-1.1
        int damage;

        if(this.powerType == PowerType.Physical)
            damage = (int)((user.GetCurrStr() * this.dmgMod) * (1 - target.GetCurrDef()) * UnityEngine.Random.Range(0.9f, 1.1f));
        else
            damage = (int)((user.GetCurrWill() * this.dmgMod) * (1 - target.GetCurrRes()) * UnityEngine.Random.Range(0.9f, 1.1f));
            
        target.TakeDamage(damage, battle);

        ApplyEffects(user, target, battle);

        user.ap -= 100000;
        user.apMod = this.apMod;

        DisplayDamageText(damage, target, battle);
       
        if (user.isPlayer)
        {
            battle.DisplaySkillMessage(this);
            Dictionary<Skill, int> skillCooldowns = ((PlayerBattler)user).skillCooldownDict;
            foreach (Skill key in new List<Skill>(skillCooldowns.Keys))
            {
                if (skillCooldowns[key] > 0)
                        skillCooldowns[key] -= 1;
            }
            skillCooldowns[this] = this.cooldown;
            battle.StartCoroutine(battle.FinishPlayerTurn());
        }   
    }

    public void DisplayDamageText(int damage, Battler target, BattleSystem battle)
    {
        string displayString = "" + damage;

        GameObject damageTextContainer = Instantiate(battle.damageTextPopup, target.gameObject.transform);
        damageTextContainer.transform.GetChild(0).GetComponent<TMP_Text>().text = displayString;
    }

}
