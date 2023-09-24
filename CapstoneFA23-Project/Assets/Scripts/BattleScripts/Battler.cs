using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Class defines battlers, which are the actors that fight in battles.
/// </summary>

public class Battler: MonoBehaviour
{
    public int hp; //battler's current health points
    public int mhp; //battler's max health points

    public int str; //battler's str
    public int will; //battler's will

    public double def;  //battler's defense 
    public double res; //battler's resistance    

    public int ini; //battler's initiative

    public string battlerName;

    public int ap;
    public int level;

    public bool isPlayer;


}
