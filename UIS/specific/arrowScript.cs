using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowScript : UI, UIAction {

	void UIAction.doAction(MParams par){
		if (par.phase == TouchPhase.Began && GManager.GSState == GS.E){
			GManager.moveTo = transform.parent.position.y;
		}
	}

}
