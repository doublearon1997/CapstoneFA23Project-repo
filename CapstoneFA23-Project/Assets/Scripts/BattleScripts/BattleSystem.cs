using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Reflection;
using System.Configuration;
using System.Collections.Specialized;

public enum BattleState { Start, PlayerInputTurn, EnemyTurn,  PlayerNoInputTurn, DetermineNext, Win, Lose}

public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    public List<GameObject> startingPlayerBattlerGOs = new List<GameObject>();
    public List<GameObject> startingEnemyBattlerGOs = new List<GameObject>();

    public List<PlayerBattler> playerBattlers;
    public List<EnemyBattler> enemyBattlers;

    public Transform[] playerSpawns;
    public Transform[] enemySpawns;

    public GameObject panelPlayerActions;

    public GameObject damageTextPopup;


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
    public Button buttonAttack;

    public SEManager seManager;


    public void Start()
    {
        state = BattleState.Start;

        currentlyActingBattlers = new SortedSet<Battler>(new BattlerAPComparator());

        StartCoroutine(SetupBattle());
        StartCoroutine(DetermineNextBattler());
        SetTurnOrderPanel();
    }

    IEnumerator SetupBattle()
    {
        this.playerBattlers = new List<PlayerBattler>();
        this.enemyBattlers = new List<EnemyBattler>();

        for(int i = 0; i < startingPlayerBattlerGOs.Count; i++)
        {
            GameObject playerGO = Instantiate(startingPlayerBattlerGOs[i], playerSpawns[i]);
            playerBattlers.Add(playerGO.GetComponent<PlayerBattler>());

            playerGO.GetComponent<PlayerBattler>().ap = UnityEngine.Random.Range(0, 20000);

        }

        for (int i = 0; i < startingEnemyBattlerGOs.Count; i++)
        {
            GameObject enemyGO = Instantiate(startingEnemyBattlerGOs[i], enemySpawns[i]);
            enemyBattlers.Add(enemyGO.GetComponent<EnemyBattler>());

            enemyGO.GetComponent<EnemyBattler>().ap = UnityEngine.Random.Range(0, 20000);

        }

        SetupPartyOverviewPanel();
        SetupEnemyOverviewPanel();

        yield return new WaitForSeconds(2f);

        state = BattleState.DetermineNext;
    }

    //This method profilerates the overview panel with information about the party's battlers. 
    public void SetupPartyOverviewPanel()
    {
        for(int i = 0; i < playerBattlers.Count; i++)
        {
            PlayerBattler battler = playerBattlers[i];
            battler.SetPartyPosition(i);
            
            playerNameTexts[i].text = battler.battlerName;
            playerClassLevelTexts[i].text = " Lvl " + battler.level + " " + battler.playerClass;
            playerHPTexts[i].text = "" + battler.hp +"/" + battler.mhp;

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
            enemyHPTexts[i].text = "" + battler.hp +"/" + battler.mhp;

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

        for (int i=0; i<turnOrder.Count; i++)
        {
            GameObject portrait;
            Battler battler = turnOrder[i];
            
            if(battler.isPlayer)
            {
                if(battler == currentlyActingBattler)
                    portrait = Instantiate(portraitTurnOrderCurrentPlayer, panelTurnOrder.transform) as GameObject;
                else
                    portrait = Instantiate(portraitTurnOrderPlayer, panelTurnOrder.transform) as GameObject;
            }
            else
            {
                if(battler == currentlyActingBattler)
                    portrait = Instantiate(portraitTurnOrderCurrentEnemy, panelTurnOrder.transform) as GameObject;
                else
                    portrait = Instantiate(portraitTurnOrderEnemy, panelTurnOrder.transform) as GameObject;
            }

            portrait.GetComponent<Image>().sprite = battler.portrait60;
            portrait.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, i * -75.0f - 46 - bigPortraitCushion);

            if (battler == currentlyActingBattler)
                bigPortraitCushion = 7;
            
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
    private List<Battler> DetermineCurrentTurnOrder()
    {
        List<Battler> battlerTurnOrder = new List<Battler>();

        Dictionary<Battler, int> needToCalculate = new Dictionary<Battler, int>();
        List<Battler> doneCalculated = new List<Battler>();
        SortedSet<Battler> futureCurrentlyActingBattlers = new SortedSet<Battler>(new BattlerAPComparator());

        //Add battlers to calculate to a dictionary to prevent actually adding ap to the real battlers.
        foreach(PlayerBattler battler in playerBattlers)
        {
            if(!currentlyActingBattlers.Contains(battler))
                needToCalculate.Add(battler, battler.ap);
        }

        foreach(EnemyBattler battler in enemyBattlers)
        {
            if (!currentlyActingBattlers.Contains(battler))
                needToCalculate.Add(battler, battler.ap);
        }

        // Add the battlers that don't need any future calculations.
        foreach (Battler battler in currentlyActingBattlers)
        {
            battlerTurnOrder.Add(battler);
        }

        while (doneCalculated.Count != needToCalculate.Count)
        {
            futureCurrentlyActingBattlers.Clear();

            var keys = new List<Battler>(needToCalculate.Keys);

            for(int i = 0; i<keys.Count; i++)
            {
                Battler battler = keys[i];

                if (needToCalculate[battler] < 100000)
                {
                    needToCalculate[battler] = needToCalculate[battler] + ((int)(battler.ini * battler.apMod)); ;

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
        while(currentlyActingBattlers.Count < 1) //runs this loop until a battler has atleast 100000 AP
        {
            foreach (PlayerBattler battler in playerBattlers)
            {
                battler.ap += (int)(battler.ini * battler.apMod);
                if(battler.ap >= 100000) 
                {
                    currentlyActingBattlers.Add(battler);
                }
            }

            foreach (EnemyBattler battler in enemyBattlers)
            {
                battler.ap += (int)(battler.ini * battler.apMod);
                if(battler.ap >= 100000) 
                {
                    currentlyActingBattlers.Add(battler);
                }
            }
        }

        currentlyActingBattler = currentlyActingBattlers.First();

        yield return new WaitForSeconds(1f);

        if(currentlyActingBattler.isPlayer)
        {
            state = BattleState.PlayerInputTurn;
            StartCoroutine(PlayerTurn());
        }
        else 
        {
            state = BattleState.EnemyTurn;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator PlayerTurn()
    {
        SetTurnOrderPanel();
        seManager.PlaySE("playerTurnStart", 1);

        yield return new WaitForSeconds(0.4f);

        panelPlayerActions.SetActive(true);
    } 

    public void PlayerSelectTarget()
    {
        buttonAttack.interactable = false;
        ((PlayerBattler)currentlyActingBattler).standardAttack.ChooseTarget((PlayerBattler)currentlyActingBattler, this);  
    }

    public void PlayerActionSelected()
    {
        panelPlayerActions.SetActive(false);
        RemovePlayerTargetingHandlers();
    }

    public IEnumerator FinishPlayerTurn()
    {
        currentlyActingBattlers.Remove(currentlyActingBattler);
        currentlyActingBattler = null;

        buttonAttack.interactable = true;

        state = BattleState.DetermineNext;

        yield return new WaitForSeconds(1.0f);

        StartCoroutine(DetermineNextBattler());

    }

    //Removes all possible targeting handlers from enemybattler game objects. Called after the player chooses the target.
    public void RemovePlayerTargetingHandlers()
    {
        foreach (EnemyBattler battler in enemyBattlers)
        {
            Destroy(battler.gameObject.GetComponent<OffensiveSkill.HandlerTargetSelected>());
        }
    }

    IEnumerator EnemyTurn()
    {
        SetTurnOrderPanel();

        Skill chosenSkill = ((EnemyBattler)currentlyActingBattler).ChooseSkill();
        Battler chosenTarget = ((OffensiveSkill)chosenSkill).ChooseTarget((EnemyBattler)currentlyActingBattler, this); 
  
        ((OffensiveSkill)chosenSkill).UseSkill(currentlyActingBattler, chosenTarget, this);
   
        yield return new WaitForSeconds(1.0f);
 
        FinishEnemyTurn();
        
    } 

    public void FinishEnemyTurn()
    {
        currentlyActingBattlers.Remove(currentlyActingBattler);
        currentlyActingBattler = null;
        
        state = BattleState.DetermineNext;
        StartCoroutine(DetermineNextBattler());

    }

  

    internal class BattlerAPComparator : IComparer<Battler>
    {
        public int Compare(Battler a, Battler b)
        {
            return a.ap.CompareTo(b.ap);
        }
    }


}
