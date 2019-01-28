using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIZoom : UIScroll, UIAction {

	void onMove(float curVal){
		float x = GManager.map (curVal, maxPos, minPos, -2.5f, -8f);
		float y = GManager.map (curVal, maxPos, minPos, 1f, 7f);
		float z = GManager.map (curVal, maxPos, minPos, -2.5f, -8f);
		CameraController.followVector = new Vector3 (x, y, z);
	}

	void UIAction.doAction(MParams par){
		Vector3 point = transform.InverseTransformPoint (par.hit.point);
		float getVal = (dir == DIR.Horizontal) ? point.x : point.y;
		getVal = GManager.clampF (getVal, minPos, maxPos);
		setVal (getVal);
	}
}
