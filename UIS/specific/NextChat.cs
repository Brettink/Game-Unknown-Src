using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextChat : UI, UIAction {

	void UIAction.doAction(MParams par){
		if (par.phase == TouchPhase.Ended){
			ChatController.Next ();
		}
	}

}
