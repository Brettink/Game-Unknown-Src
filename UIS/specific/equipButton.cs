using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class equipButton : UI, UIAction {
	public WType type;
	public IParams iParams;
	void UIAction.doAction(MParams par){
		if (par.phase == TouchPhase.Ended){
			Item selfE = iParams.invItem;
			Inventory selfinv = iParams.invObj;
			switch (selfE.type){
				case Type.Equip:{
					selfE.setWepType (type);
					string name = (selfE.equipType != EType.weapon) ? selfE.equipType.ToString () : selfE.getWepType ().ToString ();
					GameObject there = GManager.equipment.findByName(name);
					Item item = GManager.equipment.getItem (there);
					GManager.equipment.insertItem (there, selfE);
					selfinv.removeItem (iParams.slot);
					if (item != null){
						selfinv.insertItem (selfinv.firstEmpty (), item);
					}
					GManager.playerLocation.gameObject.SendMessage ("addEquip", selfE);
					GManager.soundEquip.Play ();
					HAS.self.UpdateE ();
					break;
				}
			}
			GManager.self.UIStats.SendMessage ("Hide");
		}
	}
}
