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

    private double strBuff = 1.0, willBuff = 1.0, defBuff = 0, resBuff = 0, iniBuff = 1.0;
    private double strDebuff = 1.0, willDebuff = 1.0, defDebuff = 0, resDebuff = 0, iniDebuff = 1.0;

    public string battlerName;

    public Dictionary<BuffEffect, int> buffEffects = new Dictionary<BuffEffect, int>(); // effects and their current duration.

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

    public void HealBattler(int hpHealed, BattleSystem battle)
    {
        int currHp = this.hp;
        currHp = currHp+hpHealed;

        if(currHp > this.mhp)
            currHp = this.mhp;

        this.hp = currHp;

        if (this.isPlayer)
            battle.SetPlayerHPSliderValue((PlayerBattler)this);
        else
            battle.SetEnemyHPSliderValue((EnemyBattler)this);
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

    public double GetStrBuff(){return this.strBuff;}
    public double GetWillBuff(){return this.willBuff;}
    public double GetDefBuff(){return this.defBuff;}
    public double GetResBuff(){return this.resBuff;}
    public double GetIniBuff(){return this.iniBuff;}
    public double GetStrDebuff(){return this.strDebuff;}
    public double GetWillDebuff(){return this.willDebuff;}
    public double GetDefDebuff(){return this.defDebuff;}
    public double GetResDebuff(){return this.resDebuff;}
    public double GetIniDebuff(){return this.iniDebuff;}

    //These methods get the corresponding with its current modifier 
    public int GetCurrStr()
    {
        return (int)(this.str * this.strBuff * this.strDebuff);
    }
    public int GetCurrWill()
    {
        return (int)(this.will * this.willBuff * this.willDebuff);
    }
    public double GetCurrDef()
    {
        return this.def + this.defBuff + this.defDebuff;
    }
    public double GetCurrRes()
    {
        return this.res + this.resBuff + this.resDebuff;
    }
    public int GetCurrIni()
    {
        return (int)(this.ini * this.iniBuff * this.iniDebuff);
    }

    // Converts a buffStat to a buff attribute of this battler's, then changes the value.
    public void SetBuffStat(BuffEffect.BuffStat buffStat, double value)
    {
        switch(buffStat) 
        {
            case (BuffEffect.BuffStat.StrBuff):
                this.strBuff = 1.0 + value;
                break;
            case (BuffEffect.BuffStat.WilBuff):
                this.willBuff = 1.0 + value;
                break;
            case (BuffEffect.BuffStat.DefBuff):
                this.defBuff = value;
                break;
            case (BuffEffect.BuffStat.ResBuff):
                this.resBuff = value;
                break;
            case (BuffEffect.BuffStat.IniBuff):
                this.iniBuff = 1.0 + value;
                break;
            case (BuffEffect.BuffStat.StrDebuff):
                this.strDebuff = 1.0 + value;
                break;
            case (BuffEffect.BuffStat.WilDebuff):
                this.willDebuff = 1.0 + value;
                break;
            case (BuffEffect.BuffStat.DefDebuff):
                this.defDebuff = value;
                break;
            case (BuffEffect.BuffStat.ResDebuff):
                this.resDebuff = value;
                break;
            case (BuffEffect.BuffStat.IniDebuff):
                this.iniDebuff = 1.0 + value;
                break;    
        }
    }

    // Converts a buffStat to a buff attribute of this battler's, then decays the value. Returns true if value is fully decayed.
    public bool DecayBuffStat(BuffEffect.BuffStat buffStat, double value)
    {
        switch(buffStat) 
        {
            case (BuffEffect.BuffStat.StrBuff):
                this.strBuff += value;
                if(this.strBuff <= 1.00)
                    return true;
                break;
            case (BuffEffect.BuffStat.WilBuff):
                this.willBuff += value;
                if(this.willBuff <= 1.00)
                    return true;
                break;
            case (BuffEffect.BuffStat.DefBuff):
                this.defBuff += value;
                if(this.defBuff <= 0)
                    return true;
                break;
            case (BuffEffect.BuffStat.ResBuff):
                this.resBuff += value;
                if(this.resBuff <= 0)
                    return true;
                break;
            case (BuffEffect.BuffStat.IniBuff):
                this.iniBuff += value;
                if(this.iniBuff <= 1.00)
                    return true;
                break;
            case (BuffEffect.BuffStat.StrDebuff):
                this.strDebuff += value;
                if(this.strDebuff >= 1.00)
                    return true;
                break;
            case (BuffEffect.BuffStat.WilDebuff):
                this.willDebuff += value;
                if(this.willDebuff >= 1.00)
                    return true;
                break;
            case (BuffEffect.BuffStat.DefDebuff):
                this.defDebuff += value;
                if(this.defDebuff >= 0)
                    return true;
                break;
            case (BuffEffect.BuffStat.ResDebuff):
                this.resDebuff += value;
                if(this.resDebuff >= 0)
                    return true;
                break;
            case (BuffEffect.BuffStat.IniDebuff):
                this.iniDebuff += value;
                if(this.iniDebuff >= 1.00)
                    return true;
                break;    
        }
        return false;
    }

    // Counts down the durations and decays the values of effects.
    public void CountDownEffects()
    {
        List<BuffEffect> effectKeys = new List<BuffEffect>(this.buffEffects.Keys);
        List<BuffEffect> removeEffects = new List<BuffEffect>();

        foreach(BuffEffect effect in effectKeys)
        {
            buffEffects[effect] -= 1;

            if(buffEffects[effect] == 0)
                removeEffects.Add(effect);

            if(effect.decayValue != 0)
            {
                if(DecayBuffStat(effect.buffStat, effect.decayValue))
                    removeEffects.Add(effect);
            }
        }

        foreach(BuffEffect effect in removeEffects)
            this.buffEffects.Remove(effect);
    }
}


