using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : UI, UIAction {
	public bool isEquipmentInv = false;
	void UIAction.doAction(MParams par){
		if (par.phase == TouchPhase.Ended){
			Inventory selfinv = (isEquipmentInv) ? GManager.equipment : GManager.inventory;
			Item selfE = selfinv.getItem (transform.GetChild (0).gameObject);
            GManager.items.TryGetValue(selfE.name, out Item self);
			if (self != null) {
				if (!isEquipmentInv) {
					GManager.self.UIStats.SetActive (true);
					GManager.self.UIStats.SendMessage ("SetAndShow", 
							new IParams (selfinv, self, transform.GetChild (0).gameObject));
				} else {
					GameObject backTo = GManager.inventory.firstEmpty ();
					if (backTo != null) {
						GManager.inventory.insertItem (backTo, self);
						selfinv.removeItem (this.transform.GetChild (0).gameObject);
						GManager.playerLocation.gameObject.SendMessage ("remEquip", self);
						HAS.self.UpdateE ();
					}
				}
			}
		}
	}
}
