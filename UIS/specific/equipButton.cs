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
			if (selfE is Modable) {
				if (!(selfE is IConsume)) {
                    string name = string.Empty;
                    if (selfE is IWeapon) {
                        IWeapon weap = selfE as IWeapon;
                        weap.setWepType(type);
                        name = type.ToString();
                        Debug.Log("Slot: " + name);
                    } else {
                        IArmor arm = selfE as IArmor;
                        name = arm.eType.ToString();
                    }
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

				}
			}
			GManager.self.UIStats.SendMessage ("Hide");
		}
	}
}
