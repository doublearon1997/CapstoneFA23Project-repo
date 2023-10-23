using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEncounter", menuName = "Encounter")]
public class Encounter : ScriptableObject
{
    public List<GameObject> enemies = new List<GameObject>();

    //We can include more information in these, maybe background, bgm, etc.


}
