using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCChat : UI, UIAction {
	Controller controller;

	new void Start(){
		base.Start ();
		controller = GetComponent<Controller> ();
	}

	void UIAction.doAction(MParams par){
		if (par.phase == TouchPhase.Began){
			if (GManager.GSState == GS.N) {
				GManager.playerLocation.gameObject.SendMessage ("endMove");
				GManager.playerLocation.gameObject.SendMessage ("FaceTo", transform);
				gameObject.SendMessage ("FaceTo", GManager.playerLocation);
				ChatController.setSeq (transform.GetChild (0).gameObject, controller.type, controller.seqId);
				GManager.GSState = GS.C;
			}
		}
	}
}
