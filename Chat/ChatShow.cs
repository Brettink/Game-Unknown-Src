using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChatShow : MonoBehaviour {

	private STATE state = STATE.NONE;
	public bool dir = false;
	float hide = 0f;
	float showR = 200f;

	void show(bool dir){
		state = STATE.LERPING;
		this.dir = dir;
	}

	bool moveAndCheck(GameObject check, float posTo, bool type){
		float scale = check.transform.localScale.x;
		if (Math.Abs(scale - posTo) > 0.01f){
			scale = Mathf.Lerp (scale, posTo, .5f);
			check.transform.localScale = new Vector3 (scale, scale, 1f);
			return true;
		}
		return false;
	}

	// Update is called once per frame
	void Update () {
		switch (state){
			case STATE.LERPING:{
				float posTo = (dir)?showR:hide;
				if (!moveAndCheck (this.gameObject, posTo, true)) {
					state = STATE.NONE;
					if (!dir) {
						GManager.GSState = GS.N;
					}
				}
				break;
			}
		}
	}
}
