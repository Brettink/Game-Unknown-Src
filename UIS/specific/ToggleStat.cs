using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleStat : UI, UIAction
{
	public static bool onStat = false;
	private static GameObject equip, stats;
	private static Image selfImg;
	private static Text title;

	new static Color enabled = new Color (1f, 1f, 1f);
	static Color disabled = new Color (.2f, .2f, .2f);

	new void Start(){
		base.Start ();
		selfImg = GetComponent<Image> ();
		equip = transform.parent.GetChild (0).gameObject;
		stats = transform.parent.GetChild (1).gameObject;
		title = transform.parent.GetChild (2).GetComponent<Text> ();
		equip.SetActive (true);
		stats.SetActive (false);
	}

	public static void flip(){
		onStat = !onStat;
		equip.SetActive (!onStat);
		stats.SetActive (onStat);
		selfImg.color = (!onStat) ? enabled : disabled;
		title.text = (!onStat) ? "Equipment" : "Stats";
	}

	void UIAction.doAction(MParams par){
		if (par.phase == TouchPhase.Ended){
			flip ();
		}
	}
}
