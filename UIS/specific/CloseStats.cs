using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseStats : UI, UIAction {
	void UIAction.doAction(MParams par){
		if (par.phase == TouchPhase.Ended) {
			SendMessage ("Hide");
		}
	}
}
