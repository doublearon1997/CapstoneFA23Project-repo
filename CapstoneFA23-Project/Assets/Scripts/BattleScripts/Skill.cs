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

    public AudioClip soundEffect;
    public AudioClip hitSoundEffect;
    public float soundEffectHitDelay;

    //Applies all of the effects on the skill to the target battler. Returns a list of flags to determine if a Buff or Debuff animation should be displayed.
    public bool[] ApplyEffects(Battler user, Battler target, BattleSystem battle)
    {
        bool[] displayFlags = {false, false, false, false, false, false};

        foreach(Effect effect in effects)
        {
            if(effect.ApplyEffect(user, target, this, battle))
            {
                if(effect is Buff)
                    displayFlags[0] = true;
                else if(effect is Debuff)
                    displayFlags[1] = true;
                else if(effect is ClearCooldownsEffect)
                    displayFlags[2] = true;
                else if(effect is CurseEffect)
                    displayFlags[3] = true;
                else if(effect is SealEffect)
                    displayFlags[4] = true;
                else if(effect is StaggerEffect)
                    displayFlags[5] = true;
            }
        }
        return displayFlags;
    }

    public void FillNotificationQueues(List<GameObject> effectNotificationQueue, List<string> effectSoundEffectQueue, bool[] displayFlags, BattleSystem battle)
    {
        if(displayFlags[0])
        {
            effectNotificationQueue.Add(battle.buffPopup);
            effectSoundEffectQueue.Add("buff");
        }
        if(displayFlags[1])
        {
            effectNotificationQueue.Add(battle.debuffPopup);
            effectSoundEffectQueue.Add("debuff");
        }
        if(displayFlags[2])
        {
            effectNotificationQueue.Add(battle.cooldownClearPopup);
            effectSoundEffectQueue.Add("powerUp_1");
        }
        if(displayFlags[3])
        {
            effectNotificationQueue.Add(battle.cursePopup);
            effectSoundEffectQueue.Add("stagger");
        }
        if(displayFlags[4])
        {
            effectNotificationQueue.Add(battle.sealPopup);
            effectSoundEffectQueue.Add("seal");
        }
        if(displayFlags[5])
        {
            effectNotificationQueue.Add(battle.staggerPopup);
            effectSoundEffectQueue.Add("stagger");
        }
    }
 
}

