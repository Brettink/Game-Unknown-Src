using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseEdit : UI, UIAction {

	void UIAction.doAction(MParams par){
		if (par.phase == TouchPhase.Ended) {
			GManager.GSState = GS.N;
		}
	}
}
