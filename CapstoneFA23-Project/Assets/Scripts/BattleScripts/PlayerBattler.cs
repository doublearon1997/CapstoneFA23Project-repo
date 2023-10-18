using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattler : Battler
{
    public string playerClass;

    public double targetRatio = 1.0;

    public List<Skill> skills;

    public OffensiveSkill standardAttack;
}
