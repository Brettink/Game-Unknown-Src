using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivacyPOption : UI, UIAction, UIOptionAction
{
    void UIAction.doAction(MParams par) {
        YesNo.self.gameObject.SetActive(true);
        YesNo.set(GManager.self.UIOptions, gameObject, "Open link to Policy?");
        GManager.self.UIOptions.SetActive(false);
    }

    public void MyAction() {
        Application.OpenURL("https://docs.google.com/document/d/1omrCJjU4sPi84tqvnT5K4SsJVRJQ27rKal65rpU_9vY/edit?usp=sharing");
    }
}
