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

    private double mhpMod = 1.0, strMod = 1.0, wilMod = 1.0, defMod = 0.0, resMod = 0.0, crtMod = 0.0, iniMod = 1.0;

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

        standardAttack = charClass.standardAttack;
        skills = charClass.startingSkills;

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

}

