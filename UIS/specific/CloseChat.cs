using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseChat : UI, UIAction {

	void UIAction.doAction(MParams par){
		if (par.phase == TouchPhase.Ended){
			ChatController.Next (true);
		}
	}
}
