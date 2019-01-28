using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIToggle : UI {
	private SpriteRenderer self2;

	public SpriteRenderer selfGet(){
		return self2;
	}

	void Awake(){
		self2 = GetComponent<SpriteRenderer> ();
	}
}
