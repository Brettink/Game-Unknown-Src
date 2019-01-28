using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Linq;
using UnityEngine.SocialPlatforms;
using System.Xml.Schema;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

	public static Vector3 selfRot = new Vector3(22.5f, 45f, 0f);
	public static Vector3 followVector = new Vector3 (-5f, 3.6f, -5f);

	void snapToGrid(Vector3 point, out Vector3 pos){
		pos = new Vector3 ();
		pos.x = Mathf.Round (point.x - this.transform.position.x);
		pos.z = Mathf.Round (point.z - this.transform.position.z);
		pos.y = 0.3f;
	}

	void FixedUpdate () {
		Transform playerLocation = (GManager.GSState == GS.E)?GManager.playerFollowL:GManager.playerLocation;
		Vector3 self = transform.position;
		Vector3 selfRot = transform.rotation.eulerAngles;
		Vector3 lerpTo;
		Vector3 rotateTo;
		if (GManager.GSState == GS.E) {
			lerpTo = new Vector3 (0f, 0f, 0f);
			rotateTo = playerLocation.rotation.eulerAngles;
		} else {
			lerpTo = followVector;
			rotateTo = CameraController.selfRot;
			Vector3 lookAtPos = playerLocation.position;
			lookAtPos.y += 1f;
			transform.LookAt (lookAtPos);
		}
		self.x = Mathf.Lerp (self.x, playerLocation.position.x + lerpTo.x, .2f);
		self.y = Mathf.Lerp (self.y, playerLocation.position.y + lerpTo.y, .2f);
		self.z = Mathf.Lerp (self.z, playerLocation.position.z + lerpTo.z, .2f);
		if (GManager.GSState == GS.E) {
			selfRot.x = Mathf.Lerp (selfRot.x, rotateTo.x, .2f);
			selfRot.y = Mathf.Lerp (selfRot.y, rotateTo.y, .2f);
			selfRot.z = Mathf.Lerp (selfRot.z, rotateTo.z, .2f);
			transform.rotation = Quaternion.Euler (selfRot);
		}
		transform.position = self;
	}
		
}
