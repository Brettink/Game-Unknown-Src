using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitOption : UI, UIAction, UIOptionAction
{
    void UIAction.doAction(MParams par) {
        if (par.phase == TouchPhase.Ended) {
            YesNo.self.gameObject.SetActive(true);
            YesNo.set(GManager.self.UIOptions, gameObject, "Quit?");
            GManager.self.UIOptions.SetActive(false);
        }
    }

    public void MyAction() {
        Application.Quit();
    }
}
