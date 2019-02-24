using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class HAS : MonoBehaviour
{
	public static HAS self;
	Image healthBar, manaBar, xpBar;
	Text healthMan, lvl;
	public static Dictionary<STAT, float> stats = new Dictionary<STAT, float> ();
	private Dictionary<STAT, float> baseStats = new Dictionary<STAT, float>();

	public static float
		health 		= 120f, 
		maxHealth 	= 120f, 
		mana 		= 120f, 
		maxMana 	= 120f,
		xp 			= 0f, 
		xpToNext 	= 1000f,
		lvlTot 		= 0f;

    // Start is called before the first frame update
    void Start()
    {
		self = this;
		foreach (STAT value in Enum.GetValues (STAT.AGI.GetType ())){
			baseStats.Add (value, 10f);
		}
		healthBar = transform.GetChild (1).GetComponent<Image> ();
		manaBar = transform.GetChild (2).GetComponent<Image> ();
		xpBar = transform.GetChild (3).GetComponent<Image> ();
		healthMan = transform.GetChild (5).GetComponent<Text> ();
		lvl = transform.GetChild (6).GetComponent<Text> ();
		UpdateE ();
    }

	public void UpdateE(){
		stats.Clear ();
		baseStats.ToList().ForEach (dict=>stats.Add (dict.Key, dict.Value));
		foreach (Modable item in GManager.equipment.getSlots ().Values.Where (t=>t!=null)){
			foreach (Modifier mod in item.mods){
				stats [mod.stat] += mod.val;
			}
		}
		calculate ();
	}

	public void calculate(){
		maxHealth = (int)(stats [STAT.CON] * (((stats [STAT.END] / 2.5f) + (stats [STAT.STR] / 5f)) * 2f));
		maxMana = 	(int)(stats [STAT.WIS] * (((stats [STAT.INT] / 2.5f) + (stats [STAT.AGI] / 5f)) * 2f));

		if (health > maxHealth)
			health = maxHealth;
		if (mana > maxMana)
			mana = maxMana;
		UpdateS ();
		if (StatWin.selfObj.activeSelf) {
			StatWin.UpdateNums ();
		}
	}

	public void UpdateS(){
		float hPerc = health / maxHealth;
		float mPerc = mana / maxMana;
		float xPerc = xp / xpToNext;
		healthBar.fillAmount = hPerc;
		manaBar.fillAmount = mPerc;
		xpBar.fillAmount = xPerc;
		healthMan.text = Math.Round (hPerc*100f, 1) + "%\n" + Math.Round (mPerc*100f, 1) + "%";
		lvl.text = (int)lvlTot + "";
	}
}
