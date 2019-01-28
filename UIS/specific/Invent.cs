using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invent : UI, UIAction {

	void UIAction.doAction(MParams par){
		if (par.phase == TouchPhase.Began) {
			GManager.GSState = GS.I;
			ToggleStat.onStat = true;
			ToggleStat.flip ();
		}
	}
}
