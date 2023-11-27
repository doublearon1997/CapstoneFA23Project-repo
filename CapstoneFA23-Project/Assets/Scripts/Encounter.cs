using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEncounter", menuName = "Encounter")]
public class Encounter : ScriptableObject
{
    public List<GameObject> enemies = new List<GameObject>();

    public AudioClip battleBGM;

    public Sprite battleBackgroundImage;

    public int sceneID;

    //We can include more information in these, maybe background, bgm, etc.
}
