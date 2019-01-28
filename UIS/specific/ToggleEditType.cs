using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleEditType : UIToggle, UIAction {
	public ES doWhat = ES.SS;
	void UIAction.doAction(MParams par){
		if (par.phase == TouchPhase.Ended) {
			GManager.editType = doWhat;
			transform.parent.SendMessage ("onToggleAction", base.selfGet ());
		}
	}
}
