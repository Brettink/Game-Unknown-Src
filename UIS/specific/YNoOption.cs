using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YNoOption : UI, UIAction
{
    void UIAction.doAction(MParams par) {
        if (par.phase == TouchPhase.Ended) {
            YesNo.parent.SetActive(true);
            YesNo.self.gameObject.SetActive(false);
        }
    }
}
