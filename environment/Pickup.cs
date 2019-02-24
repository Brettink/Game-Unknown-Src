using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pickup : MonoBehaviour {
	public IEBaseMesh item;
	MeshFilter childMesh;
	MeshRenderer childRend;
	// Use this for initialization
	void Awake () {
		GameObject child = transform.GetChild (0).GetChild (0).gameObject;
		childMesh = child.GetComponent<MeshFilter> ();
		childRend = child.GetComponent<MeshRenderer> ();
	}

	public void setItem(IEBaseMesh item){
		this.item = item;
		childMesh.mesh = item.mesh;
		childRend.materials = item.mats;
		Bounds bounds = item.mesh.bounds;
		transform.GetChild (0).GetChild (0).localPosition = (Vector3.forward * (bounds.max.z/2f));
		transform.GetChild (0).localPosition = (Vector3.up * (bounds.max.z/2f));
		Vector3 size = transform.GetChild (0).GetComponent<BoxCollider> ().size;
		size.z = bounds.max.z;
		transform.GetChild (0).GetComponent<BoxCollider> ().size = size;
	}

	void OnTriggerEnter(Collider col){
		if (col.gameObject.name == "char") {
			NC.push (item.icon, item.count + "x " + item.name);
			StartCoroutine (GManager.inventory.addItem (item));
			GameObject t = this.gameObject;
			Pos at = GManager.pickups.Where (pickup => pickup.myTile == t).First ();
			GManager.pickups.Remove (at);
			Destroy (this.gameObject);
		}
	}
	
	// Update is called once per frame
	void OnEnable(){
		StartCoroutine (Rotate ());
	}

	IEnumerator Rotate () {
		transform.Rotate (Vector3.up*5f);
        if (transform.position.y < 0f) {
            Vector3 pos = transform.position;
            pos.y = 5f;
            transform.position = pos;
        }
		//yield return new WaitForSeconds (.1f);
		yield return null;
		StartCoroutine (Rotate ());
	}
}
