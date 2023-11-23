using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect : DurationEffect
{
    public abstract void RemoveStatusEffect(Battler target, BattleSystem battle);

    public StatusEffectType statusEffectType;

    //Must update this when adding a new Status Effect Type!
    public enum StatusEffectType 
    { 
        StaggerEffect, CurseEffect, SealEffect, KOEffect
    }

}
