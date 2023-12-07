using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeterTalkEnd : NPCEndAction
{
    public override void DoAction()
    {
        PartyController.instance.addCharacter(3);


    }
}
