using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Class defines Skills, which are actions that Battlers can perform in Battle when it is their turn.
/// </summary>
/// 

public enum TargetType { Single, All }
public enum PowerType { Physical, Will }

public abstract class Skill: ScriptableObject
{
    public string skillName;

    public TargetType targetType;

    public PowerType powerType;

    public double apMod;

    public int cooldown;

    public bool offensive;

    public Sprite portrait65, portrait50, portrait100;

    public bool isOffensive;

    //add effects list/hash
 
}
