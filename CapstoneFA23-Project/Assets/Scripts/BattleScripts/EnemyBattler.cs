using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

public class EnemyBattler : Battler
{
    public Dictionary<Skill, SkillUsageAI> skillsDict = new Dictionary<Skill, SkillUsageAI>();

    public SkillUsageAI[] skillUsageAIList;

    public List<Item> dropList = null;
    public List<double> dropListChances =  null;

    public List<int> dropListQuantities = null;

    public int exp;

    private void Awake()
    {
        for(int i = 0; i<skills.Count; i++)
        {
            skillUsageAIList[i].Initialize();
            skillsDict.Add(skills[i], skillUsageAIList[i]);
        }
    }

    //This method selects an available skill for the enemy to use. If they don't have any available skills, they use the wait skill.
    public Skill ChooseSkill(BattleSystem battle)
    {
        Skill chosenSkill;
        List<Skill> skillChoices = new List<Skill>();

        foreach(KeyValuePair<Skill, SkillUsageAI> skillEntry in skillsDict)
        {
            if(skillEntry.Key.powerType == PowerType.Physical && !this.physicalEnabled){} // don't select this skill if powertype is diabled
            else if(skillEntry.Key.powerType == PowerType.Will && !this.willEnabled){} 
            else 
            {
                int maxWeight = 0;
                bool canUse = false;

                foreach (KeyValuePair<SkillUsageCondition, int> conditionEntry in skillEntry.Value.skillUsages)
                {
                    if(CanUseSkill(conditionEntry.Key))
                    {
                        canUse = true;
                        int weight = conditionEntry.Value;
                        if(weight > maxWeight)
                            maxWeight = weight;
                    }
                }

                if(canUse)
                {
                    for(int num = 0; num < maxWeight; num++)
                        skillChoices.Add(skillEntry.Key);
                } 
            }
        }

        if(skillChoices.Count == 0)
            chosenSkill = battle.waitSkill;
        else
            chosenSkill = skillChoices[UnityEngine.Random.Range(0, skillChoices.Count)];

        return chosenSkill;
    }

    // Checks SkillUsageConditions to see if the battler's state permits using the skill.
    private bool CanUseSkill(SkillUsageCondition condition)
    {
        if(condition == SkillUsageCondition.Always)
            return true;

        else if(condition == SkillUsageCondition.HPLower75)
        {
            if((double)this.hp / (double)this.mhp < 0.75)
                return true;
            else
                return false;
        }    

        else if(condition == SkillUsageCondition.HPLower50)
        {
            if((double)this.hp / (double)this.mhp < 0.5)
                return true;
            else
                return false;
        }

        else if(condition == SkillUsageCondition.HPLower25)
        {
            if((double)this.hp / (double)this.mhp < 0.25)
                return true;
            else
                return false;
        }

        else if(condition == SkillUsageCondition.HPBetween25And75)
        {
            if(((double)this.hp / (double)this.mhp >= 0.25) && ((double)this.hp / (double)this.mhp <= 0.75))
                return true;
            else
                return false;
        }

        else if(condition == SkillUsageCondition.HPAbove25)
        {
            if((double)this.hp / (double)this.mhp > 0.25)
                return true;
            else
                return false;
        }

        else if(condition == SkillUsageCondition.HPAbove50)
        {
            if((double)this.hp / (double)this.mhp > 0.50)
                return true;
            else
                return false;
        }

        else if(condition == SkillUsageCondition.HPAbove75)
        {
            if((double)this.hp / (double)this.mhp > 0.75)
                return true;
            else
                return false;
        }

        else if(condition == SkillUsageCondition.IsDebuffed)
        {
            if(GetStrDebuff() < 1.0 
                || GetWilDebuff() < 1.0 
                || GetDefDebuff() < 0.0
                || GetResDebuff() < 0.0
                || GetIniDebuff() < 1.0 
                || GetCrtDebuff() < 0.0
                || GetDebuffResistDebuff() < 0 
                || GetCurseResistDebuff() < 0
                || GetSealResistDebuff() < 0
                || GetStaggerResistDebuff() < 0
            )
                return true;
            else
                return false;
        }
        return false;
    }

    public Dictionary<Item, int> DropItems()
    {
        Dictionary<Item, int> drops = new Dictionary<Item, int>();

        for(int i = 0; i< dropList.Count; i++)
        {
            if(UnityEngine.Random.Range(0, 1.0f) < dropListChances[i])
            {
                if(drops.ContainsKey(dropList[i]))
                    drops.Add(dropList[i], drops[dropList[i]] + dropListQuantities[i]);
                else 
                    drops.Add(dropList[i], dropListQuantities[i]);
            }
                
        }

        return drops;
    }
    
}
