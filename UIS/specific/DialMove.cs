using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialMove : UI, UIAction {

	Quaternion rotMove = Quaternion.Euler (new Vector3(1f, 45f, 1f));

	void UIAction.doAction(MParams par){
		RaycastHit hit = par.hit;
		TouchPhase phase = par.phase;
		Vector3 adjustVect = self.transform.InverseTransformPoint (hit.point);
		adjustVect.z = adjustVect.y;
		adjustVect.y = 0f;
		adjustVect *= GManager.speed;

		GameObject toMove;
		if (GManager.GSState == GS.E) {
			toMove = GManager.playerFollowL.gameObject;
			adjustVect *= 10f;
		} else {
			GManager.playerAnim.SetBool ("walking", true);
            rotMove.eulerAngles = Vector3.up * CameraController.rotation;
			adjustVect = rotMove * adjustVect;
            toMove = GManager.playerLocation.gameObject;
		}

		if (phase == TouchPhase.Ended) {
			toMove.SendMessage ("endMove");
		} else {
			toMove.SendMessage ("move", adjustVect);
		}
	}
}
