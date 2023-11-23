using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

public class EnemyBattler : Battler
{
    public Dictionary<Skill, SkillUsageAI> skills = new Dictionary<Skill, SkillUsageAI>();

    public Skill[] skillList;
    public SkillUsageAI[] skillUsageAIList;

    public List<Item> dropList = null;
    public List<double> dropListChances =  null;

    public List<int> dropListQuantities = null;

    public int exp;

    private void Awake()
    {
        for(int i = 0; i<skillList.Length; i++)
        {
            skillUsageAIList[i].Initialize();
            skills.Add(skillList[i], skillUsageAIList[i]);
        }
    }

    public Skill ChooseSkill()
    {
        Skill chosenSkill;
        List<Skill> skillChoices = new List<Skill>();

        foreach(KeyValuePair<Skill, SkillUsageAI> skillEntry in skills)
        {
            int maxWeight = 0;
            bool canUse = false;

            foreach (KeyValuePair<SkillUsageCondition, int> conditionEntry in skillEntry.Value.skillUsages)
            {
                if(canUseSkill(conditionEntry.Key))
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
        
        chosenSkill = skillChoices[UnityEngine.Random.Range(0, skillChoices.Count)];

        return chosenSkill;
    }

    // Checks SkillUsageConditions to see if the battler's state permits using the skill.
    private bool canUseSkill(SkillUsageCondition condition)
    {
        if(condition == SkillUsageCondition.Always)
            return true;

        else if(condition == SkillUsageCondition.HPLower50)
        {
            if((double)this.hp / (double)this.mhp < 0.5)
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
