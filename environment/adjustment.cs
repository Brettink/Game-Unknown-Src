using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class adjustment : MonoBehaviour {
	public int type;
    MeshRenderer mesh, plMesh;
    public string placerName;
    public GameObject plObject;
    MeshFilter plFilt;
    MeshCollider plCol;
	Material mat;
	SpriteRenderer sel;
	GameObject arrow;
    int x, z;
	float yTo;
    private bool hasStarted = false;
	private bool isSelected = false;
    private Mesh myMesh, placerMesh;

	// Use this for initialization
	void Awake () {
        if (!hasStarted) {
            x = (int)transform.position.x + 12;
            z = (int)transform.position.z + 12;
            mesh = GetComponent<MeshRenderer>();
            MeshFilter m = GetComponent<MeshFilter>();
            myMesh = Instantiate(m.mesh) as Mesh;
            m.mesh = myMesh;
            plFilt = plObject.GetComponent<MeshFilter>();
            plMesh = plObject.GetComponent<MeshRenderer>();
            plCol = plObject.GetComponent<MeshCollider>();
            sel = transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>();
            arrow = transform.GetChild(3).gameObject;
            arrow.SetActive(false);
            hasStarted = true;
        }
	}

    public void SetPlacer(IPlaceable placeObject) {
        SetPlacer(placeObject, false);
    }

    public void SetPlacer(IPlaceable placeObject, bool toDelete) {
        placerName = (toDelete)?string.Empty:placeObject.name;
        plMesh.materials = (toDelete) ? PlayerController.defaultMat:placeObject.mats;
        plFilt.sharedMesh = (toDelete) ? null:Instantiate(placeObject.mesh) as Mesh;
        plCol.sharedMesh = (toDelete) ? null : plFilt.sharedMesh;
        placerMesh = (toDelete) ? null : plFilt.sharedMesh;
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
        if (Input.GetKeyUp(KeyCode.U)) {
            if (placerName != string.Empty) {
                IPlaceable pO = GManager.items[placerName] as IPlaceable;
                Vector3[] myVerts = new Vector3[GManager.baseMainVerts.Length];
                Array.Copy(GManager.baseMainVerts, myVerts, myVerts.Length);
                Vector3[] plVerts = new Vector3[pO.baseVerts.Length];
                Array.Copy(pO.baseVerts, plVerts, plVerts.Length);
                GManager.sideVerts.TryGetValue(SIDE.N, out HashSet<int> setP);
                pO.sideIds.TryGetValue(SIDE.N, out HashSet<int> setPL);
                float r = UnityEngine.Random.Range(-.5f, .5f);
                setP.ToList().ForEach(i => myVerts[i].y = r + 1.025f);
                setPL.ToList().ForEach(i => plVerts[i].y = r);
                myMesh.vertices = myVerts;
                placerMesh.vertices = plVerts;
            }
        }
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
