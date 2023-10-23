using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : ScriptableObject
{
    public string effectName;

    public abstract string GetEffectStatsString();

    public abstract void ApplyEffect(Battler user, Battler target, BattleSystem battle);



}
