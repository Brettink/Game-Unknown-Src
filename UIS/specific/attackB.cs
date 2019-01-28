using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackB : UI, UIAction {

	public Sprite[] sprites;

	void UIAction.doAction(MParams par){
		if (GManager.GSState != GS.E) {
			if (par.phase != TouchPhase.Ended) {
				GManager.playerAnim.SetBool ("melee", true);
			} else {
				GManager.playerAnim.SetBool ("melee", false);
			}
			Vector3 dist = self.transform.InverseTransformPoint (par.hit.point);

			if (dist.magnitude > 0.48f){
				GManager.playerAnim.SetBool ("melee", false);
			}
		}
	}

	void OnSheath(bool sheath){
		selfSprite.sprite = (Controller.hasWeapon)?(!sheath)?sprites[1]:sprites[0]:sprites[0];
	}
}
