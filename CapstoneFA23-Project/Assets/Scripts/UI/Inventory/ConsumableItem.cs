using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Item", menuName = "ConsumableItem")]
public class ConsumableItem : Item
{
    public List<Effect> effects;

    public bool isSupportItem = true;
    
    public AudioClip hitSoundEffect;

    public float soundEffectHitDelay;
}
