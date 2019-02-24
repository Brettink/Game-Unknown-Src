using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseOptions : UI, UIAction {
    void UIAction.doAction(MParams par) {
        if (par.phase == TouchPhase.Ended) {
            GManager.self.UIOptions.SendMessage("Show", false);
        }
    }
}
