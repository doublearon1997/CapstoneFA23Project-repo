using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelingData : MonoBehaviour
{
    public static Dictionary<int, int> expLevelRequirements = new Dictionary<int, int>();

    public static readonly int MAX_LEVEL = 10;

    public void Awake()
    {
        for (int level = 1; level < 10; level++)
        {
            expLevelRequirements.Add(level, (int)(10 * Math.Pow(2, level - 1) + (5 * Math.Pow(2, level - 1)) - 5));
        }
    }

    public static int DetermineLevel(int exp)
    {
        for(int i = 0; i<expLevelRequirements.Count-1; i++)
        {
            if (exp >= expLevelRequirements.Values.ToList()[i] && exp < expLevelRequirements.Values.ToList()[i + 1])
                return i + 1;
        }

        return MAX_LEVEL;
    }
}
