using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class Character : ScriptableObject
{
    public int characterID;
    public Sprite characterImage;
    public string characterName, description;

    public CharacterClass charClass = null;

    public int level;
    public int exp;

    //Stats
    public int hp, mhp;
    public int str, wil, ini;
    public double def, res, crt;

    public double curseRes, sealRes, staggerRes;

    private double mhpMod = 1.0, strMod = 1.0, wilMod = 1.0, defMod = 0.0, resMod = 0.0, crtMod = 0.0, iniMod = 1.0;
    private double curseResMod = 0.0, sealResMod = 0.0, staggerResMod = 0.0;

    //Stores data about the battler's stats pre-adding exp, mainly for animations. 
    public int preBattleExp = 0;
    public int preBattleLevel = 0;

    //Skills
    public List<Skill> skills;
    public OffensiveSkill standardAttack;

    public void InitializeCharacter()
    {
        mhp = (int)(charClass.sMhp * Math.Pow(1.1, level-1));
        hp = mhp;
        str = (int)(charClass.sStr * Math.Pow(1.1, level-1));
        wil = (int)(charClass.sWil * Math.Pow(1.1, level-1));
        ini = (int)(charClass.sIni * Math.Pow(1.1, level-1));
        def = charClass.sDef;
        res = charClass.sRes;
        crt = charClass.sCrt;

        curseRes = charClass.sCurseRes;
        sealRes = charClass.sSealRes;
        staggerRes = charClass.sStaggerRes;

        standardAttack = charClass.standardAttack;

        foreach(Skill skill in charClass.startingSkills)
        {
            if(!skills.Contains(skill))
                skills.Add(skill);
        }   
    }

    public int GetCurrMHP()
    {
        return (int)(mhp * mhpMod);
    }

    public int GetCurrHP()
    {
        return hp;
    }

    public int GetCurrStr()
    {
        return (int)(str * strMod);
    }

    public int GetCurrWil()
    {
        return (int)(wil * wilMod);
    }

    public double GetCurrDef()
    {
        if (def + defMod < 0)
            return 0;
        else 
            return def + defMod;
    }

    public double GetCurrRes()
    {
        if (res + resMod < 0)
            return 0;
        else
            return res + resMod;
    }

    public int GetCurrIni()
    {
        return (int)(ini * iniMod);
    }

    public int GetCurrCrt()
    {
        return (int)(crt * crtMod);
    }

    public double GetCurrCurseRes()
    {
        if (curseRes + curseResMod < 0)
            return 0;
        else
            return curseRes + curseResMod;
    }

    public double GetCurrSealRes()
    {
        if (sealRes + sealResMod < 0)
            return 0;
        else
            return sealRes + sealResMod;
    }

    public double GetCurrStaggerRes()
    {
        if (staggerRes + staggerResMod < 0)
            return 0;
        else
            return staggerRes + staggerResMod;
    }

    public void LevelUp()
    {
        level += 1;

        mhp = (int)(charClass.sMhp * Math.Pow(1.1, level-1));
        hp = mhp;
        str = (int)(charClass.sStr * Math.Pow(1.1, level-1));
        wil = (int)(charClass.sWil * Math.Pow(1.1, level-1));
        ini = (int)(charClass.sIni * Math.Pow(1.1, level-1));
        crt = charClass.sCrt + 0.01;
    }

    public List<Skill> GetNextUnlockableSkills()
    {
        List<Skill> availSkills = new List<Skill>();
        int numSkills = 0;

        foreach(Skill s in charClass.skillUnlocks)
        {
            if(!skills.Contains(s))
            {
                availSkills.Add(s);
                numSkills++;
            }

            if(numSkills >= 2)
                break;
        }

        return availSkills;

    }


}

