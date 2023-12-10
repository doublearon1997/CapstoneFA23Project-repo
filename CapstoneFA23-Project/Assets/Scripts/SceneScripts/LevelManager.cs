using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance = null;
    public GameObject player;
    public List<GameObject> enemyList;
    public static List<bool> enemyActiveList;
    public static bool inBattle = false;
    public static int InDungeon = -2;
    public static int currentEnemy;
    public static Vector2 playerPosition = new Vector2(0.0f, 0.0f);
    public static Vector2 playerLoadPosition = new Vector2(-9999, -9999);
    public static Encounter currentEncounter;
    public static string currentScene;
    public AudioClip bgm;

    public static float bgmSaveTime;

    public static LevelManager levelManagerInstance;

    // Start is called before the first frame update
    void Start()
    {
        levelManagerInstance = this;

        if(currentScene == null || currentScene != levelManagerInstance.gameObject.scene.name)
        {
            currentScene = gameObject.scene.name;
            enemyActiveList = new List<bool>();
            bgmSaveTime = -1;

            for(int i = 0; i < levelManagerInstance.enemyList.Count; i++)
                enemyActiveList.Add(true);
        }

        if (inBattle == true)
        {
            if (currentEncounter.result == Encounter.EncounterResult.PlayerVictory)
            {
                enemyActiveList[currentEnemy] = false;
            }
            else if (currentEncounter.result == Encounter.EncounterResult.PlayerFlee)
                StartCoroutine(levelManagerInstance.enemyList[currentEnemy].GetComponent<EnemyAI>().WaitAfterFlee());
                
            player.transform.position = playerPosition;
            inBattle = false;
        }
        else if(playerLoadPosition != new Vector2(-9999, -9999))
        {
            player.transform.position = new Vector2(playerLoadPosition.x, playerLoadPosition.y);
            playerLoadPosition = new Vector2(-9999, -9999);
        }
        for(int i = 0; i<enemyActiveList.Count; i++)
        {
            if(enemyActiveList[i] == false)
            {
                levelManagerInstance.enemyList[i].SetActive(false);
            }
        }

        if(bgmSaveTime != -1)
            BGMManager.instance.PlayBGM(bgm, bgmSaveTime);
        else 
            BGMManager.instance.PlayBGM(bgm);
    }

    public void UpdatePlayerPosition()
    {
        playerPosition = player.transform.position;
    }

    public static void SetEnemy(GameObject enemy)
    {
        currentEnemy = levelManagerInstance.enemyList.IndexOf(enemy);
        inBattle = true;
    }

   


}
