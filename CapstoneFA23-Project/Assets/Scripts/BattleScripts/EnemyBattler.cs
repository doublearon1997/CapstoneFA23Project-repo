using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

public class EnemyBattler : Battler
{
    public Dictionary<Skill, SkillUsageAI> skills = new Dictionary<Skill, SkillUsageAI>();

    public Skill[] skillList;
    public SkillUsageAI[] skillUsageAIList;

    private void Awake()
    {
        for(int i = 0; i<skillList.Length; i++)
        {
            skillUsageAIList[0].Initialize();
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

            foreach(KeyValuePair<SkillUsageCondition, int> conditionEntry in skillEntry.Value.skillUsages)
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
    
}
