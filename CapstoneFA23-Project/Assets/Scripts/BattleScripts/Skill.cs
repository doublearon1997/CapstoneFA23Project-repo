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

    public List<Effect> effects;

    //Applies all of the effects on the skill to the target battler. Returns a list of flags to determine if a Buff or Debuff animation should be displayed.
    public bool[] ApplyEffects(Battler user, Battler target, BattleSystem battle)
    {
        bool[] displayFlags = {false, false};

        foreach(Effect effect in effects)
        {
            if(effect.ApplyEffect(user, target, battle))
            {
                if(effect is Buff)
                    displayFlags[0] = true;
                else if(effect is Debuff)
                    displayFlags[1] = true;
            }
        }
        return displayFlags;
    }
 
}
