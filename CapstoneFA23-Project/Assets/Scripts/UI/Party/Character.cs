using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]

public class Character : ScriptableObject
{
    public int characterID;
    public Sprite characterImage;
    public string characterName, description;
}
