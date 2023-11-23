using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSupportSkill", menuName = "SupportSkill")]
public class SupportSkill : Skill
{ 
    // After a player has selected a support skill to use, this sets up the screen for the player to click on a target to use the skill.
    public void ChooseTarget(PlayerBattler user, BattleSystem battle)
    {
        if (this.targetType == TargetType.Single)
        {
            foreach (PlayerBattler battler in battle.playerBattlers)
            {
                List<Battler> target = new List<Battler>();
                target.Add(battler);

                GameObject targetButton = Instantiate(battle.supportTarget100, battler.gameObject.transform.parent.transform) as GameObject;
                targetButton.GetComponent<SupportTargetButton>().Initialize(this, user, target, battle);
                battle.currentTargetingObjects.Add(targetButton);
            }
        }
        else if (this.targetType == TargetType.Self)
        {
            List<Battler> target = new List<Battler>();
                target.Add(user);

            GameObject targetButton = Instantiate(battle.supportTarget100, user.gameObject.transform.parent.transform) as GameObject;
            targetButton.GetComponent<SupportTargetButton>().Initialize(this, user, target, battle);
            battle.currentTargetingObjects.Add(targetButton);
        }
        else if (targetType == TargetType.All)
        {
            List<Battler> targets = new List<Battler>();

            foreach (PlayerBattler battler in battle.playerBattlers)
                targets.Add(battler);

            GameObject targetAllButton = Instantiate(battle.supportTarget500, battle.playerAOELoc) as GameObject;
            targetAllButton.GetComponent<SupportTargetButton>().Initialize(this, user, targets, battle);
            battle.currentTargetingObjects.Add(targetAllButton);
        }
    }

    // When an enemy battler has chosen an offensive skill, this method calculates which player it will attack it with. Currently this will only work with single target attacks.
    // TODO: For AOE, need to change the return to a list of enemybattler targets.
    public List<EnemyBattler> ChooseTarget(EnemyBattler user, BattleSystem battle)
    {
        List<EnemyBattler> targets = new List<EnemyBattler>();

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

            targets.Add(target);
        }
        else if(targetType == TargetType.Self)
        {
            targets.Add(user);
        }
        else if(targetType == TargetType.All)
        {
            foreach(EnemyBattler battler in battle.enemyBattlers)
                targets.Add(battler);
        }

        return targets;
    }

    public void UseSkill(Battler user, List<Battler> targets, BattleSystem battle)
    {
        int maxAdditionalAnimations = 0;

        foreach(Battler target in targets)
        {
            bool[] displayFlags = ApplyEffects(user, target, battle);
            List<GameObject> effectNotificationQueue = new List<GameObject>();
            List<string> effectSoundEffectQueue = new List<string>();

            FillNotificationQueues(effectNotificationQueue, effectSoundEffectQueue, displayFlags, battle);

            if(effectNotificationQueue.Count > maxAdditionalAnimations)
                maxAdditionalAnimations = effectNotificationQueue.Count;

            battle.StartCoroutine(DisplayAnimations(target, battle, effectNotificationQueue, effectSoundEffectQueue));
        }

        user.ap -= 100000;
        user.apMod = this.apMod;

        if(user.isPlayer)
        {
            battle.DisplaySkillMessage(this);
            Dictionary<Skill, int> skillCooldowns = ((PlayerBattler)user).skillCooldownDict;
            foreach (Skill key in new List<Skill>(skillCooldowns.Keys))
            {
                if (skillCooldowns[key] > 0)
                    skillCooldowns[key] -= 1;
            }
            skillCooldowns[this] = this.cooldown;

            if(this is SupportItemSkill)
                battle.inventory.removeItem(((SupportItemSkill)this).item.itemID, 1);
            
            battle.StartCoroutine(battle.FinishPlayerTurn(maxAdditionalAnimations, soundEffectHitDelay));
        }
        else 
        {
            battle.StartCoroutine(battle.FinishEnemyTurn(maxAdditionalAnimations, soundEffectHitDelay));
        }

    }

    IEnumerator DisplayAnimations(Battler target, BattleSystem battle, List<GameObject> effectNotificationQueue, List<string> effectSoundEffectQueue)
    {
        SEManager.instance.PlaySE(soundEffect);
        yield return new WaitForSeconds(soundEffectHitDelay);

        if(hitSoundEffect != null)
            SEManager.instance.PlaySE(hitSoundEffect);

        yield return new WaitForSeconds(1.7f);

        for(int i = 0; i< effectNotificationQueue.Count; i++)
        {
            GameObject obj = Instantiate(effectNotificationQueue[i], target.gameObject.transform) as GameObject;
            SEManager.instance.StartCoroutine(SEManager.instance.PlaySEOnlyOnce(effectSoundEffectQueue[i]));

            yield return new WaitForSeconds(1.6f);

            Destroy(obj);
        }
    }
}
