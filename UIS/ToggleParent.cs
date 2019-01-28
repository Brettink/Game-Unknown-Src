using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleParent : MonoBehaviour {

	List<SpriteRenderer> toggles = new List<SpriteRenderer>();
	Color selected = new Color (1f, 1f, 1f, 0.5f);
	Color unSelected = new Color (1f, 1f, 1f, 1f);

	// Use this for initialization
	void Start () {
		for (int i = 0; i < transform.childCount; i++){
			toggles.Add (transform.GetChild (i).GetComponent <SpriteRenderer>());
		}
	}
	
	void onToggleAction(SpriteRenderer toSelect){
		toggles.ForEach ((SpriteRenderer obj) =>{
			obj.color = unSelected;
		});
		toSelect.color = selected;
	}
}
