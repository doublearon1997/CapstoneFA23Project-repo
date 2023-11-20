using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : ScriptableObject
{
    public string effectName;

    public double chance = 1.0;

    public abstract string GetEffectStatsString();

    public abstract bool ApplyEffect(Battler user, Battler target, Skill skill, BattleSystem battle);



}
