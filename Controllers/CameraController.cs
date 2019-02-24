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
    public static float baseR = -6f;
    private float baseRotY = 45f;
    public static float rotation = 0f;
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
            self = Vector3.Lerp(self, playerLocation.position + lerpTo, .2f);
            selfRot = Vector3.Lerp(selfRot, rotateTo, .2f);
            transform.rotation = Quaternion.Euler(selfRot);
            transform.position = self;
        } else {
            float angle = 0f;
            //float angleRad = ((angle) * Mathf.PI) / 180f;
            angle = rotation-90f;
            followVector.x = Mathf.Cos(angle*Mathf.Deg2Rad) * baseR;
            followVector.z = -Mathf.Sin(angle*Mathf.Deg2Rad) * baseR;
			lerpTo = followVector;
			rotateTo = CameraController.selfRot;
            rotateTo.y = angle + 90f;
			Vector3 lookAtPos = playerLocation.position;
			lookAtPos.y += 1f;
            //transform.LookAt (lookAtPos);
            transform.position = playerLocation.position + lerpTo;
            transform.rotation = Quaternion.Euler(rotateTo);
        }
	}
		
}
