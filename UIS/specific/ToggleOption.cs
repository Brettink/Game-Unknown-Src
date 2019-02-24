using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleOption : UI, UIAction
{
    public Sprite off, on;
    private bool selfOn = false;

    new public void Start() {
        base.Start();
        selfSprite.sprite = off;
    }

    void UIAction.doAction(MParams par) {
        if (par.phase == TouchPhase.Ended) {
            selfOn = !selfOn;
            selfSprite.sprite = (selfOn) ? on : off;
        }
    }
}
