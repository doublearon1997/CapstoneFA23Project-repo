using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Class defines Skills, which are actions that Battlers can perform in Battle when it is their turn.
/// </summary>
/// 

public enum TargetType { Single, All, Self }
public enum PowerType { Physical, Will }

public abstract class Skill: ScriptableObject
{
    public string skillName;

    public TargetType targetType;

    public PowerType powerType;

    public double apMod;

    public int cooldown;

    public Sprite portrait65, portrait50, portrait100;

    public bool isOffensive;

    //add effects list/hash
    public List<Effect> effects;

    public void ApplyEffects(Battler user, Battler target, BattleSystem battle)
    {
        foreach(Effect effect in effects)
        {
            effect.ApplyEffect(user, target, battle);
        }
    }
 
}
