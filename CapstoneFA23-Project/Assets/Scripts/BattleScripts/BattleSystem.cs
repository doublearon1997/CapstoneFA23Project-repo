using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public enum BattleState { START, PLAYERINPUTTURN, ENEMYTURN,  PLAYERNOINPUTTURN, DETERMINENEXT, WIN, LOSE}

public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    public GameObject[] playerBattlerGOs;
    public GameObject[] enemyBattlerGOs;

    public Transform[] playerSpawns;
    public Transform[] enemySpawns;

    public GameObject containerPlayer1;
    public GameObject panelPlayerActions;

    public TMP_Text textPlayer1Name;
    public TMP_Text textPlayer1ClassAndLevel;
    public TMP_Text textPlayer1HP;

    List<PlayerBattler> playerBattlers;
    List<EnemyBattler> enemyBattlers;

    SortedSet<Battler> currentlyActingBattlers;
    Battler currentlyActingBattler;


    public void Start()
    {
        state = BattleState.START;

        currentlyActingBattlers = new SortedSet<Battler>(new BattlerAPComparator());

        StartCoroutine(SetupBattle());
        StartCoroutine(DetermineNextBattler());


    }

    IEnumerator SetupBattle()
    {
        this.playerBattlers = new List<PlayerBattler>();
        this.enemyBattlers = new List<EnemyBattler>();

        GameObject playerGO  = Instantiate(playerBattlerGOs[0], playerSpawns[0]);
        playerBattlers.Add(playerGO.GetComponent<PlayerBattler>());   

        GameObject enemyGO = Instantiate(enemyBattlerGOs[0], enemySpawns[0]);
        enemyBattlers.Add(enemyGO.GetComponent<EnemyBattler>());

        SetupPartyOverviewPanel();

        yield return new WaitForSeconds(2f);

        state = BattleState.DETERMINENEXT;

    }

    public void SetupPartyOverviewPanel()
    {
        for(int i = 0; i < playerBattlers.Count; i++)
        {
            if(i == 0)
            {
                textPlayer1Name.text = playerBattlers[0].battlerName;
                textPlayer1ClassAndLevel.text = " Lvl " + playerBattlers[0].level + " " + playerBattlers[0].playerClass;
                textPlayer1HP.text = "" + playerBattlers[0].hp +"/" + playerBattlers[0].mhp;

                containerPlayer1.SetActive(true);
            }

            
         
        }
    }

    IEnumerator DetermineNextBattler()
    {
        while(currentlyActingBattlers.Count < 1)
        {
            foreach (PlayerBattler battler in playerBattlers)
            {
                battler.ap += battler.ini;
                if(battler.ap > 100000) 
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
            state = BattleState.PLAYERINPUTTURN;
            StartCoroutine(PlayerTurn());
        }
       
    }

    IEnumerator PlayerTurn()
    {
        panelPlayerActions.SetActive(true);

        yield return new WaitForSeconds(1f);
    } 


    internal class BattlerAPComparator : IComparer<Battler>
    {
        public int Compare(Battler a, Battler b)
        {
            return a.ap.CompareTo(b.ap);
        }
    }


}
