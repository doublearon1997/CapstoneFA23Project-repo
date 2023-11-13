using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]

public class Item : ScriptableObject
{
    public enum ItemCategory {Consumable, Equippable, Other };

    public int itemID, value;
    public ItemCategory category;
    public Sprite itemImage;
    public string itemName, description;
    
}
