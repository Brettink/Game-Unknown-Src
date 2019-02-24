using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsShow : MonoBehaviour
{
    STATE state = STATE.NONE;
    bool dir = false;
    float hide = 720f;
    float showR = 0f;

    public void Show(bool dir) {
        state = STATE.LERPING;
        this.dir = dir;
    }

    bool moveAndCheck(GameObject check, float posTo, bool type) {
        Vector3 pos = check.transform.localPosition;
        if (Math.Abs(pos.y - posTo) > 0.01f) {
            pos.y = Mathf.Lerp(pos.y, posTo, .9f);
            check.transform.localPosition = pos;
            return true;
        }
        pos.y = posTo;
        check.transform.localPosition = pos;
        return false;
    }

    // Update is called once per frame
    void Update() {
        switch (state) {
            case STATE.LERPING: {
                float posTo = (dir) ? showR : hide;
                if (!moveAndCheck(this.gameObject, posTo, true)) {
                    state = STATE.NONE;
                    if (!dir) {
                        GManager.GSState = GS.N;
                    }
                }
                break;
            }
        }
    }
}
