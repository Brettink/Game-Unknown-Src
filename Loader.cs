using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Loader : MonoBehaviour {

	private static GameObject cube;
	private Text perc;
	private float tileGenMax;
	private static GameObject CamLoad;

	private static bool loadingV = true;
	public static bool loading {
		get { return loadingV; }
		set {
			loadingV = value;
			cube.SetActive (value);
		}
	}

	// Use this for initialization
	void Start () {
		CamLoad = transform.GetChild (0).gameObject;
		perc = transform.GetChild (0).GetChild (0).GetComponent<Text> ();
		cube = transform.GetChild (1).gameObject;
		tileGenMax = Mathf.Pow (GManager.MAX_CHUNK_SIZE, 2);
	}
	
	// Update is called once per frame
	void Update () {
		if (loading) {
			cube.transform.Rotate (Vector3.up * 5);
		}
		if (GManager.chunks.Count > 0) {
			float percent = ((GManager.chunks [0].myTile.transform.childCount / tileGenMax) * 100f);
			perc.text = Math.Round (percent, 1) + "%";
			if (percent >= 100f && CamLoad.activeSelf) {
				GManager.myCam.enabled = true;
				GManager.mainCam.enabled = true;
				CamLoad.SetActive (false);
			}
		}
	}
}
