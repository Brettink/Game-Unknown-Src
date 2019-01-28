using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseInvent : UI, UIAction {

	void UIAction.doAction(MParams par){
		if (par.phase == TouchPhase.Ended){
			GManager.self.UIInventory.SendMessage ("show", false);
		}
	}
}
