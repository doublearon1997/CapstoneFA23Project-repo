using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameObject Player;
    public List<GameObject> EnemyList = new List<GameObject>();
    public static bool InBattle = false;
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
        PlayerPosition = Player.transform.position;
    }

    void Awake()
    {
        if(InBattle == true)
        {
            if (TempEnemy != null)
            {
                TempEnemy.SetActive(false);
            }
                
            Player.transform.position = PlayerPosition;
            Debug.Log(PlayerPosition);
            InBattle = false;
        }
    }

    public static void SetEnemy(GameObject InObject)
    {
        TempEnemy = InObject;
        InBattle = true;
    }
}
