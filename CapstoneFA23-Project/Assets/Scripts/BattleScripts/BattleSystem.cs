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




    //Player Overview Panel Objects

    public GameObject containerPlayer1;
    public TMP_Text textPlayer1Name, textPlayer1ClassAndLevel, textPlayer1HP;

    public GameObject containerPlayer2;
    public TMP_Text textPlayer2Name, textPlayer2ClassAndLevel, textPlayer2HP;
  
    public GameObject containerPlayer3;
    public TMP_Text textPlayer3Name, textPlayer3ClassAndLevel, textPlayer3HP;

    public GameObject containerPlayer4;
    public TMP_Text textPlayer4Name, textPlayer4ClassAndLevel, textPlayer4HP;


    SortedSet<Battler> currentlyActingBattlers;
    public Battler currentlyActingBattler;


    //Player Action Buttons
    public Button buttonAttack;


    public void Start()
    {
        state = BattleState.Start;

        currentlyActingBattlers = new SortedSet<Battler>(new BattlerAPComparator());

        StartCoroutine(SetupBattle());
        StartCoroutine(DetermineNextBattler());
    }



    IEnumerator SetupBattle()
    {
        this.playerBattlers = new List<PlayerBattler>();
        this.enemyBattlers = new List<EnemyBattler>();

        for(int i = 0; i < startingPlayerBattlerGOs.Count; i++)
        {
            GameObject playerGO = Instantiate(startingPlayerBattlerGOs[i], playerSpawns[i]);
            playerBattlers.Add(playerGO.GetComponent<PlayerBattler>());

        }

        GameObject enemyGO = Instantiate(startingEnemyBattlerGOs[0], enemySpawns[0]);
        enemyBattlers.Add(enemyGO.GetComponent<EnemyBattler>());

        SetupPartyOverviewPanel();

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

                containerPlayer1.SetActive(true);
            }  
            else if (i == 1)
            {
                battler = playerBattlers[1];

                textPlayer2Name.text = battler.battlerName;
                textPlayer2ClassAndLevel.text = " Lvl " + battler.level + " " + battler.playerClass;
                textPlayer2HP.text = "" + battler.hp +"/" + battler.mhp;

                containerPlayer2.SetActive(true);
            }
            else if (i == 2)
            {
                battler = playerBattlers[2];

                textPlayer3Name.text = battler.battlerName;
                textPlayer3ClassAndLevel.text = " Lvl " + battler.level + " " + battler.playerClass;
                textPlayer3HP.text = "" + battler.hp +"/" + battler.mhp;

                containerPlayer3.SetActive(true);
            }
            else if (i == 3)
            {
                battler = playerBattlers[3];

                textPlayer4Name.text = battler.battlerName;
                textPlayer4ClassAndLevel.text = " Lvl " + battler.level + " " + battler.playerClass;
                textPlayer4HP.text = "" + battler.hp +"/" + battler.mhp;

                containerPlayer4.SetActive(true);
            }
            else
            {
                UnityEngine.Debug.Log("Why is there more than 4 player battlers?");
            }
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
        currentlyActingBattlers.Remove(currentlyActingBattler); 

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
        panelPlayerActions.SetActive(true);

        yield return new WaitForSeconds(1f);
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

    public void FinishPlayerTurn()
    {
        currentlyActingBattlers.Remove(currentlyActingBattler);
        currentlyActingBattler = null;

        buttonAttack.interactable = true;
        
        state = BattleState.DetermineNext;
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
        Skill chosenSkill = ((EnemyBattler)currentlyActingBattler).ChooseSkill();
        Battler chosenTarget = ((OffensiveSkill)chosenSkill).ChooseTarget((EnemyBattler)currentlyActingBattler, this); 
  
        StartCoroutine(((OffensiveSkill)chosenSkill).UseSkill(currentlyActingBattler, chosenTarget, this));
   
        yield return new WaitForSeconds(1f);
 
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
