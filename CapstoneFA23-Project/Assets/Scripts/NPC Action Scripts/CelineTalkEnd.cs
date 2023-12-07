using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelineTalkEnd : NPCEndAction
{
    public override void DoAction()
    {
        PartyController.instance.addCharacter(2);


    }
}
