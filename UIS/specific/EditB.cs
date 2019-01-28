using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditB : UI, UIAction {

	void UIAction.doAction(MParams par){
		if (par.phase == TouchPhase.Ended) {
			GManager.playerLocation.gameObject.SendMessage ("endMove");
			GManager.GSState = GS.E;
		}
	}

}
