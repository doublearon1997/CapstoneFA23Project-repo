using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Class defines Skills, which are actions that Battlers can perform in Battle when it is their turn.
/// </summary>
/// 

public enum TargetType { Single, All }
public enum PowerType { Physical, Will }

public class Skill: ScriptableObject
{
    public string skillName;

    public TargetType targetType;

    public PowerType powerType;

    public double apMod;

    public bool offensive;

    //add effects list/hash
 
}
