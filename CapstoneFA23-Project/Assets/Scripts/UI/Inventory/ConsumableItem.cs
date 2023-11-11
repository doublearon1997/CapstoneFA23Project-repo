using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Item", menuName = "ConsumableItem")]
public class ConsumableItem : Item
{
    public List<Effect> effects;
    
}
