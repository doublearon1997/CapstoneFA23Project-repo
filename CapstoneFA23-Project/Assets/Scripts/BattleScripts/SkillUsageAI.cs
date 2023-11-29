using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillUsageCondition {Always, HPLower50, HPLower75, HPLower25}

[CreateAssetMenu(fileName = "NewSkillUsageAI", menuName = "SkillUsageAI")]
public class SkillUsageAI : ScriptableObject
{
    public Dictionary<SkillUsageCondition, int> skillUsages;

    public SkillUsageCondition[] usageConditions;
    public int[] weights; 

    public void Initialize()
    {
        skillUsages = new Dictionary<SkillUsageCondition, int>();

        for(int i = 0; i < usageConditions.Length; i++)
            skillUsages.Add(usageConditions[i], weights[i]);
        
    }

    

}
