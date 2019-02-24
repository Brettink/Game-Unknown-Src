using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIZoom : UIScroll, UIAction {

	void onMove(float curVal){
        CameraController.followVector.y = GManager.map(curVal, maxPos, minPos, .9f, 5f);
        CameraController.baseR = GManager.map(curVal, maxPos, minPos, -2f, -10f);
        float rX = Mathf.Clamp(GManager.map(curVal, maxPos, minPos, 0f, 35f), 0f, 25f);
        CameraController.selfRot.x = rX;
    }

	void UIAction.doAction(MParams par){
		Vector3 point = transform.InverseTransformPoint (par.hit.point);
		float getVal = (dir == DIR.Horizontal) ? point.x : point.y;
		getVal = GManager.clampF (getVal, minPos, maxPos);
		setVal (getVal);
	}
}
