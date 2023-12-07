using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateData : MonoBehaviour
{
    public enum GameStateVariable {None, CelineJoin, PhantopusSlain}

    public static Dictionary<GameStateVariable, int> gameStateMap = new Dictionary<GameStateVariable, int>();

    public static bool setupDone = false;

    void Awake()
    {
        if(!setupDone)
        {
             gameStateMap.Add(GameStateVariable.CelineJoin, 0);
             gameStateMap.Add(GameStateVariable.PhantopusSlain, 0);

             setupDone = true;
        }
    }

    public static int GetGameStateValue(GameStateVariable gameStateVariable)
    {
        return gameStateMap[gameStateVariable];
    }

    public static void AddGameStateValue(GameStateVariable gameStateVariable, int value)
    {
        gameStateMap[gameStateVariable] = gameStateMap[gameStateVariable] +1;
    }
}
