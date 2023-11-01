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

    public enum AttackResult {Normal, Crit}

    // After a player has selected an offensive skill to use, this sets up the screen for the player to click on a target to use the skill.
    // TODO: Add ability for AOE.
    public void ChooseTarget(PlayerBattler user, BattleSystem battle)
    {
        if (this.targetType == TargetType.Single)
        {
            foreach (EnemyBattler battler in battle.enemyBattlers)
            {
                List<Battler> target = new List<Battler>();
                target.Add(battler);

                GameObject targetButton = Instantiate(battle.offensiveTarget100, battler.gameObject.transform.parent.transform) as GameObject;
                targetButton.GetComponent<OffensiveTargetButton>().Initialize(this, user, target, battle);
                battle.currentTargetingObjects.Add(targetButton);
            }
        }
        else if (targetType == TargetType.All)
        {
            List<Battler> targets = new List<Battler>();

            foreach (EnemyBattler battler in battle.enemyBattlers)
                targets.Add(battler);

            GameObject targetAllButton = Instantiate(battle.offensiveTarget550, battle.enemyAOELoc) as GameObject;
            targetAllButton.GetComponent<OffensiveTargetButton>().Initialize(this, user, targets, battle);
            battle.currentTargetingObjects.Add(targetAllButton);
        }
    }

    // When an enemy battler has chosen an offensive skill, this method calculates which player it will attack it with. Currently this will only work with single target attacks.
    // TODO: For AOE, need to change the return to a list of playerbattler targets.
    public List<PlayerBattler> ChooseTarget(EnemyBattler user, BattleSystem battle)
    {
        List<PlayerBattler> targets = new List<PlayerBattler>();

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

            targets.Add(target);
        }
        else if(targetType == TargetType.All)
        {
            foreach(PlayerBattler battler in battle.playerBattlers)
                targets.Add(battler);
        }

        return targets;
    }

    // currently just deals damage. More may be added later, like hit or crit chance. Also need to turn the target into an array for aoe.
    public void UseSkill(Battler user, List<Battler> targets, BattleSystem battle)
    {
        foreach (Battler target in targets)
        {
            // Current damage formula: (user's str * skill's dmgMod * (1- target's defense)) * random 0.9-1.1
            double damage;
            AttackResult result = AttackResult.Normal;

            if (this.powerType == PowerType.Physical)
                damage = (user.GetCurrStr() * this.dmgMod) * (1 - target.GetCurrDef()) * UnityEngine.Random.Range(0.9f, 1.1f);
            else
                damage = (user.GetCurrWil() * this.dmgMod) * (1 - target.GetCurrRes()) * UnityEngine.Random.Range(0.9f, 1.1f);

            //Critical Hit
            if (UnityEngine.Random.Range(0.0f, 1.0f) < user.GetCurrCrt())
            {
                damage *= 1.5;
                result = AttackResult.Crit;
            }

            int finalDamage = (int)damage;

            target.TakeDamage(finalDamage, battle);

            ApplyEffects(user, target, battle);

            DisplayDamageText(finalDamage, target, battle, result);

        }

        user.ap -= 100000;
        user.apMod = this.apMod;

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

    public void DisplayDamageText(int damage, Battler target, BattleSystem battle, AttackResult result)
    {
        string displayString;

        if(result == AttackResult.Crit)
            displayString = "Crit!\n" + damage;
        else
            displayString = "" + damage;

        GameObject damageTextContainer = Instantiate(battle.damageTextPopup, target.gameObject.transform);
        damageTextContainer.transform.GetChild(0).GetComponent<TMP_Text>().text = displayString;
    }

}
