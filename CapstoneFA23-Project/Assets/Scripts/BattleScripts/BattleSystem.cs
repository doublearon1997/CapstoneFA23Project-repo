using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class BattleSystem : MonoBehaviour
{
    public List<GameObject> startingPlayerBattlerGOs = new List<GameObject>();
    public List<GameObject> startingEnemyBattlerGOs = new List<GameObject>();

    public List<PlayerBattler> playerBattlers;
    public List<EnemyBattler> enemyBattlers;

    public Transform[] playerSpawns;
    public Transform[] enemySpawns;

    public GameObject panelPlayerActions;
    public GameObject damageTextPopup, healTextPopup;

    private bool inPinch = false;

    //Current Encounter: Set when battle is loaded in.
    public static Encounter currentEncounter = null;

    //Player Overview Panel Objects
    public GameObject[] playerContainers, playerOverviewPortraits;
    public TMP_Text[] playerNameTexts, playerClassLevelTexts, playerHPTexts;
    public Slider[] playerHPSliders;

    //Enemy Overview Panel Objects;
    public GameObject[] enemyContainers;
    public TMP_Text[] enemyNameTexts, enemyHPTexts;
    public Slider[] enemyHPSliders;

    //Turn Order Stuff
    public GameObject panelTurnOrder;
    public GameObject portraitTurnOrderCurrentEnemy;
    public GameObject portraitTurnOrderCurrentPlayer;
    public GameObject portraitTurnOrderEnemy;
    public GameObject portraitTurnOrderPlayer;

    SortedSet<Battler> currentlyActingBattlers;
    public Battler currentlyActingBattler;

    //Player Action Buttons
    public Button buttonAttack, buttonSkills, buttonTactics, buttonItems;
    private bool buttonAttackPressed = false, buttonSkillsPressed = false, buttonTacticsPressed = false, buttonItemsPressed = false;
    public bool skillSelected = false;

    //Skill Selection Panel
    public GameObject panelSkillSelection;
    public GameObject buttonSkill;

    public GameObject imageSkillCooldown;

    //Skill Stat Box
    public GameObject panelOffensiveSkillStatBox;
    public GameObject panelSupportSkillStatBox;

    //Battler Info Hover Stuff
    public GameObject infoHoverObject;
    public GameObject portraitBuffEffect;
    private List<GameObject> currentInfoHoverObjects = new List<GameObject>();

    //Skill Targeting Objects
    public GameObject offensiveTarget100;
    public GameObject supportTarget100;

    public GameObject offensiveTargetEnemy100;
    public GameObject supportTargetEnemy100;

    public List<GameObject> currentTargetingObjects;

    //Message Box
    public GameObject battleMessageBox = null;
    public GameObject skillMessageDisplay = null;
    public GameObject messageBoxText = null;

    public GameObject hotkeyManager;

    public void Start()
    {
        currentlyActingBattlers = new SortedSet<Battler>(new BattlerAPComparator());

        StartCoroutine(SetupBattle()); 
    }

    IEnumerator SetupBattle()
    {
        this.playerBattlers = new List<PlayerBattler>();
        this.enemyBattlers = new List<EnemyBattler>();

        for (int i = 0; i < startingPlayerBattlerGOs.Count; i++)
        {
            GameObject playerGO = Instantiate(startingPlayerBattlerGOs[i], playerSpawns[i]);
            playerBattlers.Add(playerGO.GetComponent<PlayerBattler>());

            playerGO.GetComponent<PlayerBattler>().ap = UnityEngine.Random.Range(0, 20000);

        }

        if (currentEncounter != null)
        {
            for (int i = 0; i < currentEncounter.enemies.Count; i++)
            {
                GameObject enemyGO = Instantiate(currentEncounter.enemies[i], enemySpawns[i]);
                enemyBattlers.Add(enemyGO.GetComponent<EnemyBattler>());

                enemyGO.GetComponent<EnemyBattler>().ap = UnityEngine.Random.Range(0, 20000);
                BGMManager.instance.PlayBGM(currentEncounter.battleBGM);
            }
        }
        else // this is just for testing purposes when not entering from an encounter.
        {
            for (int i = 0; i < startingEnemyBattlerGOs.Count; i++)
            {
                GameObject enemyGO = Instantiate(startingEnemyBattlerGOs[i], enemySpawns[i]);
                enemyBattlers.Add(enemyGO.GetComponent<EnemyBattler>());

                enemyGO.GetComponent<EnemyBattler>().ap = UnityEngine.Random.Range(0, 20000);

                BGMManager.instance.PlayBGM("windUpYesterday");

            }
        }

        SetupPartyOverviewPanel();
        SetupEnemyOverviewPanel();

        yield return new WaitForSeconds(2f);

        StartCoroutine(DetermineNextBattler());
        SetTurnOrderPanel();
    }

    //This method profilerates the overview panel with information about the party's battlers. 
    public void SetupPartyOverviewPanel()
    {
        for (int i = 0; i < playerBattlers.Count; i++)
        {
            PlayerBattler battler = playerBattlers[i];
            battler.SetPartyPosition(i);
            battler.Initialize();

            playerNameTexts[i].text = battler.battlerName;
            playerClassLevelTexts[i].text = " Lvl " + battler.level + " " + battler.playerClass;
            playerHPTexts[i].text = "" + battler.hp + "/" + battler.mhp;

            playerHPSliders[i].maxValue = battler.mhp;
            playerHPSliders[i].minValue = battler.hp;

            playerOverviewPortraits[i].GetComponent<Image>().sprite = battler.portrait60;

            playerContainers[i].SetActive(true);
        }
    }

    //This method profilerates the overview panel with information about the party's battlers. 
    public void SetupEnemyOverviewPanel()
    {
        for (int i = 0; i < enemyBattlers.Count; i++)
        {
            EnemyBattler battler = enemyBattlers[i];
            battler.SetPartyPosition(i);

            enemyNameTexts[i].text = battler.battlerName;
            enemyHPTexts[i].text = "" + battler.hp + "/" + battler.mhp;

            enemyHPSliders[i].maxValue = battler.mhp;
            enemyHPSliders[i].minValue = battler.hp;

            enemyContainers[i].SetActive(true);
        }
    }

    // Displays the current turn order on the turn order panel.
    private void SetTurnOrderPanel()
    {
        clearTurnOrderPanel();

        List<Battler> turnOrder = DetermineCurrentTurnOrder();

        int bigPortraitCushion = 0;

        for (int i = 0; i < turnOrder.Count; i++)
        {
            GameObject portrait;
            Battler battler = turnOrder[i];

            if (battler.isPlayer)
            {
                if (battler == currentlyActingBattler)
                    portrait = Instantiate(portraitTurnOrderCurrentPlayer, panelTurnOrder.transform) as GameObject;
                else
                    portrait = Instantiate(portraitTurnOrderPlayer, panelTurnOrder.transform) as GameObject;
            }
            else
            {
                if (battler == currentlyActingBattler)
                    portrait = Instantiate(portraitTurnOrderCurrentEnemy, panelTurnOrder.transform) as GameObject;
                else
                    portrait = Instantiate(portraitTurnOrderEnemy, panelTurnOrder.transform) as GameObject;
            }

            portrait.GetComponent<Image>().sprite = battler.portrait60;
            portrait.GetComponent<Image>().enabled = true;
            portrait.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, i * -75.0f - 46 - bigPortraitCushion);

            if (battler == currentlyActingBattler)
                bigPortraitCushion = 7;

        }
    }

    // Displays the temporary turn order on the turn order panel, when a player has a skill selected or a battler is using a skill, but not before the end of the turn.
    public void SetTemporaryTurnOrderPanel(Skill skill)
    {
        clearTurnOrderPanel();

        List<Battler> turnOrder = DetermineCurrentTurnOrder(skill);
        GameObject emptyPortrait;

        if (currentlyActingBattler.isPlayer)
            emptyPortrait = Instantiate(portraitTurnOrderCurrentPlayer, panelTurnOrder.transform) as GameObject;
        else
            emptyPortrait = Instantiate(portraitTurnOrderCurrentEnemy, panelTurnOrder.transform) as GameObject;

        emptyPortrait.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, -46.0f);

        for (int i = 0; i < turnOrder.Count; i++)
        {
            GameObject portrait;
            Battler battler = turnOrder[i];

            if (battler.isPlayer)
                portrait = Instantiate(portraitTurnOrderPlayer, panelTurnOrder.transform) as GameObject;

            else
                portrait = Instantiate(portraitTurnOrderEnemy, panelTurnOrder.transform) as GameObject;

            portrait.GetComponent<Image>().sprite = battler.portrait60;
            portrait.GetComponent<Image>().enabled = true;

            if (battler == currentlyActingBattler)
                portrait.GetComponent<RectTransform>().anchoredPosition = new Vector2(12.0f, (i + 1) * -75.0f - 53);
            else
                portrait.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, (i + 1) * -75.0f - 53);

        }
    }

    //Clears the turn order panel of turn order portraits.
    private void clearTurnOrderPanel()
    {
        while (panelTurnOrder.transform.childCount > 0)
        {
            DestroyImmediate(panelTurnOrder.transform.GetChild(0).gameObject);
        }
    }

    // This method basically "looks into the future" to see the order in which battlers will act, so that a turn order can be displayed for the player.
    private List<Battler> DetermineCurrentTurnOrder(Skill currentBattlerSkill = null)
    {
        List<Battler> battlerTurnOrder = new List<Battler>();

        Dictionary<Battler, int> needToCalculate = new Dictionary<Battler, int>();
        List<Battler> doneCalculated = new List<Battler>();
        SortedSet<Battler> futureCurrentlyActingBattlers = new SortedSet<Battler>(new BattlerAPComparator());

        //Add battlers to calculate to a dictionary to prevent actually adding ap to the real battlers.
        foreach (PlayerBattler battler in playerBattlers)
        {
            if (!currentlyActingBattlers.Contains(battler))
                needToCalculate.Add(battler, battler.ap);
            else if (currentBattlerSkill != null && currentlyActingBattler == battler)
                needToCalculate.Add(battler, battler.ap - 100000);
        }

        foreach (EnemyBattler battler in enemyBattlers)
        {
            if (!currentlyActingBattlers.Contains(battler))
                needToCalculate.Add(battler, battler.ap);
            else if (currentBattlerSkill != null && currentlyActingBattler == battler)
                needToCalculate.Add(battler, battler.ap - 100000);
        }

        // Add the battlers that don't need any future calculations.
        foreach (Battler battler in currentlyActingBattlers)
        {
            if (currentBattlerSkill == null || battler != currentlyActingBattler)
                battlerTurnOrder.Add(battler);
        }

        while (doneCalculated.Count != needToCalculate.Count)
        {
            futureCurrentlyActingBattlers.Clear();

            var keys = new List<Battler>(needToCalculate.Keys);

            for (int i = 0; i < keys.Count; i++)
            {
                Battler battler = keys[i];

                if (needToCalculate[battler] < 100000)
                {
                    if (currentBattlerSkill == null && battler == currentlyActingBattler)
                        needToCalculate[battler] = needToCalculate[battler] + ((int)(battler.ini * currentBattlerSkill.apMod));
                    else
                        needToCalculate[battler] = needToCalculate[battler] + ((int)(battler.ini * battler.apMod));

                    if (needToCalculate[battler] >= 100000)
                        futureCurrentlyActingBattlers.Add(battler);
                }

            }

            foreach (Battler battler in futureCurrentlyActingBattlers)
            {
                battlerTurnOrder.Add(battler);
                doneCalculated.Add(battler);
            }

        }

        return battlerTurnOrder;
    }

    //Adjusts a battler's HP slider value on the party overview panel based on their current hp.
    //TODO: Animate?
    public void SetPlayerHPSliderValue(PlayerBattler battler)
    {
        const int sliderWidth = 408;

        double hpSliderPosition = sliderWidth - ((double)battler.hp / (double)battler.mhp) * sliderWidth;

        playerHPSliders[battler.GetPartyPosition()].transform.GetChild(1).GetComponent<RectTransform>().offsetMin = new Vector2((float)hpSliderPosition, 0.0f);
        playerHPTexts[battler.GetPartyPosition()].text = "" + battler.hp + "/" + battler.mhp;
    }

    //Adjusts a battler's HP slider value on the enemy overview panel based on their current hp.
    //TODO: Animate?
    public void SetEnemyHPSliderValue(EnemyBattler battler)
    {
        const int sliderWidth = 250;

        double hpSliderPosition = sliderWidth - ((double)battler.hp / (double)battler.mhp) * sliderWidth;

        enemyHPSliders[battler.GetPartyPosition()].transform.GetChild(1).GetComponent<RectTransform>().offsetMin = new Vector2((float)hpSliderPosition, 0.0f);
        enemyHPTexts[battler.GetPartyPosition()].text = "" + battler.hp + "/" + battler.mhp;
    }

    //This coroutine is played when determining which battler acts next.
    public IEnumerator DetermineNextBattler()
    {
        while (currentlyActingBattlers.Count < 1) //runs this loop until a battler has atleast 100000 AP
        {
            foreach (PlayerBattler battler in playerBattlers)
            {
                battler.ap += (int)(battler.ini * battler.apMod);
                if (battler.ap >= 100000)
                {
                    currentlyActingBattlers.Add(battler);
                }
            }

            foreach (EnemyBattler battler in enemyBattlers)
            {
                battler.ap += (int)(battler.ini * battler.apMod);
                if (battler.ap >= 100000)
                {
                    currentlyActingBattlers.Add(battler);
                }
            }
        }

        currentlyActingBattler = currentlyActingBattlers.First();

        yield return new WaitForSeconds(0.5f);

        if (currentlyActingBattler.isPlayer)
        {
            StartCoroutine(PlayerTurn());
        }
        else
        {
            StartCoroutine(EnemyTurn());
        }
    }

    public void SetInfoHovers()
    {
        foreach(PlayerBattler battler in playerBattlers)
        {
            GameObject infoObject = Instantiate(infoHoverObject, battler.gameObject.transform) as GameObject;
            currentInfoHoverObjects.Add(infoObject);

            infoObject.GetComponent<BattlerStatHover>().Initialize((Battler)battler, this);
        }

        foreach (EnemyBattler battler in enemyBattlers)
        {
            GameObject infoObject = Instantiate(infoHoverObject, battler.gameObject.transform) as GameObject;
            currentInfoHoverObjects.Add(infoObject);

            infoObject.GetComponent<BattlerStatHover>().Initialize((Battler)battler, this);
        }
    }

    public void RemoveInfoHovers()
    {
        foreach (GameObject obj in currentInfoHoverObjects)
        {
            DestroyImmediate(obj);

        }
    }

    //Attack Button Methods
    public void AttackButtonPress()
    {
        buttonAttack.interactable = false;
        ReturnAnyPlayerActionHandlers();

        buttonAttackPressed = true;
        hotkeyManager.AddComponent<AttackButtonSelectedHotkeys>().Initialize(this);
        ((PlayerBattler)currentlyActingBattler).standardAttack.ChooseTarget((PlayerBattler)currentlyActingBattler, this);
        SetTemporaryTurnOrderPanel(((PlayerBattler)currentlyActingBattler).standardAttack);

        DisplayMessage("Select a target.");
    }

    public void AttackButtonReturn()
    {
        buttonAttack.interactable = true;
        buttonAttackPressed = false;
        Destroy(hotkeyManager.GetComponent<AttackButtonSelectedHotkeys>());
        ClearTargetingButtons();
        SetTurnOrderPanel();

    }

    //Skills Button Methods
    public void SkillsButtonPress()
    {
        buttonSkills.interactable = false;
        ReturnAnyPlayerActionHandlers();

        SetSkillSelectionPanel((PlayerBattler)currentlyActingBattler);
        buttonSkillsPressed = true;
        hotkeyManager.AddComponent<SkillsButtonSelectedHotkeys>().Initialize(this);
        panelSkillSelection.SetActive(true);

        DisplayMessage("Select a skill.");
    }

    public void SkillsButtonReturn()
    {
        buttonSkills.interactable = true;
        buttonSkillsPressed = false;
        Destroy(hotkeyManager.GetComponent<SkillsButtonSelectedHotkeys>());

        panelSkillSelection.SetActive(false);
        ClearSkillSelectionPanel();
  
    }

    private void SetSkillSelectionPanel(PlayerBattler battler)
    {
        ClearSkillSelectionPanel();

        float xPos = 80.0f;
        float yPos = -60.0f;
        int numSkills = battler.skills.Count;
        int currentSkillCount = 0;

        foreach (Skill skill in battler.skills)
        {
            currentSkillCount += 1;
            CreateSkillButton(skill, battler, xPos, yPos);

            if (currentSkillCount % 3 == 0)
            {
                xPos = 80.0f;
                yPos -= 119.0f;
            }
            else
                xPos += 119.0f;
        }
    }

    //Clears the selection panel of skill portraits.
    private void ClearSkillSelectionPanel()
    {
        while (panelSkillSelection.transform.childCount > 1)
        {
            DestroyImmediate(panelSkillSelection.transform.GetChild(1).gameObject);
        }
    }

    private void CreateSkillButton(Skill skill, PlayerBattler battler, float xPos, float yPos)
    {
        GameObject skillButton = Instantiate(buttonSkill, panelSkillSelection.transform) as GameObject;

        skillButton.GetComponent<SkillButton>().Initialize(skill, battler, this);
        skillButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);

        if (battler.skillCooldownDict[skill] > 0)
        {
            GameObject cooldownImage = Instantiate(imageSkillCooldown, skillButton.transform) as GameObject;
            cooldownImage.transform.GetChild(0).GetComponent<TMP_Text>().text = "" + battler.skillCooldownDict[skill];
        }
    }

    public void SkillTargetReturn()
    {
        Destroy(hotkeyManager.GetComponent<SkillTargetHotkeys>());
        ClearTargetingButtons();
        skillSelected = false;
        SetTurnOrderPanel();
    }

    private void ClearTargetingButtons()
    {
        foreach (GameObject obj in currentTargetingObjects)
            DestroyImmediate(obj);
    }

    private void ReturnAnyPlayerActionHandlers()
    {
        if(skillSelected)
            SkillTargetReturn();

        if(buttonAttackPressed)
            AttackButtonReturn();
        else if(buttonSkillsPressed)
            SkillsButtonReturn();

    }

    IEnumerator PlayerTurn()
    {
        SetTurnOrderPanel();
        SEManager.instance.PlaySE("playerTurnStart");

        yield return new WaitForSeconds(0.4f);

        SetInfoHovers();
        DisplayMessage("" + currentlyActingBattler.battlerName + "'s turn.");
        ResetPlayerActionsPanel();
        panelPlayerActions.SetActive(true);

    }

    private void ResetPlayerActionsPanel()
    {
        buttonAttack.interactable = true;
        buttonSkills.interactable = true;
        buttonTactics.interactable = true;
        buttonItems.interactable = true;
    }

    public void PlayerActionSelected()
    {
        panelPlayerActions.SetActive(false);
        panelSkillSelection.SetActive(false);

        RemoveInfoHovers();

        ClearAllHotkeys();

        ClearTargetingButtons();
    }

    public IEnumerator FinishPlayerTurn()
    {
        currentlyActingBattler.CountDownEffects();

        currentlyActingBattlers.Remove(currentlyActingBattler);
        currentlyActingBattler = null;

        buttonAttack.interactable = true;

        if (IsInPinch() && !inPinch)
        {
            inPinch = true;
            StartCoroutine(GoIntoPinch());
        }
        else if (!IsInPinch() && inPinch)
        {
            inPinch = false;
            StartCoroutine(LeavePinch());
        }

        yield return new WaitForSeconds(1.5f);

        ClearMessageBox();

        if (IsPlayerVictory())
            PlayerVictory();

        if (IsPlayerDefeat())
            PlayerDefeat();

        StartCoroutine(DetermineNextBattler());
    }

    IEnumerator EnemyTurn()
    {
        SetTurnOrderPanel();

        Skill chosenSkill = ((EnemyBattler)currentlyActingBattler).ChooseSkill();
        List<GameObject> targetObjects = new List<GameObject>();

        if(chosenSkill.isOffensive)
        {
            Battler chosenTarget = ((OffensiveSkill)chosenSkill).ChooseTarget((EnemyBattler)currentlyActingBattler, this);
            targetObjects.Add(Instantiate(offensiveTargetEnemy100, chosenTarget.gameObject.transform) as GameObject);

            SEManager.instance.PlaySE("enemyTurn");
            DisplaySkillMessage(chosenSkill);
            yield return new WaitForSeconds(1.5f);

            ((OffensiveSkill)chosenSkill).UseSkill(currentlyActingBattler, chosenTarget, this);
        }
        else
        {
            Battler chosenTarget = ((SupportSkill)chosenSkill).ChooseTarget((EnemyBattler)currentlyActingBattler, this);
            Instantiate(supportTargetEnemy100, chosenTarget.gameObject.transform);

            SEManager.instance.PlaySE("enemyTurn");
            DisplaySkillMessage(chosenSkill);
            yield return new WaitForSeconds(1.5f);

            ((SupportSkill)chosenSkill).UseSkill(currentlyActingBattler, chosenTarget, this);
        }

        foreach (GameObject target in targetObjects)
            DestroyImmediate(target);
       
        StartCoroutine(FinishEnemyTurn());

    }

    IEnumerator FinishEnemyTurn()
    {
        currentlyActingBattler.CountDownEffects();
        
        currentlyActingBattlers.Remove(currentlyActingBattler);
        currentlyActingBattler = null;

        if (IsInPinch() && !inPinch)
        {
            inPinch = true;
            StartCoroutine(GoIntoPinch());
        }
        else if (!IsInPinch() && inPinch)
        {
            inPinch = false;
            StartCoroutine(LeavePinch());
        }

        yield return new WaitForSeconds(1.5f);

        ClearMessageBox();

        if (IsPlayerVictory())
            PlayerVictory();

        if(IsPlayerDefeat())
            PlayerDefeat();

        StartCoroutine(DetermineNextBattler());
    }

    IEnumerator GoIntoPinch()
    {
        StartCoroutine(BGMManager.instance.FadeOutBGM(1.4f));

        yield return new WaitForSeconds(1.5f);
        BGMManager.instance.PlayBGM("demonicDrive");
    }

    IEnumerator LeavePinch()
    {
        StartCoroutine(BGMManager.instance.FadeOutBGM(1.4f));

        yield return new WaitForSeconds(1.5f);
        BGMManager.instance.PlayBGM(currentEncounter.battleBGM);

    }

    public bool IsInPinch()
    {
        if (playerBattlers.Count < startingPlayerBattlerGOs.Count)
            return true;

        double partyMaxHP = 0, partyHP = 0;

        foreach(PlayerBattler battler in playerBattlers)
        {
            partyMaxHP += battler.mhp;
            partyHP += battler.hp;
        }

        if (partyHP / partyMaxHP < 0.3)
            return true;
        else
            return false;
    }

    private void ClearAllHotkeys()
    {
        Destroy(hotkeyManager.GetComponent<SkillTargetHotkeys>());
        Destroy(hotkeyManager.GetComponent<SkillsButtonSelectedHotkeys>());
        Destroy(hotkeyManager.GetComponent<AttackButtonSelectedHotkeys>());
    }

    public void DisplayMessage(string message)
    {
        ClearMessageBox();

        GameObject textMessage = Instantiate(messageBoxText, battleMessageBox.transform) as GameObject;
        textMessage.GetComponent<TMP_Text>().text = message;     
    }

    public void DisplaySkillMessage(Skill skill)
    {
        ClearMessageBox();

        GameObject textMessage = Instantiate(skillMessageDisplay, battleMessageBox.transform) as GameObject;

        TMP_Text textObject = textMessage.transform.GetChild(0).GetComponent<TMP_Text>();
        textObject.text = skill.skillName;

        textObject.ForceMeshUpdate();

        textObject.GetComponent<RectTransform>().sizeDelta = new Vector2(textObject.textBounds.max.x - textObject.textBounds.min.x, 50);
        textMessage.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>().sprite = skill.portrait65;
        
    }

    public void ClearMessageBox()
    {
        if(battleMessageBox.transform.childCount > 1)
            DestroyImmediate(battleMessageBox.transform.GetChild(1).gameObject);
    }

    private bool IsPlayerVictory()
    {
        return enemyBattlers.Count <= 0;
    }

    private bool IsPlayerDefeat()
    {
        return playerBattlers.Count <= 0;
    }

    private void PlayerVictory()
    {
        SceneManager.LoadScene("sceneTutorialLevel");
    }

    private void PlayerDefeat()
    {
        SceneManager.LoadScene("sceneMain");
    }

    internal class BattlerAPComparator : IComparer<Battler>
    {
        public int Compare(Battler a, Battler b)
        {
            return a.ap.CompareTo(b.ap);
        }
    }



}
