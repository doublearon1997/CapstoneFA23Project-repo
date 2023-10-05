using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillUsageCondition {Always, HPLower50, HPLower75, HPLower25}

[CreateAssetMenu(fileName = "NewSkillUsageAI", menuName = "SkillUsageAI")]
public class SkillUsageAI : ScriptableObject
{
    public Dictionary<SkillUsageCondition, int> skillUsages;

    public SkillUsageCondition usageCondition;
    public int weight; 

    public void Initialize()
    {
        skillUsages = new Dictionary<SkillUsageCondition, int>();
        skillUsages.Add(usageCondition, weight);
    }

  

}
