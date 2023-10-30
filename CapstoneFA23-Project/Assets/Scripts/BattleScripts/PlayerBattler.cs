using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattler : Battler
{
    public string playerClass;

    public double targetRatio = 1.0;

    public List<Skill> skills;

    public Dictionary<Skill, int> skillCooldownDict = new Dictionary<Skill, int>();

    public OffensiveSkill standardAttack;

    public void Initialize()
    {
        foreach(Skill skill in skills)
        {
            skillCooldownDict.Add(skill, 0);
        }
    }
}
