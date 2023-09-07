using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This Class defines battlers, which are the actors that fight in battles.
/// </summary>

public class Battler
{
    private int hp { get; set; } //battler's current health points
    private int mhp { get; set; } //battler's max health points

    private int str { get; set; } //battler's str
    private int will { get; set; } //battler's will

    private double def { get; set; } //battler's defense 
    private double res {  get; set; } //battler's resistance    

    private int ini { get; set; } //battler's initiative

    private string name { get; set; }

    public Battler(string name, int mhp, int str, int will, double def, double res, int ini)
    {
        this.name = name;
        this.mhp = mhp;
        this.hp = mhp;
        this.str = str;
        this.will = will;
        this.def = def;
        this.res = res;
        this.ini = ini; 
    }



}
