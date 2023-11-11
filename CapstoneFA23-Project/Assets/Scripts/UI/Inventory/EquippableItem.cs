using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equippable Item", menuName = "EquippableItem")]
public class EquippableItem : Item
{
    public double hpMod = 1.0, strMod = 1.0, wilMod = 1.0, defMod = 0, resMod = 0, iniMod = 1.0, crtMod = 0;

}

