using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : UI, UIAction {

	void UIAction.doAction(MParams par){
		if (par.phase == TouchPhase.Began) {
			GManager.playerLocation.gameObject.SendMessage ("Jump");
		}
	}
}
