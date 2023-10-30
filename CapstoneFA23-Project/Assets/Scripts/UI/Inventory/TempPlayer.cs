using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System;

public class TempPlayer : MonoBehaviour
{
    public InventoryController ic;

    public void addItem()
    {
        ic.removeItem(5,2);
    }
}
