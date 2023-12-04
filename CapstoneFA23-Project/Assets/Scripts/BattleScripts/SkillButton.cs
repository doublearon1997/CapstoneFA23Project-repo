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

    private bool isLevelUpButton = false;

    public void Initialize(Skill skill, PlayerBattler user, BattleSystem battle)
    {
        this.skill = skill;
        this.user = user;
        this.battle = battle;

        gameObject.transform.GetChild(0).GetComponent<Image>().sprite = skill.portrait100;

        if(skill is SupportItemSkill)
            gameObject.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "" + ((SupportItemSkill)skill).quantity;
        else if(skill is OffensiveItemSkill)
            gameObject.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "" + ((OffensiveItemSkill)skill).quantity;
    }

    public void InitializeLevelupButton(Skill skill, BattleSystem battle)
    {
        this.skill = skill;
        this.battle = battle;
        this.isLevelUpButton = true;

        gameObject.GetComponent<Image>().sprite = skill.portrait100;
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
        Destroy(battle.hotkeyManager.GetComponent<ItemsButtonSelectedHotkeys>());

        battle.SetTemporaryTurnOrderPanel(this.skill);

        if (skill.targetType == TargetType.Single)
            battle.DisplayMessage("Select a target.");
        else if (skill.targetType == TargetType.All)
            battle.DisplayMessage("Select the target group.");
        else if (skill.targetType == TargetType.Self)
            battle.DisplayMessage("Select yourself.");

        if (skill.isOffensive)
            ((OffensiveSkill)skill).SetupChooseTarget(user, battle);
        else
            ((SupportSkill)skill).SetupChooseTarget(user, battle);
    }

    public void LevelupSkillButtonPress()
    {
        if(battle.selectedLevelupSkill == null || skill != battle.selectedLevelupSkill)
        {
            gameObject.transform.parent.GetChild(0).gameObject.SetActive(true);
            battle.selectedLevelupSkill = skill;

            if(gameObject.transform.parent.parent == battle.levelUpPanel.transform.GetChild(21) && battle.levelUpPanel.transform.GetChild(22).childCount > 0)
                battle.levelUpPanel.transform.GetChild(22).GetChild(0).GetChild(0).gameObject.SetActive(false);
            
            else if(gameObject.transform.parent.parent == battle.levelUpPanel.transform.GetChild(22))
                battle.levelUpPanel.transform.GetChild(21).GetChild(0).GetChild(0).gameObject.SetActive(false);
            

            //Popup Continue Button when skill is selected
            battle.levelUpPanel.transform.GetChild(23).gameObject.SetActive(true);

            SEManager.instance.PlaySE("coolBeans");
        }
    }

    // When button is hovered, display a skill stat box for the button's skill.
    public void SkillButtonHover()
    {
        SEManager.instance.PlaySE("buttonHover");

        if(skill.isOffensive)
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
            effectsStatString += effect.GetEffectStatsString() + ", ";
        

        if(effectsStatString.Length > 10)
            effectsStatString = effectsStatString.Substring(0, effectsStatString.Length -2);

        (statBox.transform.GetChild(12).gameObject.GetComponent<TMP_Text>()).text = effectsStatString;

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
            effectsStatString += effect.GetEffectStatsString() + ", ";
        

        if(effectsStatString.Length > 10)
            effectsStatString = effectsStatString.Substring(0, effectsStatString.Length -2);

        (statBox.transform.GetChild(11).gameObject.GetComponent<TMP_Text>()).text = effectsStatString;

    }

    private void RemoveSkillStatBox()
    {
        if(!isLevelUpButton)
            DestroyImmediate(gameObject.transform.GetChild(2).gameObject);
        else 
            DestroyImmediate(gameObject.transform.GetChild(0).gameObject);
    }
}
