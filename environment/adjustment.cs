using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class adjustment : MonoBehaviour {
	public int type;
	MeshRenderer mesh;
	Material mat;
	SpriteRenderer sel;
	GameObject arrow;
    int x, z;
	float yTo;
    private bool hasStarted = false;
	private bool isSelected = false;

	// Use this for initialization
	void Awake () {
        if (!hasStarted) {
            x = (int)transform.position.x + 12;
            z = (int)transform.position.z + 12;
            mesh = GetComponent<MeshRenderer>();
            sel = transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>();
            arrow = transform.GetChild(2).gameObject;
            arrow.SetActive(false);
            hasStarted = true;
        }
	}

    public float GetYTo() {
        return yTo;
    }

	// Use this for initialization
	void Start () {
		yTo = transform.position.y;
		mat = Resources.Load<Material> ("ground/" + type);
		mesh.material = mat;
	}

	public void moveTileTo(float to){
		if (isSelected) {
			yTo = to;
            transform.parent.SendMessage("pushHeight", new Vector3(x, yTo, z));
		}
	}

	public void arrowEnable(bool enabled){
		arrow.SetActive (enabled);
	}

	public void selDisable(){
		isSelected = false;
		sel.enabled = false;
	}

	public void selEnable(){
		isSelected = true;
		sel.enabled = true;
	}

	void OnTriggerStay(Collider col){
		if (!isSelected) {
			if (col.name == "insetBlock" || col.name == "insetCircle") {
				GManager.selectedTiles.Add (gameObject);
				isSelected = true;
				sel.enabled = true;
			}
		}
	}

	void OnTriggerExit(Collider col){
		if (col.name == "insetBlock" || col.name == "insetCircle"){
			GManager.selectedTiles.Remove (gameObject);
			isSelected = false;
			sel.enabled = false;
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (transform.position.y != yTo) {
			Vector3 posTo = transform.position;
            if (Mathf.Abs(transform.position.y - yTo) < 0.05) {
                posTo.y = yTo;
            } else {
                posTo.y = Mathf.Lerp(posTo.y, yTo, .05f);
            }
			transform.position = posTo;
		}
	}
}
