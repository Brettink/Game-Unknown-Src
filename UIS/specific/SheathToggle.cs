using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheathToggle : UI, UIAction {

	new Color enabled = new Color (1f, 1f, 1f);
	Color disabled = new Color (.2f, .2f, .2f);

	void UIAction.doAction(MParams par){
		if (par.phase == TouchPhase.Ended) {
			GManager.playerLocation.SendMessage ("Sheath");
		}
	}

	void OnSheath(bool sheath){
		selfSprite.color = (sheath) ? disabled : enabled;
	}

}
