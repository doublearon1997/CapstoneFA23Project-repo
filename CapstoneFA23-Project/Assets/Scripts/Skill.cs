using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Class defines Skills, which are actions that Battlers can perform in Battle when it is their turn.
/// </summary>

public class Skill
{
    private string name { get; set; }

    private string targetType { get; set; }

    private double iniMod { get; set; } 

    private bool offensive {get;}

    //add effects list/hash

    public Skill(string name, string targetType, double iniMod, bool isOffensive) 
    {
        this.name = name;
        this.targetType = targetType;
        this.iniMod = iniMod;
        this.offensive = isOffensive;
    }  
}
