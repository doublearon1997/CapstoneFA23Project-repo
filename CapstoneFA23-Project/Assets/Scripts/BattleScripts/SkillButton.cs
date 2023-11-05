using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SkillButton : MonoBehaviour
{
    private Skill skill;
    private PlayerBattler user;
    private BattleSystem battle;

    public void Initialize(Skill skill, PlayerBattler user, BattleSystem battle)
    {
        this.skill = skill;
        this.user = user;
        this.battle = battle;

        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = skill.portrait100;
    }

    // When button is pressed, go into choose target mode for the button's skill.
    public void SkillButtonPress()
    {
        SEManager.instance.PlaySE("buttonClick", 1);

        if(battle.skillSelected)
            battle.SkillTargetReturn();
        battle.skillSelected = true;

        battle.hotkeyManager.AddComponent<SkillTargetHotkeys>().Initialize(battle);
        Destroy(battle.hotkeyManager.GetComponent<SkillsButtonSelectedHotkeys>());
        Destroy(battle.hotkeyManager.GetComponent<TacticsButtonSelectedHotkeys>());

        battle.SetTemporaryTurnOrderPanel(this.skill);

        if (skill.targetType == TargetType.Single)
            battle.DisplayMessage("Select a target.");
        else if (skill.targetType == TargetType.All)
            battle.DisplayMessage("Select the target group.");
        else if (skill.targetType == TargetType.Self)
            battle.DisplayMessage("Select yourself.");

        if (skill.isOffensive)
            ((OffensiveSkill)skill).ChooseTarget(user, battle);
        else
            ((SupportSkill)skill).ChooseTarget(user, battle);
    }

    // When button is hovered, display a skill stat box for the button's skill.
    public void SkillButtonHover()
    {
        SEManager.instance.PlaySE("buttonHover", 1);

        if (skill.isOffensive)
            CreateOffensiveSkillStatBox();
        else
            CreateSupportSkillStatBox();
    }

    public void SkillButtonHoverExit()
    {
        RemoveSkillStatBox();
    }

    // Instantiates a skillstatbox on this button, filling in the appropriate stats for the skill
    private void CreateOffensiveSkillStatBox()
    {
        GameObject statBox = Instantiate(battle.panelOffensiveSkillStatBox, gameObject.transform);

        statBox.GetComponent<RectTransform>().anchoredPosition = new Vector2(225, 150);

        (statBox.transform.GetChild(2).gameObject.GetComponent<TMP_Text>()).text = skill.skillName;
        (statBox.transform.GetChild(3).gameObject.GetComponent<Image>()).sprite = skill.portrait50;

        (statBox.transform.GetChild(5).gameObject.GetComponent<TMP_Text>()).text = "Offensive";
        (statBox.transform.GetChild(6).gameObject.GetComponent<TMP_Text>()).text = skill.powerType.ToString();
        (statBox.transform.GetChild(7).gameObject.GetComponent<TMP_Text>()).text = skill.targetType.ToString();
        (statBox.transform.GetChild(8).gameObject.GetComponent<TMP_Text>()).text = "x" + ((OffensiveSkill)skill).dmgMod;
        (statBox.transform.GetChild(9).gameObject.GetComponent<TMP_Text>()).text = "x" + skill.apMod;

        if(skill.cooldown == 0)
            (statBox.transform.GetChild(10).gameObject.GetComponent<TMP_Text>()).text = "None";
        else if(skill.cooldown == 1)
            (statBox.transform.GetChild(10).gameObject.GetComponent<TMP_Text>()).text = "" + skill.cooldown + " turn";
        else
            (statBox.transform.GetChild(10).gameObject.GetComponent<TMP_Text>()).text = "" + skill.cooldown + " turns";

        string effectsStatString = "Effects: ";
        foreach (Effect effect in skill.effects)
        {
            effectsStatString += effect.GetEffectStatsString() + ", ";
        }

        (statBox.transform.GetChild(12).gameObject.GetComponent<TMP_Text>()).text = effectsStatString.Substring(0, effectsStatString.Length - 2);

    }

    private void CreateSupportSkillStatBox()
    {
        GameObject statBox = Instantiate(battle.panelSupportSkillStatBox, gameObject.transform);

        statBox.GetComponent<RectTransform>().anchoredPosition = new Vector2(225, 150);

        (statBox.transform.GetChild(2).gameObject.GetComponent<TMP_Text>()).text = skill.skillName;
        (statBox.transform.GetChild(3).gameObject.GetComponent<Image>()).sprite = skill.portrait50;

        (statBox.transform.GetChild(5).gameObject.GetComponent<TMP_Text>()).text = "Support";
        (statBox.transform.GetChild(6).gameObject.GetComponent<TMP_Text>()).text = skill.powerType.ToString();
        (statBox.transform.GetChild(7).gameObject.GetComponent<TMP_Text>()).text = skill.targetType.ToString();
        (statBox.transform.GetChild(8).gameObject.GetComponent<TMP_Text>()).text = "x" + skill.apMod;

        if (skill.cooldown == 0)
            (statBox.transform.GetChild(9).gameObject.GetComponent<TMP_Text>()).text = "None";
        else if (skill.cooldown == 1)
            (statBox.transform.GetChild(9).gameObject.GetComponent<TMP_Text>()).text = "" + skill.cooldown + " turn";
        else
            (statBox.transform.GetChild(9).gameObject.GetComponent<TMP_Text>()).text = "" + skill.cooldown + " turns";


        string effectsStatString = "Effects: ";
        foreach(Effect effect in skill.effects)
        {
            effectsStatString += effect.GetEffectStatsString() + ", ";
        }

        (statBox.transform.GetChild(11).gameObject.GetComponent<TMP_Text>()).text = effectsStatString.Substring(0, effectsStatString.Length-2);

    }

    private void RemoveSkillStatBox()
    {
        DestroyImmediate(gameObject.transform.GetChild(1).gameObject);
    }
}
