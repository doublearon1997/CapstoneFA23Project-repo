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

    // This method deals damage to a battler and checks if the damage is enough to kill them.
    public void TakeDamage(int damage, BattleSystem battle)
    {
        int currHp = this.hp; 
        currHp = currHp - damage;

        if (currHp < 0)
            currHp = 0;

        this.hp = currHp;

        if (this.hp == 0)
            KillBattler(battle);
    }

    // This method removes a battler from the battle.
    public void KillBattler(BattleSystem battle)
    {
        this.gameObject.SetActive(false);
        if(isPlayer)
            battle.playerBattlers.Remove((PlayerBattler)this);
        else
            battle.enemyBattlers.Remove((EnemyBattler)this);
    }


}


