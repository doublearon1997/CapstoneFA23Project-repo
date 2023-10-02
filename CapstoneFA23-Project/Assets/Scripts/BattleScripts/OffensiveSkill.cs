using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "NewOffensiveSkill", menuName = "OffensiveSkill")]
public class OffensiveSkill : Skill
{
    public double dmgMod; 

    public void ChooseTarget(PlayerBattler user, BattleSystem battle)
    {
        if (this.targetType == TargetType.Single)
        {
            foreach (EnemyBattler battler in battle.enemyBattlers)
            {
                battler.gameObject.AddComponent<HandlerTargetSelected>().initialize(this, user, battler, battle);
               // battler.gameObject.GetComponent<Sprite>().
            }
       
        }
    }

    private void UseSkill(Battler user, Battler target)
    {

        
        // Current damage formula: (user's str * skill's dmgMod * (1- target's defense)) * random 0.9-1.1

        int damage = (int)(((user.str * this.dmgMod)* (1-target.def))  * UnityEngine.Random.Range(0.9f, 1.1f));
        target.hp = target.hp - damage;

    }

    //Event handler that selects the target of the skill based on what battler was clicked.

    internal class HandlerTargetSelected: MonoBehaviour
    {
        public Battler user;
        public Battler target;
        public OffensiveSkill skill;
        public BattleSystem battle;

        void OnMouseDown()
        {
            skill.UseSkill(battle.currentlyActingBattler, target);
        }

        public void initialize(OffensiveSkill skill, Battler user, Battler target, BattleSystem battle)
        {
            this.skill = skill;
            this.user = user;
            this.target = target;
            this.battle = battle;
                  
        }
    }

   

   

}
