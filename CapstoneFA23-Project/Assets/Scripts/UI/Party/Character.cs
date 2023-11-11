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

    /*public void Awake()
    {
        mhp = (int)(charClass.sMhp * Math.Pow(1.1, level-1));
        hp = mhp;
        str = charClass.sStr;
        wil = charClass.sWil;
        ini = charClass.sIni;
        def = charClass.sDef;
        res = charClass.sRes;
        crt = charClass.sCrt;
    }*/

}

