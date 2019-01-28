using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScroll : UI {

	public DIR dir = DIR.Horizontal;
	public float minPos, maxPos;
	private float val = 0f;
	private float valTo = 0f;
	private Transform scrollDial;

	// Use this for initialization
	void Awake () {
		scrollDial = transform.GetChild (0);
	}

	public void setVal(float valTo){ this.valTo = valTo; }

	void Update(){
		if (val != valTo) {
			val = Mathf.Lerp (val, valTo, 0.25f);
			Vector3 sdPos = scrollDial.localPosition;
			switch (dir) {
				case DIR.Horizontal: { sdPos.x = val; break; }
				case DIR.Vertical: { sdPos.y = val; break; }
			}
			scrollDial.localPosition = sdPos;
			SendMessage ("onMove", val);
		}
	}
}
