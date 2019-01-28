using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIContMove : UI, UIAction {

	void UIAction.doAction(MParams par){
		if (GManager.GSState != GS.E) {
			if (par.phase == TouchPhase.Moved || par.phase == TouchPhase.Began) {
				GManager.playerLocation.gameObject.SendMessage ("contToMove", true);
			}
		}
	}
}
