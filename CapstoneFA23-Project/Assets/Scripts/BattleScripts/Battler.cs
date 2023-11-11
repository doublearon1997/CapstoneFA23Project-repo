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
    public int wil; //battler's will

    public double def;  //battler's defense 
    public double res; //battler's resistance    

    public int ini; //battler's initiative

    public double crt; //battler's critical chance

    private double strBuff = 1.0, wilBuff = 1.0, defBuff = 0, resBuff = 0, iniBuff = 1.0, crtBuff = 0;
    private double strDebuff = 1.0, wilDebuff = 1.0, defDebuff = 0, resDebuff = 0, iniDebuff = 1.0, crtDebuff = 0;

    private bool fled = false;

    public string battlerName;

    public Dictionary<BuffEffect, int> buffEffects = new Dictionary<BuffEffect, int>(); // effects and their current duration.

    public int ap;
    public double apMod = 1;

    public int level;

    public bool isPlayer;

    public Sprite portrait70;
    public Sprite portrait60;

    private int partyPosition;

    private HashSet<Effect> currentTurnAppliedEffects = new HashSet<Effect>();


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
        {
            battle.enemyBattlers.Remove((EnemyBattler)this);
            battle.StartCoroutine(SEManager.instance.PlaySEOnlyOnce("enemyDeath"));
        }
            
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
    public double GetWilBuff(){return this.wilBuff;}
    public double GetDefBuff(){return this.defBuff;}
    public double GetResBuff(){return this.resBuff;}
    public double GetIniBuff(){return this.iniBuff;}
    public double GetCrtBuff(){return this.crtBuff;}
    public double GetStrDebuff(){return this.strDebuff;}
    public double GetWilDebuff(){return this.wilDebuff;}
    public double GetDefDebuff(){return this.defDebuff;}
    public double GetResDebuff(){return this.resDebuff;}
    public double GetIniDebuff(){return this.iniDebuff;}
    public double GetCrtDebuff(){return this.crtDebuff;}

    //These methods get the corresponding with its current modifier 
    public int GetCurrStr()
    {
        return (int)(this.str * this.strBuff * this.strDebuff);
    }
    public int GetCurrWil()
    {
        return (int)(this.wil * this.wilBuff * this.wilDebuff);
    }
    public double GetCurrDef()
    {
        if (def + defBuff + defDebuff < 0)
            return 0;
        else
            return this.def + this.defBuff + this.defDebuff;
    }
    public double GetCurrRes()
    {
        if (res + resBuff + resDebuff < 0)
            return 0;
        else 
            return this.res + this.resBuff + this.resDebuff;
    }
    public int GetCurrIni()
    {
        return (int)(this.ini * this.iniBuff * this.iniDebuff);
    }
    public double GetCurrCrt()
    {
        return this.crt + this.crtBuff + this.crtDebuff;
    }

    public double GetBuffValue(BuffEffect.BuffStat buffStat)
    {
        double returnValue = -1;

        switch (buffStat)
        {
            case (BuffEffect.BuffStat.StrBuff):
                returnValue = strBuff - 1.0;
                break;
            case (BuffEffect.BuffStat.WilBuff):
                returnValue = wilBuff - 1.0;
                break;
            case (BuffEffect.BuffStat.DefBuff):
                returnValue = defBuff;
                break;
            case (BuffEffect.BuffStat.ResBuff):
                returnValue = resBuff;
                break;
            case (BuffEffect.BuffStat.IniBuff):
                returnValue = iniBuff - 1.0;
                break;
            case (BuffEffect.BuffStat.CrtBuff):
                returnValue = crtBuff;
                break;
            case (BuffEffect.BuffStat.StrDebuff):
                returnValue = strDebuff - 1.0;
                break;
            case (BuffEffect.BuffStat.WilDebuff):
                returnValue = wilDebuff - 1.0;
                break;
            case (BuffEffect.BuffStat.DefDebuff):
                returnValue = defDebuff;
                break;
            case (BuffEffect.BuffStat.ResDebuff):
                returnValue = resDebuff;
                break;
            case (BuffEffect.BuffStat.IniDebuff):
                returnValue = iniDebuff - 1.0;
                break;
            case (BuffEffect.BuffStat.CrtDebuff):
                returnValue = crtDebuff;
                break;
        }
        return returnValue;
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
                this.wilBuff = 1.0 + value;
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
            case (BuffEffect.BuffStat.CrtBuff):
                this.crtBuff = value;
                break;
            case (BuffEffect.BuffStat.StrDebuff):
                this.strDebuff = 1.0 + value;
                break;
            case (BuffEffect.BuffStat.WilDebuff):
                this.wilDebuff = 1.0 + value;
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
            case (BuffEffect.BuffStat.CrtDebuff):
                this.crtDebuff = value;
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
                {
                    this.strBuff = 1.0;
                    return true;
                }
                break;
            case (BuffEffect.BuffStat.WilBuff):
                this.wilBuff += value;
                if(this.wilBuff <= 1.00)
                {
                    this.wilBuff = 1.0;
                    return true;
                }
                break;
            case (BuffEffect.BuffStat.DefBuff):
                this.defBuff += value;
                if(this.defBuff <= 0)
                {
                    this.defBuff = 0.0;
                    return true;
                }
                break;
            case (BuffEffect.BuffStat.ResBuff):
                this.resBuff += value;
                if(this.resBuff <= 0)
                {
                    this.resBuff = 0.0;
                    return true;
                }
                break;
            case (BuffEffect.BuffStat.IniBuff):
                this.iniBuff += value;
                if(this.iniBuff <= 1.00)
                {
                    this.iniBuff = 1.0;
                    return true;
                }
                break;
            case (BuffEffect.BuffStat.CrtBuff):
                this.crtBuff += value;
                if(this.crtBuff <= 0.0)
                {
                    this.crtBuff = 0.0;
                    return true;
                }
                break;
            case (BuffEffect.BuffStat.StrDebuff):
                this.strDebuff += value;
                if(this.strDebuff >= 1.00)
                {
                    this.strDebuff = 1.0;
                    return true;
                }
                break;
            case (BuffEffect.BuffStat.WilDebuff):
                this.wilDebuff += value;
                if(this.wilDebuff >= 1.00)
                {
                    this.wilDebuff = 1.0;
                    return true;
                }
                break;
            case (BuffEffect.BuffStat.DefDebuff):
                this.defDebuff += value;
                if(this.defDebuff >= 0)
                {
                    this.defDebuff = 0.0;
                    return true;
                }
                break;
            case (BuffEffect.BuffStat.ResDebuff):
                this.resDebuff += value;
                if(this.resDebuff >= 0)
                {
                    this.resDebuff = 0.0;
                    return true;
                }
                break;
            case (BuffEffect.BuffStat.IniDebuff):
                this.iniDebuff += value;
                if(this.iniDebuff >= 1.00)
                {
                    this.iniDebuff = 1.0;
                    return true;
                }
                break;  
            case (BuffEffect.BuffStat.CrtDebuff):
                this.crtDebuff += value;
                if(this.crtDebuff <= 0.0)
                {
                    this.crtDebuff = 0.0;
                    return true;
                }
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
            if(!currentTurnAppliedEffects.Contains(effect))
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
            else
            {
                currentTurnAppliedEffects.Remove(effect);
            }
        }

        foreach(BuffEffect effect in removeEffects)
            this.buffEffects.Remove(effect);
    }

    public void Flee(BattleSystem battle)
    {
        this.fled = true;
        Destroy(this.gameObject);
        RemoveOverviewContainer(battle);
    }

    public bool HasFled()
    {
        return fled;
    }

    public void SetHasFled(bool hasFled)
    {
        this.fled = hasFled;
    }

    public void AddCurrentTurnEffect(Effect effect)
    {
        this.currentTurnAppliedEffects.Add(effect);
    }

}


