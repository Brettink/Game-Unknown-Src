using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour {
	Collider selfCollider;
	public SpriteRenderer selfSprite;
	public GameObject self;
	public bool is3D = false;
	// Use this for initialization
	public void Start () {
		self = this.gameObject;
		selfCollider = GetComponent<Collider> ();
		selfSprite = GetComponent<SpriteRenderer> ();
		GManager.addUI (selfCollider.name, gameObject, is3D);
	}

}
public interface UIAction{
	void doAction(MParams par);
}
