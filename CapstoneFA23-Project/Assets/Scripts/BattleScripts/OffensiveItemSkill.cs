using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffensiveItemSkill : OffensiveSkill
{
    public int quantity;
    public ConsumableItem item;

    public void Initialize(ConsumableItem item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
        this.isOffensive = true;

        this.portrait65 = item.itemImage;
        this.portrait100 = item.itemImage;
        this.portrait50 = item.itemImage;

        this.skillName = item.itemName;
        this.effects = item.effects;

        this.apMod = 1.15;
        this.dmgMod = 0;

        this.targetType = TargetType.Single;
        this.powerType = PowerType.Physical;

        this.soundEffect = SEManager.instance.GetAudioClip("useItem");
        this.hitSoundEffect = item.hitSoundEffect;
        this.soundEffectHitDelay = item.soundEffectHitDelay;
    }
}
