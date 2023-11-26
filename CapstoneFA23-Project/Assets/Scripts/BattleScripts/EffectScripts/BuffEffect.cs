using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class BuffEffect : DurationEffect
{
    public enum BuffStat 
    { 
        StrBuff, StrDebuff, WilBuff, WilDebuff, DefBuff, DefDebuff, ResBuff, ResDebuff, IniBuff, IniDebuff, CrtBuff, CrtDebuff, 
        DbfRstBuff, DbfRstDebuff, CurRstBuff, CurRstDebuff, SelRstBuff, SelRstDebuff, StaRstBuff, StaRstDebuff
    }

    public readonly int DURATION_UNTIL_DECAYED = -1;

    public BuffStat buffStat;

    public double value = 0, decayValue = 0;

    public abstract void ApplyDecay(Battler target, BattleSystem battle);

    public bool isDebuff;

   
}


