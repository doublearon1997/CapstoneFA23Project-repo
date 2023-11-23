using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattler : Battler
{
    public CharacterClass playerClass;

    public double targetRatio = 1.0;

    public List<Skill> skills;

    public Dictionary<Skill, int> skillCooldownDict = new Dictionary<Skill, int>();

    public OffensiveSkill standardAttack;

    public Character character;

    public bool isKO = false;

    public void LoadStatsFromCharacter(Character c)
    {
        character = c;

        mhp = c.GetCurrMHP();
        hp = c.GetCurrHP();

        str = c.GetCurrStr();
        wil = c.GetCurrWil();
        def = c.GetCurrDef();
        res = c.GetCurrRes();

        ini = c.GetCurrIni();
        crt = c.GetCurrCrt();

        curseResist = c.GetCurrCurseRes();
        sealResist = c.GetCurrSealRes();
        staggerResist = c.GetCurrStaggerRes();

        portrait60 = c.characterImage;
        portrait70 = c.characterImage;

        skills = c.skills;

        battlerName = c.characterName;
        level = c.level;
        playerClass = c.charClass;

        foreach(Skill skill in skills)
            skillCooldownDict.Add(skill, 0);
        
        standardAttack = c.standardAttack;
        skills = c.skills;


    }

}
