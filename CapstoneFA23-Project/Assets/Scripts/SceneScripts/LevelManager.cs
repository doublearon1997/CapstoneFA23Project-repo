using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance = null;
    public GameObject Player;
    public List<GameObject> EnemyList = new List<GameObject>();
    public static bool InBattle = false;
    public static int InDungeon = -2;
    public static GameObject TempEnemy = null;
    public static Vector2 PlayerPosition = new Vector2(0.0f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        if (instance == this)
        {
            return;
        }

        Destroy(gameObject);

        if (InBattle == true)
        {
            if (TempEnemy != null)
            {
                TempEnemy.SetActive(false);
            }
                
            Player.transform.position = PlayerPosition;
            Debug.Log(PlayerPosition);
            InBattle = false;
        }

        if (InDungeon>=0)
        {
            InDungeon--;
            if (InDungeon == -1)
            {
                InDungeon = -2;
                Player.transform.position = PlayerPosition - new Vector2(0.0f, 2.0f); ;
            }
        }
    }

    public void UpdatePlayerPosition()
    {
        PlayerPosition = Player.transform.position;
    }

    public static void SetEnemy(GameObject InObject)
    {
        TempEnemy = InObject;
        InBattle = true;
    }
}
