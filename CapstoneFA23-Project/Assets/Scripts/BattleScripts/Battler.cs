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

    public double debuffResist, curseResist, sealResist, staggerResist;

    private double strBuff = 1.0, wilBuff = 1.0, defBuff = 0, resBuff = 0, iniBuff = 1.0, crtBuff = 0;
    private double strDebuff = 1.0, wilDebuff = 1.0, defDebuff = 0, resDebuff = 0, iniDebuff = 1.0, crtDebuff = 0;

    private double debuffResistBuff = 0, curseResistBuff = 0, sealResistBuff = 0, staggerResistBuff = 0;
    private double debuffResistDebuff = 0, curseResistDebuff = 0, sealResistDebuff = 0, staggerResistDebuff = 0;

    private bool fled = false;

    public string battlerName;

    public Sprite idleSprite, attackSprite;

    public Dictionary<BuffEffect, int> buffEffects = new Dictionary<BuffEffect, int>(); // effects and their current duration.
    public Dictionary<StatusEffect, int> statusEffects = new Dictionary<StatusEffect, int>(); 

    public int ap;
    public double apMod = 1;

    public int level;

    public bool isPlayer;

    public Sprite portrait70;
    public Sprite portrait60;

    private int partyPosition;

    private HashSet<Effect> currentTurnAppliedEffects = new HashSet<Effect>();

    // Battler Status
    public bool physicalEnabled = true, willEnabled = true, nextHitCrit = false;

    public List<Skill> skills;

    // This method deals damage to a battler and checks if the damage is enough to kill them.
    public void TakeDamage(int damage, BattleSystem battle)
    {
        int currHp = this.hp; 
        currHp = currHp - damage;

        if (currHp < 0)
            currHp = 0;

        this.hp = currHp;

        if (this.isPlayer)
        {
            battle.SetPlayerHPSliderValue((PlayerBattler)this);
            if(this.hp == 0)
            {
                this.TryApplyStatusEffect(battle.kOEffect, battle);
                this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = ((PlayerBattler)this).kOSprite;
            }
                
        }  
        else
        {
            battle.SetEnemyHPSliderValue((EnemyBattler)this);
            if (this.hp == 0)
                KillBattler(battle);

        }
            
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
        if(!this.isPlayer || (this.isPlayer && !((PlayerBattler)this).isKO))
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
    public double GetDebuffResistBuff(){return this.debuffResistBuff;}
    public double GetCurseResistBuff(){return this.curseResistBuff;}
    public double GetSealResistBuff(){return this.sealResistBuff;}
    public double GetStaggerResistBuff(){return this.staggerResistBuff;}

    public double GetStrDebuff(){return this.strDebuff;}
    public double GetWilDebuff(){return this.wilDebuff;}
    public double GetDefDebuff(){return this.defDebuff;}
    public double GetResDebuff(){return this.resDebuff;}
    public double GetIniDebuff(){return this.iniDebuff;}
    public double GetCrtDebuff(){return this.crtDebuff;}
    public double GetDebuffResistDebuff(){return this.debuffResistDebuff;}
    public double GetCurseResistDebuff(){return this.curseResistDebuff;}
    public double GetSealResistDebuff(){return this.sealResistDebuff;}
    public double GetStaggerResistDebuff(){return this.staggerResistDebuff;}

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
    public double GetCurrDebuffResistance()
    {
        if (debuffResist + debuffResistBuff + debuffResistDebuff < 0)
            return 0;
        return debuffResist + debuffResistBuff + debuffResistDebuff;
    }
    public double GetCurrCurseResistance()
    {
        if (curseResist + curseResistBuff + curseResistDebuff < 0)
            return 0;
        return curseResist + curseResistBuff + curseResistDebuff;
    }
    public double GetCurrSealResistance()
    {
        if (sealResist + sealResistBuff + sealResistDebuff < 0)
            return 0;
        return sealResist + sealResistBuff + sealResistDebuff;
    }
    public double GetCurrStaggerResistance()
    {
        if (staggerResist + staggerResistBuff + staggerResistDebuff < 0)
            return 0;
        return staggerResist + staggerResistBuff + staggerResistDebuff;
    }

    //Gets a battler's stat value given the buffstat type.
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
            case (BuffEffect.BuffStat.DbfRstBuff):
                returnValue = debuffResistBuff;
                break;
            case (BuffEffect.BuffStat.CurRstBuff):
                returnValue = curseResistBuff;
                break;
            case (BuffEffect.BuffStat.SelRstBuff):
                returnValue = sealResistBuff;
                break;
            case (BuffEffect.BuffStat.StaRstBuff):
                returnValue = staggerResistBuff;
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
            case (BuffEffect.BuffStat.DbfRstDebuff):
                returnValue = debuffResistDebuff;
                break;
            case (BuffEffect.BuffStat.CurRstDebuff):
                returnValue = curseResistDebuff;
                break;
            case (BuffEffect.BuffStat.SelRstDebuff):
                returnValue = sealResistDebuff;
                break;
            case (BuffEffect.BuffStat.StaRstDebuff):
                returnValue = staggerResistDebuff;
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
    public void CountDownEffects(BattleSystem battle)
    {
        CountDownBuffEffects();
        CountDownStatusEffects(battle);  
    }

    private void CountDownBuffEffects()
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

    private void CountDownStatusEffects(BattleSystem battle)
    {
        List<StatusEffect> effectKeys = new List<StatusEffect>(this.statusEffects.Keys);
        List<StatusEffect> removeEffects = new List<StatusEffect>();

        foreach(StatusEffect effect in effectKeys)
        {
            if(!currentTurnAppliedEffects.Contains(effect))
            {
                statusEffects[effect] -= 1;

                if(statusEffects[effect] == 0)
                    removeEffects.Add(effect);
            }
            else
                currentTurnAppliedEffects.Remove(effect);
        }

        foreach(StatusEffect effect in removeEffects)
        {
            this.statusEffects.Remove(effect);
            effect.RemoveStatusEffect(this, battle);
        }
            


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

    // Applies a status effect to the battler. If the battler already has the same status effect, it keeps the one that will last the longest to keep. Returns if the effect was applied or not.
    public bool TryApplyStatusEffect(StatusEffect effect)
    {
        bool applied = true;
        StatusEffect removeEffect = null;

        foreach(StatusEffect currentEffect in statusEffects.Keys)
        {
            if(currentEffect.statusEffectType == effect.statusEffectType)
            {
                if(statusEffects[currentEffect] >= effect.duration)
                {
                    applied = false;
                    break;
                }
                else 
                    removeEffect = currentEffect;
                
            }
        }

        if(removeEffect != null)
            statusEffects.Remove(removeEffect);

        if(applied)
            statusEffects.Add(effect, effect.duration);
            
        return applied;
    }

    // Applies a buff effect to the battler. If the battler already has the same status effect, it keeps the one with the highest value. Returns if the effect was applied or not.
    public bool TryApplyBuffEffect(BuffEffect effect, BattleSystem battle)
    {
        bool applied = true;
        BuffEffect removeEffect = null;

        foreach(BuffEffect currentEffect in buffEffects.Keys)
        {
            if(currentEffect.buffStat == effect.buffStat)
            {
                if(effect.isDebuff && GetBuffValue(currentEffect.buffStat) < effect.value)
                {
                    applied = false;
                    break;
                }
                else if(!effect.isDebuff && GetBuffValue(currentEffect.buffStat) > effect.value)
                {
                    applied = false;
                    break;
                }
                else 
                    removeEffect = currentEffect;
            }
        }

        if(removeEffect != null)
            buffEffects.Remove(removeEffect);

        if(applied)
            buffEffects.Add(effect, effect.duration);
            
        return applied;
    }

    //Applies a status effect to the battler. This method is for adding effects that are not connected to a skill, instead use Skill.ApplyEffects() for that. 
    //Any effects that must have a user or skill attached cannot be used.
    public bool TryApplyStatusEffect(StatusEffect effect, BattleSystem battle)
    {
        bool applied = true;
        StatusEffect removeEffect = null;

        foreach(StatusEffect currentEffect in statusEffects.Keys)
        {
            if(currentEffect.statusEffectType == effect.statusEffectType)
            {
                if(statusEffects[currentEffect] >= effect.duration)
                {
                    applied = false;
                    break;
                }
                else 
                    removeEffect = currentEffect;
                
            }
        }

        if(removeEffect != null)
            statusEffects.Remove(removeEffect);

        if(applied)
        {
            statusEffects.Add(effect, effect.duration);
            effect.ApplyEffect(null, this, null, battle);
        }

        effect.ApplyEffect(null, this, null, battle);

        return applied;
    }

    public bool TryRemoveStatusEffectType(StatusEffect.StatusEffectType type, BattleSystem battle)
    {
        bool removed = false;
        StatusEffect removeEffect = null;

        foreach(StatusEffect effect in statusEffects.Keys)
        {
            if(effect.statusEffectType == type)
            {
                removeEffect = effect;
                break;
            }
        }

        if(removeEffect != null)
        {
            statusEffects.Remove(removeEffect);
            removeEffect.RemoveStatusEffect(this, battle);
            removed = true;
        }

        return removed;
            

    }

}


