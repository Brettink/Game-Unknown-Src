using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class inventoryShow : MonoBehaviour {

	private STATE state = STATE.NONE;
	private GameObject panel, equip;
	public bool dir = false;
	Quaternion rest = Quaternion.Euler (new Vector3 (0f, -90f, 0f));
	Quaternion outP = Quaternion.Euler (new Vector3 (0f, 0f, 0f));

	void show(bool dir){
		state = (dir) ? STATE.MOVING_SELF : STATE.MOVING_PANEL;
		if (!dir){
			if (GManager.self.UIStats.activeSelf){GManager.self.UIStats.SendMessage ("Hide");}
		}
		this.dir = dir;
	}
	void Start () {
		panel = GameObject.FindGameObjectWithTag ("inventory");
		equip = GameObject.FindGameObjectWithTag ("equip");
	}

	bool moveAndCheck(GameObject check, float posTo, bool type){
		Vector3 pos = check.transform.localPosition;
		float posCheck = (type) ? pos.x : pos.y;
		if (Math.Abs(posCheck - posTo) > 0.01f){
			posCheck = Mathf.Lerp (posCheck, posTo, .75f);
			if (type){
				pos.x = posCheck;
			} else {
				pos.y = posCheck;
			}
			check.transform.localPosition = pos;
			return true;
		}
		if (type){
			pos.x = posTo;
		} else {
			pos.y = posTo;
		}
		check.transform.localPosition = pos;
		return false;
	}
	
	// Update is called once per frame
	void Update () {
		switch (state){
			case STATE.MOVING_SELF:{
				float posTo = (dir)?270f:1650f;
				if (!moveAndCheck (this.gameObject, posTo, true)) {
					state = (dir) ? STATE.MOVING_PANEL : STATE.NONE;
					if (state == STATE.NONE){
						GManager.GSState = GS.N;
					}
				}
				break;
			}
			case STATE.MOVING_PANEL: {
				float posTo = (dir) ? -1.36f : 1.36f;
				Quaternion rotTo = (dir) ? outP : rest;
				Quaternion rotGet = equip.transform.localRotation;
				Quaternion newRot = Quaternion.Lerp (rotGet, rotTo, 0.4f);
				equip.transform.localRotation = newRot;
				if (!moveAndCheck (panel, posTo, false)){
					Quaternion rot = (dir) ? outP : rest;
					equip.transform.localRotation = rot;
					state = (dir) ? STATE.NONE : STATE.MOVING_SELF;
				}
				break;
			}
		}
	}
}
