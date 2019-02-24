using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YesOption : UI, UIAction
{
    void UIAction.doAction(MParams par) {
        YesNo.parent.SetActive(true);
        YesNo.toSend.SendMessage("MyAction");
        YesNo.self.gameObject.SetActive(false);
    }
}
