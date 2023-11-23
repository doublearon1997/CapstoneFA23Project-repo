using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattlerStatHover : MonoBehaviour
{
    public GameObject statBox = null;

    private Battler battler;
    private BattleSystem battle;
    private GameObject infoPanel;

    private GameObject highlightAnimation;

    public void Initialize(Battler battler, BattleSystem battle)
    {
        this.battler = battler;
        this.battle = battle;
    }

    public void InfoObjectHover()
    {
        SEManager.instance.PlaySE("buttonHover");
        infoPanel = Instantiate(statBox, battler.gameObject.transform) as GameObject;

        SetupInfoObjectHover();
        if(battler == battle.currentlyActingBattler && !battle.tempTurnOrder)
            highlightAnimation = Instantiate(battle.turnOrderCurrentHighlightAnimation, battle.battlerTurnOrderObjects[battler].transform) as GameObject;
        else if(battler.isPlayer && ((PlayerBattler)battler).isKO)
        {

        }
        else
            highlightAnimation = Instantiate(battle.turnOrderHighlightAnimation, battle.battlerTurnOrderObjects[battler].transform) as GameObject;
    }

    public void InfoObjectHoverExit()
    {
        DestroyImmediate(infoPanel);
        DestroyImmediate(highlightAnimation);
    }

    private void SetupInfoObjectHover()
    {
        if (battler.isPlayer)
        {
            infoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(280, -115);
        }
            
        else
            infoPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-170, -115);

        (infoPanel.transform.GetChild(2).gameObject.GetComponent<TMP_Text>()).text = battler.battlerName;
        (infoPanel.transform.GetChild(3).gameObject.GetComponent<Image>()).sprite = battler.portrait60;

        (infoPanel.transform.GetChild(5).gameObject.GetComponent<TMP_Text>()).text = "" + battler.hp + "/" + battler.mhp;

        (infoPanel.transform.GetChild(6).gameObject.GetComponent<TMP_Text>()).text = "" + battler.level;

        (infoPanel.transform.GetChild(7).gameObject.GetComponent<TMP_Text>()).text = "" + battler.GetCurrStr();
        if (battler.GetCurrStr() > battler.str)
            (infoPanel.transform.GetChild(7).gameObject.GetComponent<TMP_Text>()).color = new Color32(255, 145, 0, 255);
        else if (battler.GetCurrStr() < battler.str)
            (infoPanel.transform.GetChild(7).gameObject.GetComponent<TMP_Text>()).color = new Color32(60, 125, 255, 255);

        (infoPanel.transform.GetChild(8).gameObject.GetComponent<TMP_Text>()).text = "" + battler.GetCurrWil();
        if (battler.GetCurrWil() > battler.wil)
            (infoPanel.transform.GetChild(8).gameObject.GetComponent<TMP_Text>()).color = new Color32(255, 145, 0, 255);
        else if (battler.GetCurrWil() < battler.wil)
            (infoPanel.transform.GetChild(8).gameObject.GetComponent<TMP_Text>()).color = new Color32(60, 125, 255, 255);

        (infoPanel.transform.GetChild(9).gameObject.GetComponent<TMP_Text>()).text = "" + battler.GetCurrIni();
        if (battler.GetCurrIni() > battler.ini)
            (infoPanel.transform.GetChild(9).gameObject.GetComponent<TMP_Text>()).color = new Color32(255, 145, 0, 255);
        else if (battler.GetCurrIni() < battler.ini)
            (infoPanel.transform.GetChild(9).gameObject.GetComponent<TMP_Text>()).color = new Color32(60, 125, 255, 255);

        (infoPanel.transform.GetChild(10).gameObject.GetComponent<TMP_Text>()).text = "" + battler.GetCurrDef() * 100 + "%";
        if (battler.GetCurrDef() > battler.def)
            (infoPanel.transform.GetChild(10).gameObject.GetComponent<TMP_Text>()).color = new Color32(255, 145, 0, 255);
        else if (battler.GetCurrDef() < battler.def)
            (infoPanel.transform.GetChild(10).gameObject.GetComponent<TMP_Text>()).color = new Color32(60, 125, 255, 255);

        (infoPanel.transform.GetChild(11).gameObject.GetComponent<TMP_Text>()).text = "" + battler.GetCurrRes() * 100 + "%";
        if (battler.GetCurrRes() > battler.res)
            (infoPanel.transform.GetChild(11).gameObject.GetComponent<TMP_Text>()).color = new Color32(255, 145, 0, 255);
        else if (battler.GetCurrRes() < battler.res)
            (infoPanel.transform.GetChild(11).gameObject.GetComponent<TMP_Text>()).color = new Color32(60, 125, 255, 255);

        (infoPanel.transform.GetChild(12).gameObject.GetComponent<TMP_Text>()).text = "" + battler.GetCurrCrt() * 100 + "%";
        if (battler.GetCurrCrt() > battler.crt)
            (infoPanel.transform.GetChild(12).gameObject.GetComponent<TMP_Text>()).color = new Color32(255, 145, 0, 255);
        else if (battler.GetCurrCrt() < battler.crt)
            (infoPanel.transform.GetChild(12).gameObject.GetComponent<TMP_Text>()).color = new Color32(60, 125, 255, 255);

        (infoPanel.transform.GetChild(17).gameObject.GetComponent<TMP_Text>()).text = ": " + battler.GetCurrCurseResistance() * 100 + "%";
        if (battler.GetCurrCurseResistance() > battler.curseResist)
            (infoPanel.transform.GetChild(17).gameObject.GetComponent<TMP_Text>()).color = new Color32(255, 145, 0, 255);
        else if (battler.GetCurrCurseResistance() < battler.curseResist)
            (infoPanel.transform.GetChild(17).gameObject.GetComponent<TMP_Text>()).color = new Color32(60, 125, 255, 255);

        (infoPanel.transform.GetChild(18).gameObject.GetComponent<TMP_Text>()).text = ": " + battler.GetCurrSealResistance() * 100 + "%";
        if (battler.GetCurrSealResistance() > battler.sealResist)
            (infoPanel.transform.GetChild(18).gameObject.GetComponent<TMP_Text>()).color = new Color32(255, 145, 0, 255);
        else if (battler.GetCurrSealResistance() < battler.sealResist)
            (infoPanel.transform.GetChild(18).gameObject.GetComponent<TMP_Text>()).color = new Color32(60, 125, 255, 255);

        (infoPanel.transform.GetChild(19).gameObject.GetComponent<TMP_Text>()).text = ": " + battler.GetCurrStaggerResistance() * 100 + "%";
        if (battler.GetCurrStaggerResistance() > battler.staggerResist)
            (infoPanel.transform.GetChild(19).gameObject.GetComponent<TMP_Text>()).color = new Color32(255, 145, 0, 255);
        else if (battler.GetCurrStaggerResistance() < battler.staggerResist)
            (infoPanel.transform.GetChild(19).gameObject.GetComponent<TMP_Text>()).color = new Color32(60, 125, 255, 255);

        int currX = 0, currY = 0, i = 0;

        foreach(StatusEffect effect in battler.statusEffects.Keys)
        {
            GameObject portraitStatusEffect = Instantiate(battle.portraitStatusEffect, infoPanel.transform.GetChild(29).gameObject.transform) as GameObject;
            portraitStatusEffect.GetComponent<Image>().sprite = effect.portrait_65;

            if(battler.statusEffects[effect] >= 0)
                portraitStatusEffect.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "" + battler.statusEffects[effect];
            else 
                portraitStatusEffect.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "";
            
                
        }

        foreach (BuffEffect effect in battler.buffEffects.Keys)
        {
            if (battler.buffEffects[effect] < 0)
            {
                GameObject portraitBuffEffect = Instantiate(battle.portraitBuffEffect, infoPanel.transform.GetChild(29).gameObject.transform) as GameObject;

                portraitBuffEffect.GetComponent<Image>().sprite = effect.portrait_65;

                if (effect.isDebuff)
                    (portraitBuffEffect.transform.GetChild(0).GetComponent<TMP_Text>()).text = "" + battler.GetBuffValue(effect.buffStat) * 100 + "%";
                else
                    (portraitBuffEffect.transform.GetChild(0).GetComponent<TMP_Text>()).text = "+" + battler.GetBuffValue(effect.buffStat) * 100 + "%";

                portraitBuffEffect.GetComponent<RectTransform>().anchoredPosition = new Vector2(currX, currY);
            }
            else
                break;

            if (i % 5 == 0 && i != 0)
            {
                currY += 75;
                currX = 0;
            }
            else
                currX += 75;

            i++;
        }

    }

    
}
