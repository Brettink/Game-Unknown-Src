using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BatteryPerc : MonoBehaviour
{
	Text percText;
	Image img;
	float timeDelay = 0f;
	BatteryStatus lastStatus;
    // Start is called before the first frame update
    void Start()
    {
		lastStatus = SystemInfo.batteryStatus;
		percText = transform.parent.GetChild (1).GetComponent<Text> ();
		img = GetComponent<Image> ();
		//if (Application.platform != RuntimePlatform.Android){
		//	transform.parent.gameObject.SetActive (false);
		//}
    }

    // Update is called once per frame
    void Update()
    {
		if (Time.time >= timeDelay || lastStatus != SystemInfo.batteryStatus) {
			lastStatus = SystemInfo.batteryStatus;
			Vector3 scale = Vector3.one;
			scale.x = SystemInfo.batteryLevel;
			string addIn = System.String.Empty;
			switch (lastStatus){
				case BatteryStatus.Charging:{
					addIn = "+";
					break;
				}
				case BatteryStatus.Discharging:{
					addIn = "-";
					break;
				}
			}
			transform.localScale = scale;
			img.color = (scale.x < .2f) ? Color.red : (scale.x < .45f) ? Color.yellow : Color.green;
			string warn = (scale.x < .15f) ? "!" : System.String.Empty;
			percText.text = warn + (scale.x * 100f) + "%" + addIn + warn;
			timeDelay = Time.time + 10f;
		}
    }
}
