using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSkills : MonoBehaviour
{
    [SerializeField] private List<ScriptableObject> listSkills;

    [SerializeField] public Dictionary<string, ScriptableObject> skills;

    private void Awake()
    {
       
    }
}
