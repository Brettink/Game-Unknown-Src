using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tile : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if (transform.parent.hasChanged){
			transform.parent.hasChanged = false;
			transform.localScale = new Vector3 (1f, transform.parent.position.y * 4f, 1f);
		}
	}
}
