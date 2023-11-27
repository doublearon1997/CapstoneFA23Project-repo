using System.CodeDom;
using System;
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
    public Image backgroundImage;
    public GameObject playerBattler;
    public List<PlayerBattler> startingPlayerBattlers;
    public List<EnemyBattler> startingEnemyBattlers;

    public List<PlayerBattler> playerBattlers;
    public List<EnemyBattler> enemyBattlers;

    public GameObject fadeOutToBlack;

    public Transform[] playerSpawns;
    public Transform[] enemySpawns;

    public Transform enemyAOELoc;
    public Transform playerAOELoc;

    public GameObject panelPlayerActions;

    //Skill Popups
    public GameObject damageTextPopup, healTextPopup;
    public GameObject buffPopup, debuffPopup, cooldownClearPopup, cursePopup, sealPopup, staggerPopup, buffsClearPopup, debuffsClearPopup, markPopup;

    private bool inPinch = false;

    public KOEffect kOEffect;

    public InventoryController inventory;
    public PartyController party;

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

    //Current Actor Pointers
    public GameObject playerPointer;
    public GameObject enemyPointer;
    private GameObject currentPointer = null;

    //Turn Order Stuff
    public GameObject panelTurnOrder;
    public GameObject portraitTurnOrderCurrentEnemy;
    public GameObject portraitTurnOrderCurrentPlayer;
    public GameObject portraitTurnOrderEnemy;
    public GameObject portraitTurnOrderPlayer;

    public bool tempTurnOrder = false;

    public GameObject turnOrderHighlightAnimation;
    public GameObject turnOrderCurrentHighlightAnimation;

    public Dictionary<Battler, GameObject> battlerTurnOrderObjects = new Dictionary<Battler, GameObject>();

    SortedSet<Battler> currentlyActingBattlers = new SortedSet<Battler>(new BattlerAPComparator());
    public Battler currentlyActingBattler;

    //Player Action Buttons
    public Button buttonAttack, buttonSkills, buttonTactics, buttonItems;
    private bool buttonAttackPressed = false, buttonSkillsPressed = false, buttonTacticsPressed = false, buttonItemsPressed = false;
    public bool skillSelected = false;

    //Skill Selection Panel
    public GameObject panelSkillSelection;
    public GameObject buttonSkill;

    //Tactic Selection Panel
    public GameObject panelTacticSelection;
    public SupportSkill fleeSkill, defendSkill, waitSkill;

    //Item Selection Panel
    public GameObject panelItemSelection;

    public GameObject imageSkillCooldown;

    //Skill Stat Box
    public GameObject panelOffensiveSkillStatBox;
    public GameObject panelSupportSkillStatBox;

    //Battler Info Hover Stuff
    public GameObject infoHoverObject;
    public GameObject portraitBuffEffect, portraitStatusEffect;
    private List<GameObject> currentInfoHoverObjects = new List<GameObject>();

    //Skill Targeting Objects
    public GameObject offensiveTarget100;
    public GameObject supportTarget100;

    public GameObject offensiveTarget550;
    public GameObject supportTarget500;

    public GameObject offensiveTargetEnemy100;
    public GameObject supportTargetEnemy100;

    public GameObject offensiveTargetEnemy500;
    public GameObject supportTargetEnemy550;

    public List<GameObject> currentTargetingObjects;

    //Message Box
    public GameObject battleMessageBox = null;
    public GameObject skillMessageDisplay = null;
    public GameObject messageBoxText = null;

    public GameObject hotkeyManager;

    public Encounter defaultEncounter; //Just used for testing.

    //Victory Objects
    public GameObject victoryPanel;
    public GameObject levelUpPanel;

    public GameObject playerSlotVictoryPanel;
    public GameObject itemVictoryIcon;
    public GameObject LevelupSkillUnlockButton;

    public Skill selectedLevelupSkill = null;

    private List<Character> levelUps = new List<Character>();
    private List<int> levelUpNewLevels = new List<int>();
    private int levelUpsLoc = 0;

    public void Start()
    {
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        if(currentEncounter == null)
            currentEncounter = defaultEncounter;

        if(currentEncounter.battleBackgroundImage != null)
            backgroundImage.sprite = currentEncounter.battleBackgroundImage;

        this.playerBattlers = new List<PlayerBattler>();
        this.enemyBattlers = new List<EnemyBattler>();

        this.startingPlayerBattlers = new List<PlayerBattler>();
        this.startingEnemyBattlers = new List<EnemyBattler>();

        for (int i = 0; i < party.GetPartyList().Count; i++) //Make PlayerBattler GameObjects and fill them in with stats from the Characters in the Party.
        {
            GameObject playerGO = Instantiate(playerBattler, playerSpawns[i]);
            PlayerBattler battler = playerGO.GetComponent<PlayerBattler>();
           
            battler.LoadStatsFromCharacter(party.GetPartyList()[i]);
            battler.SetPartyPosition(i);

            startingPlayerBattlers.Add(battler);
            playerBattlers.Add(battler);

            battler.ap = UnityEngine.Random.Range(0, 20000);
        }

        

        for (int i = 0; i < currentEncounter.enemies.Count; i++)
        {
            GameObject enemyGO = Instantiate(currentEncounter.enemies[i], enemySpawns[i]);
            startingEnemyBattlers.Add(enemyGO.GetComponent<EnemyBattler>());
            enemyBattlers.Add(enemyGO.GetComponent<EnemyBattler>());

            enemyGO.GetComponent<EnemyBattler>().ap = UnityEngine.Random.Range(0, 20000);
            BGMManager.instance.PlayBGM(currentEncounter.battleBGM);
        }

        SetupPartyOverviewPanel();
        SetupEnemyOverviewPanel();

        yield return new WaitForSeconds(1.5f);

        //REMOVEME
        //StartCoroutine(PlayerVictory());

        StartCoroutine(DetermineNextBattler());
        SetTurnOrderPanel();
    }

    //This method profilerates the overview panel with information about the party's battlers. 
    public void SetupPartyOverviewPanel()
    {
        for (int i = 0; i < playerBattlers.Count; i++)
        {
            PlayerBattler battler = playerBattlers[i];
            string playerClassString = battler.playerClass.ToString().Split('(')[0];
            
            playerNameTexts[i].text = battler.battlerName;
            playerClassLevelTexts[i].text = " Lvl " + battler.level + " " + playerClassString.Substring(0, playerClassString.Length-1);
            playerHPTexts[i].text = "" + battler.hp + "/" + battler.mhp;

            playerHPSliders[i].maxValue = battler.mhp;
            playerHPSliders[i].minValue = battler.hp;

            playerOverviewPortraits[i].GetComponent<Image>().sprite = battler.portrait60;

            playerContainers[i].SetActive(true);
        }
    }

    //This method profilerates the overview panel with information about the enemy battlers. 
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
        ClearTurnOrderPanel();
        battlerTurnOrderObjects.Clear();

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

            battlerTurnOrderObjects.Add(battler, portrait);

            if (battler == currentlyActingBattler)
                bigPortraitCushion = 7;

        }
        tempTurnOrder = false;
    }

    // Displays the temporary turn order on the turn order panel, when a player has a skill selected or a battler is using a skill, but not before the end of the turn.
    public void SetTemporaryTurnOrderPanel(Skill skill)
    {
        ClearTurnOrderPanel();
        battlerTurnOrderObjects.Clear();

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


            battlerTurnOrderObjects.Add(battler, portrait);
        }

        tempTurnOrder = true;
    }

    //Clears the turn order panel of turn order portraits.
    private void ClearTurnOrderPanel()
    {
        while (panelTurnOrder.transform.childCount > 0)
            DestroyImmediate(panelTurnOrder.transform.GetChild(0).gameObject);
        
        tempTurnOrder = false;
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
            if(!battler.isKO)
            {
                if (!currentlyActingBattlers.Contains(battler))
                    needToCalculate.Add(battler, battler.ap);
                else if (currentBattlerSkill != null && currentlyActingBattler == battler)
                    needToCalculate.Add(battler, battler.ap - 100000);
            }
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
            {
                if(!battler.isPlayer || (battler.isPlayer && ! ((PlayerBattler)battler).isKO))
                    battlerTurnOrder.Add(battler);
            }   
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
                    if(currentBattlerSkill != null && battler == currentlyActingBattler)
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
    public void SetPlayerHPSliderValue(PlayerBattler battler)
    {
        const int sliderWidth = 408;
        double hpSliderPosition = sliderWidth - ((double)battler.hp / (double)battler.mhp) * sliderWidth;

        TMP_Text hpText = playerHPTexts[battler.GetPartyPosition()];
        int startingHP = int.Parse(hpText.text.Split('/')[0]);
        Slider hpSlider = playerHPSliders[battler.GetPartyPosition()];

        if(startingHP > battler.hp)
        {
            hpSlider.transform.GetChild(2).GetComponent<RectTransform>().offsetMin = new Vector2((float)hpSliderPosition, 0.0f);
            StartCoroutine(AnimateHPSlider(hpSlider.transform.GetChild(1).GetComponent<RectTransform>(), hpSlider.transform.GetChild(2).GetComponent<RectTransform>().offsetMin, hpText, startingHP, battler.hp, battler.mhp));
        }
        else
        {
            hpSlider.transform.GetChild(1).GetComponent<RectTransform>().offsetMin = new Vector2((float)hpSliderPosition, 0.0f);
            StartCoroutine(AnimateHPSlider(hpSlider.transform.GetChild(2).GetComponent<RectTransform>(), hpSlider.transform.GetChild(1).GetComponent<RectTransform>().offsetMin, hpText, startingHP, battler.hp, battler.mhp));
        }
    }

    //Adjusts a battler's HP slider value on the enemy overview panel based on their current hp.
    public void SetEnemyHPSliderValue(EnemyBattler battler)
    {
        const int sliderWidth = 250;
        double hpSliderPosition = sliderWidth - ((double)battler.hp / (double)battler.mhp) * sliderWidth;

        TMP_Text hpText = enemyHPTexts[battler.GetPartyPosition()];
        int startingHP = int.Parse(hpText.text.Split('/')[0]);
        Slider hpSlider = enemyHPSliders[battler.GetPartyPosition()];

        if (startingHP > battler.hp)
        {
            hpSlider.transform.GetChild(2).GetComponent<RectTransform>().offsetMin = new Vector2((float)hpSliderPosition, 0.0f);
            StartCoroutine(AnimateHPSlider(hpSlider.transform.GetChild(1).GetComponent<RectTransform>(), hpSlider.transform.GetChild(2).GetComponent<RectTransform>().offsetMin, hpText, startingHP, battler.hp, battler.mhp));
        }
        else
        {
            hpSlider.transform.GetChild(1).GetComponent<RectTransform>().offsetMin = new Vector2((float)hpSliderPosition, 0.0f);
            StartCoroutine(AnimateHPSlider(hpSlider.transform.GetChild(2).GetComponent<RectTransform>(), hpSlider.transform.GetChild(1).GetComponent<RectTransform>().offsetMin, hpText, startingHP, battler.hp, battler.mhp));
        }
    }

    private IEnumerator AnimateHPSlider(RectTransform bar, Vector2 newPos, TMP_Text hpText, int startHP, int newHP, int mhp)
    {
        float t = 0;
        Vector2 startPos = bar.offsetMin;
    
        while (t < 2f)
        {
            bar.offsetMin = Vector2.Lerp(startPos, newPos, t);
            int hp = (int)(Mathf.Lerp(startHP, newHP, t));
            
            hpText.text = "" + hp + "/" + mhp;
            t+=Time.deltaTime;

            yield return null;
        }
    }

    //This coroutine is played when determining which battler acts next.
    public IEnumerator DetermineNextBattler()
    {
        List<Battler> removeBattlers = new List<Battler>();
        foreach(Battler battler in currentlyActingBattlers)
        {
            if(battler.isPlayer && ((PlayerBattler)battler).isKO)
                removeBattlers.Add(battler);
        }
        foreach(Battler battler in removeBattlers)
            currentlyActingBattlers.Remove(battler);

        while (currentlyActingBattlers.Count < 1) //runs this loop until a battler has atleast 100000 AP
        {
            foreach (PlayerBattler battler in playerBattlers)
            {
                if(!battler.isKO)
                {
                    battler.ap += (int)(battler.ini * battler.apMod);
                    if (battler.ap >= 100000)
                        currentlyActingBattlers.Add(battler); 
                }
                
            }

            foreach (EnemyBattler battler in enemyBattlers)
            {
                battler.ap += (int)(battler.ini * battler.apMod);
                if (battler.ap >= 100000)
                    currentlyActingBattlers.Add(battler);
                
            }
        }

        currentlyActingBattler = currentlyActingBattlers.First();

        yield return new WaitForSeconds(0.5f);

        if (currentlyActingBattler.isPlayer)
            StartCoroutine(PlayerTurn());
        else
            StartCoroutine(EnemyTurn());
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
            DestroyImmediate(obj);
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

    public void TacticsButtonPress()
    {
        buttonTactics.interactable = false;
        ReturnAnyPlayerActionHandlers();

        SetTacticSelectionPanel((PlayerBattler)currentlyActingBattler);
        buttonTacticsPressed = true;
        hotkeyManager.AddComponent<TacticsButtonSelectedHotkeys>().Initialize(this);
        panelTacticSelection.SetActive(true);

        DisplayMessage("Select a tactic.");
    }

    public void TacticsButtonReturn()
    {
        buttonTactics.interactable = true;
        buttonTacticsPressed = false;
        Destroy(hotkeyManager.GetComponent<TacticsButtonSelectedHotkeys>());

        panelTacticSelection.SetActive(false);
        ClearTacticSelectionPanel();
    }

    private void SetTacticSelectionPanel(PlayerBattler battler)
    {
        ClearTacticSelectionPanel();

        float xPos = 80.0f;
        float yPos = -60.0f;

        CreateTacticButton((Skill)defendSkill, battler, xPos, yPos);
        xPos += 119.0f;

        CreateTacticButton((Skill)waitSkill, battler, xPos, yPos);
        xPos += 119.0f;

        CreateTacticButton((Skill)fleeSkill, battler, xPos, yPos);
        xPos += 119.0f;

    }

    private void CreateTacticButton(Skill skill, PlayerBattler battler, float xPos, float yPos)
    {
        GameObject skillButton = Instantiate(buttonSkill, panelTacticSelection.transform) as GameObject;

        skillButton.GetComponent<SkillButton>().Initialize(skill, battler, this);
        skillButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
    }

    private void ClearTacticSelectionPanel()
    {
        while (panelTacticSelection.transform.childCount > 1)
            DestroyImmediate(panelTacticSelection.transform.GetChild(1).gameObject);
    }

    public void ItemsButtonPress()
    {
        buttonItems.interactable = false;
        ReturnAnyPlayerActionHandlers();

        SetItemSelectionPanel();
        buttonItemsPressed = true;
        hotkeyManager.AddComponent<ItemsButtonSelectedHotkeys>().Initialize(this);
        panelItemSelection.SetActive(true);

        DisplayMessage("Select an item.");
    }

    public void ItemsButtonReturn()
    {
        buttonItems.interactable = true;
        buttonItemsPressed = false;
        Destroy(hotkeyManager.GetComponent<ItemsButtonSelectedHotkeys>());

        panelItemSelection.SetActive(false);
        ClearItemSelectionPanel();
    }

    public void SetItemSelectionPanel()
    {
        //Filter out any items that are not consumable or have none in the inventory.
        Dictionary<Item, int> items = inventory.GetItemList().Where(item => item.Key.category == Item.ItemCategory.Consumable && item.Value > 0).ToDictionary(item => item.Key, item => item.Value);

        int itemRows = (int)(Math.Ceiling((double)items.Count / 3.0));

        GameObject panelImage = panelItemSelection.transform.GetChild(0).gameObject;
        panelImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 59.0f * (itemRows -1));
        panelImage.GetComponent<RectTransform>().sizeDelta = new Vector2(400, itemRows * 119);

        float xPos = 80.0f, yPos = -60.0f + (119 * (itemRows-1));
        int currItemCount = 0;

        foreach (Item item in items.Keys)
        {
            currItemCount += 1;
            CreateItemButton((ConsumableItem)item, items[item], (PlayerBattler)currentlyActingBattler, xPos, yPos);

            if (currItemCount % 3 == 0)
            {
                xPos = 80.0f;
                yPos -= 119.0f;
            }
            else
                xPos += 119.0f;
        }
    }

    public void ClearItemSelectionPanel()
    {
        while(panelItemSelection.transform.childCount > 2)
            DestroyImmediate(panelItemSelection.transform.GetChild(2).gameObject);
    }

    private void CreateItemButton(ConsumableItem item, int quantity, PlayerBattler battler, float xPos, float yPos)
    {
        GameObject itemButton = Instantiate(buttonSkill, panelItemSelection.transform) as GameObject;

        if(item.isSupportItem)
        {
            SupportItemSkill itemSkill = (SupportItemSkill)(SupportItemSkill.CreateInstance("SupportItemSkill"));
            itemSkill.Initialize(item, quantity);
            itemButton.GetComponent<SkillButton>().Initialize(itemSkill, battler, this);
            
        }
        else 
        {
            OffensiveItemSkill itemSkill = (OffensiveItemSkill)(OffensiveItemSkill.CreateInstance("OffensiveItemSkill"));
            itemSkill.Initialize(item, quantity);
            itemButton.GetComponent<SkillButton>().Initialize(itemSkill, battler, this);
        }
            
        itemButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);
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
        else if(!battler.willEnabled && skill.powerType == PowerType.Will)
        {
            skillButton.transform.GetChild(1).gameObject.SetActive(true);
        }
        else if(!battler.physicalEnabled && skill.powerType == PowerType.Physical)
        {
            skillButton.transform.GetChild(1).gameObject.SetActive(true);
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
        else if(buttonTacticsPressed)
            TacticsButtonReturn();
        else if(buttonItemsPressed)
            ItemsButtonReturn();
    }

    IEnumerator PlayerTurn()
    {
        SetTurnOrderPanel();
        SEManager.instance.PlaySE("playerTurnStart");

        yield return new WaitForSeconds(0.4f);

        currentPointer = (Instantiate(playerPointer, currentlyActingBattler.transform) as GameObject);

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
        panelTacticSelection.SetActive(false);
        panelItemSelection.SetActive(false);

        RemoveInfoHovers();

        ClearAllHotkeys();

        ClearTargetingButtons();
    }

    public IEnumerator FinishPlayerTurn(int additionalAnimations, float soundEffectHitDelay)
    {
        currentlyActingBattler.CountDownEffects();

        currentlyActingBattlers.Remove(currentlyActingBattler);
        currentlyActingBattler = null;

        buttonAttack.interactable = true;

        yield return new WaitForSeconds(1.7f + soundEffectHitDelay);

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

        yield return new WaitForSeconds((1.6f * additionalAnimations));

        DestroyImmediate(currentPointer);
        ClearMessageBox();

        if (IsPlayerVictory())
            StartCoroutine(PlayerVictory());
        else if (IsPlayerPartyFled())
            StartCoroutine(PlayerFlee());
        else if (IsPlayerDefeat())
            StartCoroutine(PlayerDefeat());
        else
            StartCoroutine(DetermineNextBattler());
    }

    IEnumerator EnemyTurn()
    {
        SetTurnOrderPanel();

        currentPointer = (Instantiate(enemyPointer, currentlyActingBattler.transform) as GameObject);

        Skill chosenSkill = ((EnemyBattler)currentlyActingBattler).ChooseSkill();
        List<GameObject> targetObjects = new List<GameObject>();

        if(chosenSkill.isOffensive)
        {
            List<PlayerBattler> chosenTargets = ((OffensiveSkill)chosenSkill).ChooseTarget((EnemyBattler)currentlyActingBattler, this);

            if (chosenSkill.targetType == TargetType.All)
                targetObjects.Add(Instantiate(offensiveTargetEnemy500, playerAOELoc) as GameObject);
            else
            {
                foreach (PlayerBattler battler in chosenTargets)
                    targetObjects.Add(Instantiate(offensiveTargetEnemy100, battler.gameObject.transform) as GameObject);
            }

            List<Battler> targets = new List<Battler>();
            foreach (PlayerBattler battler in chosenTargets)
                targets.Add((Battler)battler);

            SEManager.instance.PlaySE("enemyTurn");
            DisplaySkillMessage(chosenSkill);
            yield return new WaitForSeconds(1.5f);
            
            ((OffensiveSkill)chosenSkill).UseSkill(currentlyActingBattler, targets, this);

            foreach (GameObject target in targetObjects)
                DestroyImmediate(target);
       
        }
        else
        {
            List<EnemyBattler> chosenTargets = ((SupportSkill)chosenSkill).ChooseTarget((EnemyBattler)currentlyActingBattler, this);

            if(chosenSkill.targetType == TargetType.All)
                targetObjects.Add(Instantiate(supportTargetEnemy550, enemyAOELoc) as GameObject);
            else
            {
                foreach(EnemyBattler battler in chosenTargets)
                    targetObjects.Add(Instantiate(supportTargetEnemy100, battler.gameObject.transform) as GameObject);
            }

            List<Battler> targets = new List<Battler>();
            foreach (EnemyBattler battler in chosenTargets)
                targets.Add((Battler)battler);

            SEManager.instance.PlaySE("enemyTurn");
            DisplaySkillMessage(chosenSkill);
            yield return new WaitForSeconds(1.7f);

            ((SupportSkill)chosenSkill).UseSkill(currentlyActingBattler, targets, this);

            foreach (GameObject target in targetObjects)
                DestroyImmediate(target);
       
        }
    }

    public IEnumerator FinishEnemyTurn(int maxAdditionalAnimations, float soundEffectHitDelay)
    {
        currentlyActingBattler.CountDownEffects();
        
        currentlyActingBattlers.Remove(currentlyActingBattler);
        currentlyActingBattler = null;

        yield return new WaitForSeconds(1.7f + soundEffectHitDelay);

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

        yield return new WaitForSeconds(1.6f * maxAdditionalAnimations);

        DestroyImmediate(currentPointer);
        ClearMessageBox();

        if(IsPlayerVictory())
            StartCoroutine(PlayerVictory());
        else if(IsPlayerPartyFled())
            StartCoroutine(PlayerFlee());
        else if(IsPlayerDefeat())
            StartCoroutine(PlayerDefeat());
        else
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
        foreach(PlayerBattler battler in startingPlayerBattlers)
        {
            if(battler.isKO)
                return true;
        }

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
        Destroy(hotkeyManager.GetComponent<TacticsButtonSelectedHotkeys>());
        Destroy(hotkeyManager.GetComponent<ItemsButtonSelectedHotkeys>());
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
        foreach(PlayerBattler battler in startingPlayerBattlers)
        {
            if(battler.HasFled())
                return false;
            else if(!battler.isKO)
                return false;
                
        }  
        return true;
    }

    private bool IsPlayerPartyFled()
    {
        bool fled = true;

        foreach(PlayerBattler battler in startingPlayerBattlers)
        {
            if(!battler.HasFled() && !battler.isKO)
            {
                fled = false;
                break;
            }

        }
        return fled;
    }

    private bool IsEnemyPartyFled()
    {
        bool fled = true;

        foreach(EnemyBattler battler in startingEnemyBattlers)
        {
            if(!battler.HasFled())
            {
                fled = false;
                break;
            }
        }
        return fled;
    }

    private IEnumerator PlayerVictory()
    {
        StartCoroutine(BGMManager.instance.FadeOutBGM(1.0f));
        AdjustCharactersAfterBattle();
        Dictionary<Item , int> itemDrops = DetermineItemDrops();

        foreach(Item item in itemDrops.Keys)
            inventory.addItem(item.itemID, itemDrops[item]);
        
        SetupVictoryPanel(itemDrops);

        yield return new WaitForSeconds(1.1f);

        BGMManager.instance.PlayBGM("openingThePathWithTheseHands");
        victoryPanel.SetActive(true);

        yield return new WaitForSeconds(1.2f);
        AnimateEXP();

        yield return new WaitForSeconds(5f);

        victoryPanel.transform.GetChild(8).gameObject.SetActive(true);
    }

    private Dictionary<Item, int> DetermineItemDrops()
    {
        Dictionary<Item, int> itemDrops = new Dictionary<Item, int>();

        foreach(EnemyBattler battler in startingEnemyBattlers)
        {
            if(!battler.HasFled())
            {
                Dictionary<Item, int> items = battler.DropItems();

                foreach(Item item in items.Keys)
                {
                    if(itemDrops.ContainsKey(item))
                        itemDrops[item] = itemDrops[item]+ items[item];
                    else 
                        itemDrops.Add(item, 1);
                }
            }
        }

        return itemDrops;
    }

    private void SetupVictoryPanel(Dictionary<Item, int> itemDrops)
    {
        int xPos = 250;
        int yPos = -75;

        foreach(PlayerBattler battler in startingPlayerBattlers)
        {
            Character c = battler.character;

            GameObject slot = Instantiate(playerSlotVictoryPanel, victoryPanel.transform.GetChild(5)) as GameObject;
            slot.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, yPos);

            string playerClassString = battler.playerClass.ToString().Split('(')[0];

            slot.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = c.characterImage;
            slot.transform.GetChild(3).gameObject.GetComponent<TMP_Text>().text = battler.battlerName;
            slot.transform.GetChild(4).gameObject.GetComponent<TMP_Text>().text = "Lvl " + c.preBattleLevel + " " + playerClassString;
            slot.transform.GetChild(6).GetChild(3).gameObject.GetComponent<TMP_Text>().text = "" + c.hp + "/" + c.mhp;

            slot.transform.GetChild(6).gameObject.GetComponent<Slider>().maxValue = c.mhp;
            slot.transform.GetChild(6).gameObject.GetComponent<Slider>().minValue = 0;

            const int sliderWidth = 330;
            double hpSliderPosition = sliderWidth - ((double)c.hp / (double)c.mhp) * sliderWidth;
            
            slot.transform.GetChild(6).GetChild(2).gameObject.GetComponent<RectTransform>().offsetMin = new Vector2((float)hpSliderPosition, 0.0f);

            slot.transform.GetChild(9).gameObject.GetComponent<TMP_Text>().text = "" + c.preBattleExp + " (+" + (c.exp - c.preBattleExp) + ")"; 
            slot.transform.GetChild(10).gameObject.GetComponent<TMP_Text>().text = "" + LevelingData.expLevelRequirements[c.preBattleLevel];

            if(xPos == 250)
                xPos += 550;
            else 
            {
                xPos = 250;
                yPos = -325;
            }
        }

        xPos= 0;

        foreach(Item item in itemDrops.Keys)
        {
            GameObject itemIcon = Instantiate(itemVictoryIcon, victoryPanel.transform.GetChild(7)) as GameObject;
            itemIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(xPos, 0);

            itemIcon.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = item.itemImage;
            itemIcon.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TMP_Text>().text = "" + itemDrops[item];

            xPos += 110;
        }
    }

    private void AnimateEXP()
    {
        for(int i = 0; i< startingPlayerBattlers.Count; i++)
            StartCoroutine(AnimateEXPText(victoryPanel.transform.GetChild(5).GetChild(i).gameObject, startingPlayerBattlers[i].character));
    }

    private IEnumerator AnimateEXPText(GameObject playerPanel, Character character)
    {
        float t = 0.0f;
        int currentLevel = character.preBattleLevel;

        while (t < 2.51f)
        {
            int exp = (int)(Mathf.Lerp(character.preBattleExp, character.exp, t/2.5f));
            int expGain = (int)Math.Ceiling((Mathf.Lerp(character.exp-character.preBattleExp, 0, t/2.5f)));
            
            playerPanel.transform.GetChild(9).gameObject.GetComponent<TMP_Text>().text = "" + exp + " (+" + expGain + ")"; 
            
            if(exp >= LevelingData.expLevelRequirements[currentLevel])//Level Up!
            {
                currentLevel++;
                string playerClassString = character.charClass.ToString().Split('(')[0];
                playerPanel.transform.GetChild(4).gameObject.GetComponent<TMP_Text>().text = "Lvl " + currentLevel + " " + playerClassString;
                playerPanel.transform.GetChild(10).gameObject.GetComponent<TMP_Text>().text = "" + LevelingData.expLevelRequirements[currentLevel];

                playerPanel.transform.GetChild(2).GetChild(0).gameObject.SetActive(true);
            }

            t+=Time.deltaTime;
            yield return null;
        }
        
        playerPanel.transform.GetChild(9).gameObject.GetComponent<TMP_Text>().text = "" + character.exp;

    }

    // Changes the characters stats, like exp and hp, after a battle has finished.
    private void AdjustCharactersAfterBattle(bool fled = false)
    {
        int expGain = 0;

        if(!fled)
        {
            foreach(EnemyBattler battler in startingEnemyBattlers)
            expGain += battler.exp;
        }

        foreach(PlayerBattler battler in startingPlayerBattlers)
        {
            Character c = battler.character;

            c.preBattleExp = battler.character.exp;
            c.preBattleLevel = battler.character.level;
            c.exp += expGain;

            if(battler.isKO)
                c.hp = 1;
            else if(battler.hp > c.mhp)
                c.hp = c.mhp;
            else 
                c.hp = battler.hp;

            while(LevelingData.DetermineLevel(c.exp) > c.level)
            {
                levelUps.Add(c);
                levelUpNewLevels.Add(c.level+1);
                c.LevelUp();
            }
                
        }
    }

    private void SetupLevelUpPanel(Character c, int newLevel)
    {
        if(levelUpPanel.transform.GetChild(21).childCount > 0)
            DestroyImmediate(levelUpPanel.transform.GetChild(21).GetChild(0).gameObject);
        if(levelUpPanel.transform.GetChild(22).childCount > 0)
            DestroyImmediate(levelUpPanel.transform.GetChild(22).GetChild(0).gameObject);

        levelUpPanel.transform.GetChild(23).gameObject.SetActive(false);

        levelUpPanel.transform.GetChild(4).gameObject.GetComponent<Image>().sprite = c.characterImage;
        levelUpPanel.transform.GetChild(5).gameObject.GetComponent<TMP_Text>().text = "" + c.characterName + " has leveled up to Level " + newLevel + "!";

        levelUpPanel.transform.GetChild(7).gameObject.GetComponent<TMP_Text>().text = "" + (int)(c.mhp/1.1) + " -> " + (int)c.mhp;
        levelUpPanel.transform.GetChild(9).gameObject.GetComponent<TMP_Text>().text = "" + (int)(c.str/1.1) + " -> " + (int)c.str;
        levelUpPanel.transform.GetChild(11).gameObject.GetComponent<TMP_Text>().text = "" + (int)(c.wil/1.1) + " -> " + (int)c.wil;
        levelUpPanel.transform.GetChild(13).gameObject.GetComponent<TMP_Text>().text = "" + c.def * 100 + "%";
        levelUpPanel.transform.GetChild(15).gameObject.GetComponent<TMP_Text>().text = "" + c.res * 100 + "%";
        levelUpPanel.transform.GetChild(17).gameObject.GetComponent<TMP_Text>().text = "" + (int)(c.ini/1.1) + " -> " + (int)c.ini;
        levelUpPanel.transform.GetChild(19).gameObject.GetComponent<TMP_Text>().text = "" + (c.crt-0.01)* 100 + "% -> " + c.crt*100 + "%";

        List<Skill> availSkills = c.GetNextUnlockableSkills();

        if(availSkills.Count == 2)
        {
            GameObject skillButton = Instantiate(LevelupSkillUnlockButton, levelUpPanel.transform.GetChild(21)) as GameObject;
            skillButton.transform.GetChild(1).gameObject.GetComponent<SkillButton>().InitializeLevelupButton(availSkills[0], this);

            GameObject skillButton2 = Instantiate(LevelupSkillUnlockButton, levelUpPanel.transform.GetChild(22)) as GameObject;
            skillButton2.transform.GetChild(1).gameObject.GetComponent<SkillButton>().InitializeLevelupButton(availSkills[1], this);     
        }
        else if(availSkills.Count == 1)
        {
            GameObject skillButton = Instantiate(LevelupSkillUnlockButton, levelUpPanel.transform.GetChild(21)) as GameObject;
            skillButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(150f, 0.0f);
            skillButton.transform.GetChild(1).gameObject.GetComponent<SkillButton>().InitializeLevelupButton(availSkills[0], this);
        }
        else 
        {
            levelUpPanel.transform.GetChild(23).gameObject.SetActive(true);
        }
    }

    public void VictoryContinueButtonPress()
    {
        if(selectedLevelupSkill != null)
            levelUps[levelUpsLoc-1].skills.Add(selectedLevelupSkill);
        selectedLevelupSkill = null;

        if(levelUpsLoc < levelUps.Count)
        {
            SetupLevelUpPanel(levelUps[levelUpsLoc], levelUpNewLevels[levelUpsLoc]);

            victoryPanel.SetActive(false);
            levelUpPanel.SetActive(true);

            SEManager.instance.PlaySE("levelup");

            levelUpsLoc++;
        }
        else 
        {
            SEManager.instance.PlaySE("buttonClick");
            StartCoroutine(ExitBattle());
        }
    }

    private IEnumerator ExitBattle()
    {
        StartCoroutine(BGMManager.instance.FadeOutBGM(2f));
        fadeOutToBlack.SetActive(true);

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(4);
    }

    private IEnumerator PlayerDefeat()
    {
        StartCoroutine(BGMManager.instance.FadeOutBGM(2f));
        fadeOutToBlack.SetActive(true);
        
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("sceneDefeat");
    }

    private IEnumerator PlayerFlee()
    {
        StartCoroutine(BGMManager.instance.FadeOutBGM(2f));
        fadeOutToBlack.SetActive(true);

        AdjustCharactersAfterBattle(true);
        
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("sceneTutorialLevel");
    }

    internal class BattlerAPComparator : IComparer<Battler>
    {
        public int Compare(Battler a, Battler b)
        {
            return a.ap.CompareTo(b.ap);
        }
    }

    public bool IsButtonSkillsPressed()
    {
        return buttonSkillsPressed;
    }

    public bool IsButtonTacticsPressed()
    {
        return buttonTacticsPressed;
    }

       public bool IsButtonItemsPressed()
    {
        return buttonItemsPressed;
    }


}
