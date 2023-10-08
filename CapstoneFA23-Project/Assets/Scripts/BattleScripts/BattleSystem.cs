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

    public GameObject containerPlayer1;
    public TMP_Text textPlayer1Name, textPlayer1ClassAndLevel, textPlayer1HP;
    public GameObject portraitPlayer1;
    public Slider sliderPlayer1HP;

    public GameObject containerPlayer2;
    public TMP_Text textPlayer2Name, textPlayer2ClassAndLevel, textPlayer2HP;
    public GameObject portraitPlayer2;
    public Slider sliderPlayer2HP;

    public GameObject containerPlayer3;
    public TMP_Text textPlayer3Name, textPlayer3ClassAndLevel, textPlayer3HP;
    public GameObject portraitPlayer3;
    public Slider sliderPlayer3HP;

    public GameObject containerPlayer4;
    public TMP_Text textPlayer4Name, textPlayer4ClassAndLevel, textPlayer4HP;
    public GameObject portraitPlayer4;
    public Slider sliderPlayer4HP;

    //Enemy Overview Panel Objects;
    public GameObject containerEnemy1;
    public TMP_Text textEnemy1Name, textEnemy1HP;
    public Slider sliderEnemy1HP;

    public GameObject containerEnemy2;
    public TMP_Text textEnemy2Name, textEnemy2HP;
    public Slider sliderEnemy2HP;

    public GameObject containerEnemy3;
    public TMP_Text textEnemy3Name, textEnemy3HP;
    public Slider sliderEnemy3HP;

    public GameObject containerEnemy4;
    public TMP_Text textEnemy4Name, textEnemy4HP;
    public Slider sliderEnemy4HP;

    public GameObject containerEnemy5;
    public TMP_Text textEnemy5Name, textEnemy5HP;
    public Slider sliderEnemy5HP;


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
            PlayerBattler battler;

            if(i == 0)
            {
                battler = playerBattlers[0];

                textPlayer1Name.text = battler.battlerName;
                textPlayer1ClassAndLevel.text = " Lvl " + battler.level + " " + battler.playerClass;
                textPlayer1HP.text = "" + battler.hp +"/" + battler.mhp;

                sliderPlayer1HP.maxValue = battler.mhp;
                sliderPlayer1HP.value = battler.hp;

                portraitPlayer1.GetComponent<Image>().sprite = battler.portrait60;

                battler.setPartyPosition(0);

                containerPlayer1.SetActive(true);
            }  
            else if (i == 1)
            {
                battler = playerBattlers[1];

                textPlayer2Name.text = battler.battlerName;
                textPlayer2ClassAndLevel.text = " Lvl " + battler.level + " " + battler.playerClass;
                textPlayer2HP.text = "" + battler.hp +"/" + battler.mhp;

                sliderPlayer2HP.maxValue = battler.mhp;
                sliderPlayer2HP.value = battler.hp;

                portraitPlayer2.GetComponent<Image>().sprite = battler.portrait60;

                battler.setPartyPosition(1);

                containerPlayer2.SetActive(true);
            }
            else if (i == 2)
            {
                battler = playerBattlers[2];

                textPlayer3Name.text = battler.battlerName;
                textPlayer3ClassAndLevel.text = " Lvl " + battler.level + " " + battler.playerClass;
                textPlayer3HP.text = "" + battler.hp +"/" + battler.mhp;

                sliderPlayer3HP.maxValue = battler.mhp;
                sliderPlayer3HP.value = battler.hp;

                portraitPlayer3.GetComponent<Image>().sprite = battler.portrait60;

                battler.setPartyPosition(2);

                containerPlayer3.SetActive(true);
            }
            else if (i == 3)
            {
                battler = playerBattlers[3];

                textPlayer4Name.text = battler.battlerName;
                textPlayer4ClassAndLevel.text = " Lvl " + battler.level + " " + battler.playerClass;
                textPlayer4HP.text = "" + battler.hp +"/" + battler.mhp;

                sliderPlayer4HP.maxValue = battler.mhp;
                sliderPlayer4HP.value = battler.hp;

                portraitPlayer4.GetComponent<Image>().sprite = battler.portrait60;

                battler.setPartyPosition(3);

                containerPlayer4.SetActive(true);
            }
            else
            {
                UnityEngine.Debug.Log("Why is there more than 4 player battlers?");
            }
        }
    }

    //This method profilerates the overview panel with information about the party's battlers. 
    public void SetupEnemyOverviewPanel()
    {
        for (int i = 0; i < enemyBattlers.Count; i++)
        {
            EnemyBattler battler = enemyBattlers[i];
            battler.setPartyPosition(i);

            if (i == 0)
            {
                textEnemy1Name.text = battler.battlerName;
                textEnemy1HP.text = "" + battler.hp + "/" + battler.mhp;

                sliderEnemy1HP.maxValue = battler.mhp;
                sliderEnemy1HP.value = battler.hp;

                containerEnemy1.SetActive(true);
            }
            else if (i == 1)
            {
                textEnemy2Name.text = battler.battlerName;
                textEnemy2HP.text = "" + battler.hp + "/" + battler.mhp;

                sliderEnemy2HP.maxValue = battler.mhp;
                sliderEnemy2HP.value = battler.hp;

                containerEnemy2.SetActive(true);
            }
            else if (i == 2)
            {
                textEnemy3Name.text = battler.battlerName;
                textEnemy3HP.text = "" + battler.hp + "/" + battler.mhp;

                sliderEnemy3HP.maxValue = battler.mhp;
                sliderEnemy3HP.value = battler.hp;

                containerEnemy3.SetActive(true);
            }
            else if (i == 3)
            {
                textEnemy4Name.text = battler.battlerName;
                textEnemy4HP.text = "" + battler.hp + "/" + battler.mhp;

                sliderEnemy4HP.maxValue = battler.mhp;
                sliderEnemy4HP.value = battler.hp;

                containerEnemy4.SetActive(true);
            }
            else if (i == 4)
            {
                textEnemy5Name.text = battler.battlerName;
                textEnemy5HP.text = "" + battler.hp + "/" + battler.mhp;

                sliderEnemy5HP.maxValue = battler.mhp;
                sliderEnemy5HP.value = battler.hp;

                containerEnemy5.SetActive(true);
            }
            else
            {
                UnityEngine.Debug.Log("Why is there more than 5 enemy battlers?");
            }

        }
    }

    // Displays the current turn order on the turn order panel.
    private void SetTurnOrderPanel()
    {
        clearTurnOrderPanel();

        List<Battler> turnOrder = DetermineCurrentTurnOrder();

        UnityEngine.Debug.Log(turnOrder.Count);

        int bigPortraitCushion = 0;

        for (int i=0; i<turnOrder.Count; i++)
        {
            GameObject portrait;
            Battler battler = turnOrder[i];
            
            if(battler.isPlayer)
            {
                if(battler == currentlyActingBattler)
                {
                    portrait = Instantiate(portraitTurnOrderCurrentPlayer, panelTurnOrder.transform) as GameObject;
                }
                else
                    portrait = Instantiate(portraitTurnOrderPlayer, panelTurnOrder.transform) as GameObject;
            }
            else
            {
                if(battler == currentlyActingBattler)
                { 
                    portrait = Instantiate(portraitTurnOrderCurrentEnemy, panelTurnOrder.transform) as GameObject;
                }
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

    //Adjusts a battler's HP slider value on the party overview panel based on their current hp. Maybe it could be animated in the future.
    public void SetPartyHPSliderValue(PlayerBattler battler)
    {
        const int sliderWidth = 408;

        double hpSliderPosition = sliderWidth - ((double)battler.hp / (double)battler.mhp) * sliderWidth;

        if(battler.getPartyPosition() == 0)
        {
            sliderPlayer1HP.transform.GetChild(1).GetComponent<RectTransform>().offsetMin = new Vector2((float)hpSliderPosition, 0.0f);
            textPlayer1HP.text = "" + battler.hp + "/" + battler.mhp;

        }
        else if (battler.getPartyPosition() == 1)
        {
            sliderPlayer2HP.transform.GetChild(1).GetComponent<RectTransform>().offsetMin = new Vector2((float)hpSliderPosition, 0.0f);
            textPlayer2HP.text = "" + battler.hp + "/" + battler.mhp;
        }
        else if (battler.getPartyPosition() == 2)
        {
            sliderPlayer3HP.transform.GetChild(1).GetComponent<RectTransform>().offsetMin = new Vector2((float)hpSliderPosition, 0.0f);
            textPlayer3HP.text = "" + battler.hp + "/" + battler.mhp;
        }
        else if (battler.getPartyPosition() == 3)
        {
            sliderPlayer4HP.transform.GetChild(1).GetComponent<RectTransform>().offsetMin = new Vector2((float)hpSliderPosition, 0.0f);
            textPlayer4HP.text = "" + battler.hp + "/" + battler.mhp;
        }
        
    }

    //Adjusts a battler's HP slider value on the enemy overview panel based on their current hp. Maybe it could be animated in the future.
    public void SetEnemyHPSliderValue(EnemyBattler battler)
    {
        const int sliderWidth = 250;

        double hpSliderPosition = sliderWidth - ((double)battler.hp / (double)battler.mhp) * sliderWidth;

        if (battler.getPartyPosition() == 0)
        {
            sliderEnemy1HP.transform.GetChild(1).GetComponent<RectTransform>().offsetMin = new Vector2((float)hpSliderPosition, 0.0f);
            textEnemy1HP.text = "" + battler.hp + "/" + battler.mhp;
        }
        else if (battler.getPartyPosition() == 1)
        {
            sliderEnemy2HP.transform.GetChild(1).GetComponent<RectTransform>().offsetMin = new Vector2((float)hpSliderPosition, 0.0f);
            textEnemy2HP.text = "" + battler.hp + "/" + battler.mhp;
        }
        else if (battler.getPartyPosition() == 2)
        {
            sliderEnemy3HP.transform.GetChild(1).GetComponent<RectTransform>().offsetMin = new Vector2((float)hpSliderPosition, 0.0f);
            textEnemy3HP.text = "" + battler.hp + "/" + battler.mhp;
        }
        else if (battler.getPartyPosition() == 3)
        {
            sliderEnemy4HP.transform.GetChild(1).GetComponent<RectTransform>().offsetMin = new Vector2((float)hpSliderPosition, 0.0f);
            textEnemy4HP.text = "" + battler.hp + "/" + battler.mhp;
        }
        else if (battler.getPartyPosition() == 4)
        {
            sliderEnemy5HP.transform.GetChild(1).GetComponent<RectTransform>().offsetMin = new Vector2((float)hpSliderPosition, 0.0f);
            textEnemy5HP.text = "" + battler.hp + "/" + battler.mhp;
        }
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
