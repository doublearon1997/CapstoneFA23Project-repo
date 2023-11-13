using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Support Item Skill", menuName = "SupportItemSkill")]
public class SupportItemSkill : SupportSkill
{
    public int quantity;
    public ConsumableItem item;

    public void Initialize(ConsumableItem item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;

        this.portrait65 = item.itemImage;
        this.portrait100 = item.itemImage;
        this.portrait50 = item.itemImage;

        this.skillName = item.itemName;
        this.effects = item.effects;

        this.apMod = 1.15;

        this.targetType = TargetType.Single;
        this.powerType = PowerType.Physical;
    }
}
