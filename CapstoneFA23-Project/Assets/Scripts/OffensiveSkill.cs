using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffensiveSkill : Skill
{
    private double dmgMod {  get; set; }    

    public OffensiveSkill(string name, string targetType, double iniMod, double dmgMod) : base(name, targetType, iniMod)
    {
        this.dmgMod = dmgMod;

    }


}
