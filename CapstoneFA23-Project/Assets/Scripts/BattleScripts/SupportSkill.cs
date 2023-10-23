using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSupportSkill", menuName = "SupportSkill")]
public class SupportSkill : Skill
{ 
    // After a player has selected a support skill to use, this sets up the screen for the player to click on a target to use the skill.
    // TODO: Add ability for AOE.
    public void ChooseTarget(PlayerBattler user, BattleSystem battle)
    {
        if (this.targetType == TargetType.Single)
        {
            foreach (PlayerBattler battler in battle.playerBattlers)
            {
                GameObject targetButton = Instantiate(battle.supportTarget100, battler.gameObject.transform.parent.transform) as GameObject;
                targetButton.GetComponent<SupportTargetButton>().Initialize(this, user, battler, battle);
                battle.currentTargetingObjects.Add(targetButton);
            }
        }
    }

    // When an enemy battler has chosen an offensive skill, this method calculates which player it will attack it with. Currently this will only work with single target attacks.
    // TODO: For AOE, need to change the return to a list of enemybattler targets.
    public EnemyBattler ChooseTarget(EnemyBattler user, BattleSystem battle)
    {
        if (this.targetType == TargetType.Single)
        {
            double cumulativeChance = 0;
            EnemyBattler target = null;
            List<double> chances = new List<double>();

            foreach (EnemyBattler battler in battle.enemyBattlers)
            {
                cumulativeChance += 1.0;
                chances.Add(cumulativeChance);
            }

            double roll = (double)UnityEngine.Random.Range(0.0f, (float)cumulativeChance);

            for (int i = 0; i < battle.enemyBattlers.Count && target == null; i++)
            {
                if (chances[i] > roll)
                    target = battle.enemyBattlers[i];
            }

            if (target == null)
                target = battle.enemyBattlers[0];

            return target;

        }

        return null;
    }

    public void UseSkill(Battler user, Battler target, BattleSystem battle)
    {
        user.ap -= 100000;
        user.apMod = this.apMod;

        ApplyEffects(user, target, battle);

        if(user.isPlayer)
        {
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

    
}
