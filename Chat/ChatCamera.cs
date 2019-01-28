using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatCamera : MonoBehaviour {

	public static ChatCamera self;
	public void Awake(){
		self = this;
	}

	public static void Change(Transform parent){
		Change (parent, false);
	}

	public static void Change(Transform parent, bool remove){
		if (!remove) {
			GManager.SetLayerRec (parent.gameObject, LayerMask.NameToLayer ("Chat"));
		}
		if (self.transform.parent != null){
			GManager.SetLayerRec (self.transform.parent.gameObject, LayerMask.NameToLayer ("Default"));
		}
		self.transform.parent = (remove)?null:parent;
		self.transform.localPosition = GManager.chatCamPos;
		self.transform.localRotation = Quaternion.Euler (GManager.chatCamRot);
	}
}
