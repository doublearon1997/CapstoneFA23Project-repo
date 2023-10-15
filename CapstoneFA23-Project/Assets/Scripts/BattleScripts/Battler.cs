using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

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
    public double apMod = 1;

    public int level;

    public bool isPlayer;

    public Sprite portrait70;
    public Sprite portrait60;

    private int partyPosition;


    // This method deals damage to a battler and checks if the damage is enough to kill them.
    public void TakeDamage(int damage, BattleSystem battle)
    {
        int currHp = this.hp; 
        currHp = currHp - damage;

        if (currHp < 0)
            currHp = 0;

        this.hp = currHp;

        if (this.isPlayer)
            battle.SetPlayerHPSliderValue((PlayerBattler)this);
        else
            battle.SetEnemyHPSliderValue((EnemyBattler)this);

        if (this.hp == 0)
            KillBattler(battle);
    }

    // This method removes a battler from the battle.
    public void KillBattler(BattleSystem battle)
    {
        this.gameObject.transform.GetChild(0).GetComponent<Renderer>().enabled = false;
        if (isPlayer)
            battle.playerBattlers.Remove((PlayerBattler)this);
        else
            battle.enemyBattlers.Remove((EnemyBattler)this);

        RemoveOverviewContainer(battle);
    }

    private void RemoveOverviewContainer(BattleSystem battle)
    {
        if(this.isPlayer)
            battle.playerContainers[this.partyPosition].SetActive(false);
        else
            battle.enemyContainers[this.partyPosition].SetActive(false);
    }

    public int GetPartyPosition()
    {
        return partyPosition;
    }

    public void SetPartyPosition(int partyPosition)
    {
        this.partyPosition = partyPosition; 
    }


}


