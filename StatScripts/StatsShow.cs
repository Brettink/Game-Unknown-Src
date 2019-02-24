using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class StatsShow : MonoBehaviour
{
	Text itemName, btnTexR, stats;
	equipButton btnR, btnLE;
	Image itemIcon;
	GameObject btnL;
	STATE state = STATE.NONE;
	bool dir = false;
	float hide = -583f;
	float showR = -6.5f;
    // Start is called before the first frame update
    void Start()
    {
		Enable ();
    }

	void Enable(){
		itemName = transform.GetChild (0).GetComponent<Text> ();
		Transform statContain = transform.GetChild (1).GetChild (0);
		itemIcon = statContain.GetChild (0).GetComponent<Image> ();
		stats = statContain.GetChild (2).GetComponent<Text> ();
		btnR = transform.GetChild (1).GetChild (1).GetChild (0).GetComponent<equipButton> ();
		btnLE = transform.GetChild (1).GetChild (1).GetChild (1).GetComponent<equipButton> ();
		btnTexR = btnR.transform.GetChild (0).GetComponent<Text> ();
		btnL = btnLE.gameObject;
	}

	void OnEnable(){
		Enable ();
	}

	public void Hide(){
		state = STATE.LERPING;
		dir = false;
		itemName.text = System.String.Empty;
		itemIcon.sprite = null;
	}

	public void SetAndShow(IParams iParam){
		state = STATE.LERPING;
		dir = true;
		UpdateFields (iParam);
	}

	public void UpdateFields(IParams iParams){
        Item item = iParams.invItem;
		itemName.text = "Stats: " + item.name;
		itemIcon.sprite = item.icon;
		string statTotal = System.String.Empty;
		bool newLine = false;
        if (item is Modable) {
            Modable itEm = item as Modable;
            foreach (Modifier mod in itEm.mods) {
                int digis = (int)Math.Floor(Math.Log10(mod.val) + 1);
                statTotal += mod.stat.ToString() + ": " + mod.val + ((newLine) ? "\n" : new String(' ', 12 - digis));
            }
        }
		stats.text = statTotal;
		bool isWep = (item is IWeapon);
		btnL.SetActive (isWep);
		if (isWep){
			btnLE.iParams = iParams;
		}
		btnR.iParams = iParams;
		string btnText = System.String.Empty;
		if (isWep || item is IArmor) {
			btnText = (btnL.activeSelf)?"Equip Right":"Equip";
		} else {
			btnText = "Heal";
		}
		btnTexR.text = btnText;
	}

	bool moveAndCheck(GameObject check, float posTo, bool type){
		Vector3 pos = check.transform.localPosition;
		if (Math.Abs(pos.y - posTo) > 0.01f){
			pos.y = Mathf.Lerp (pos.y, posTo, .75f);
			check.transform.localPosition = pos;
			return true;
		}
		pos.y = posTo;
		check.transform.localPosition = pos;
		return false;
	}

    // Update is called once per frame
    void Update()
    {
		switch (state){
		case STATE.LERPING:{
				float posTo = (dir)?showR:hide;
				if (!moveAndCheck (this.gameObject, posTo, true)) {
					state = STATE.NONE;
					if (!dir) {
						gameObject.SetActive (false);
					}
				}
				break;
			}
		}
    }
}
