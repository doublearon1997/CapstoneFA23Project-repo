using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Class", menuName = "Character Class")]
public class CharacterClass : ScriptableObject
{
    public string className;
    public int sMhp, sStr, sWil, sIni;
    public double sDef, sRes, sCrt;

    public double sCurseRes, sSealRes, sStaggerRes;

    public OffensiveSkill standardAttack;

    public List<Skill> startingSkills;

    public List<Skill> skillUnlocks;
}
