using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tile : MonoBehaviour {
    MeshRenderer rend;
    Material mat;

    void Start() {
        rend = GetComponent<MeshRenderer>();
        mat = rend.material;
    }
	// Update is called once per frame
	void Update () {
		if (transform.parent.hasChanged){
			transform.parent.hasChanged = false;
            mat.mainTextureScale = new Vector2(transform.parent.position.y*2f, 1f);
            rend.material = mat;
			transform.localScale = new Vector3 (1f, transform.parent.position.y, 1f);
		}
	}
}
