using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenOptions : UI, UIAction
{
    void UIAction.doAction(MParams par) {
        if (par.phase == TouchPhase.Ended) {
            GManager.GSState = GS.O;
        }
    }
}
