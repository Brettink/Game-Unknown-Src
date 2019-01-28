using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class StatWin : MonoBehaviour
{
	private bool hadStarted = false;
	public static GameObject selfObj;
	private static Dictionary<string, Text> statList = new Dictionary<string, Text> ();
	private static Text listMaxs;

    // Start is called before the first frame update
    void Awake()
    {
		if (!hadStarted) {
			selfObj = gameObject;
			for (int i = 0; i < transform.GetChild (0).childCount; i++) {
				Transform getChild = transform.GetChild(0).GetChild (i);
				Text t = getChild.GetChild (0).GetComponent<Text> ();
				statList.Add (getChild.name, t);
			}
			listMaxs = transform.GetChild (1).GetComponent<Text> ();
			hadStarted = true;
		}
    }

	void OnEnable(){
		if (hadStarted){
			StatWin.UpdateNums ();
		}
	}

	public static void UpdateNums(){
		foreach (String keyS in StatWin.statList.Keys){
			STAT key = (STAT)(Enum.Parse (STAT.AGI.GetType (), keyS));
			if (HAS.stats.ContainsKey (key)) {
				StatWin.statList [keyS].text = HAS.stats [key] + " ";
			}
		}
		listMaxs.text = HAS.maxHealth + "\n" + HAS.maxMana + "\n" + HAS.xp;
	}
}
