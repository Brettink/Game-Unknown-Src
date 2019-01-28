using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notif : MonoBehaviour {

	public Sprite icon;
	public string text;
	float ttl = 0f;
	public bool move = false;

	// Use this for initialization
	void Start () {
		ttl = Time.fixedTime + 3f;
		transform.GetChild (0).GetComponent<Image> ().sprite = icon;
		transform.GetChild (1).GetComponent<Text> ().text = text;
	}

	void FixedUpdate(){
		if (!move){
			if (Time.fixedTime >= ttl){
				move = true;
			}
		}
	}
}
