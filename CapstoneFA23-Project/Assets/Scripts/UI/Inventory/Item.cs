using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]

public class Item : ScriptableObject
{
    public int itemID, category, value;
    public Sprite itemImage;
    public string itemName, description;
    
}
