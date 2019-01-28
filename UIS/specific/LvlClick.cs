using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvlClick : UI, UIAction
{
	void UIAction.doAction(MParams par){
		if (par.phase == TouchPhase.Ended && GManager.GSState == GS.N) {
			GManager.GSState = GS.I;
			ToggleStat.onStat = false;
			ToggleStat.flip ();
		}
	}
}
