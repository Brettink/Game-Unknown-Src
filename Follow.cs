using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

	public float heightBy = 12f;
	public float zBy = 0f;

	public void move(Vector3 moveBy){
		transform.position += moveBy;
	}

	public void endMove(){}
	
	// Update is called once per frame
	void Update () {
		if (GManager.GSState != GS.E) {
			Vector3 pos = GManager.playerLocation.position;
			pos.y = (heightBy >=0)?pos.y + heightBy:1.2f;
			pos.z += zBy;
			transform.position = pos;
		}
	}
}
