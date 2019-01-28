using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleView : UIToggle, UIAction {
	public bool doWhat = false;
	void UIAction.doAction(MParams par){
		if (par.phase == TouchPhase.Ended) {
			GManager.sideView = doWhat;
			transform.parent.SendMessage ("onToggleAction", base.selfGet ());
		}
	}
}
