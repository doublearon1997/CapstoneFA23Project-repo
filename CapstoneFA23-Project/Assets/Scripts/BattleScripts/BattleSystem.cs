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
