using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Notification Center
public class NC : MonoBehaviour {
	static NC self;
	static float notifYSize = 130f;
	public GameObject notifPrefab;
	static Vector3 prefabPos;
	private static bool remove = false;
	private static bool adding = false;

	// Use this for initialization
	void Start () {
		self = this;
		prefabPos = notifPrefab.transform.position;
		prefabPos.x -= 100f;
		StartCoroutine (update());
	}

	private IEnumerator update(){
		if (transform.childCount >0){
			if (transform.childCount > 1) {
				Vector3 posAdd = transform.GetChild (0).localPosition;
				for (int i = 1; i < transform.childCount; i++) {
					Vector3 pos = transform.GetChild (i).localPosition;

					float lerpTo = (posAdd.y - (i * notifYSize));
					pos.y = Mathf.Lerp (pos.y, lerpTo, .25f);
					pos.x = Mathf.Lerp (pos.x, 10f, 0.25f);
					transform.GetChild (i).localPosition = pos;
				}
			}
			Notif not = transform.GetChild (0).GetComponent<Notif>();
			Vector3 lerper = not.transform.localPosition;
			lerper.y = Mathf.Lerp (lerper.y, (not.move)?280f:-70f, .1f);
			lerper.x = Mathf.Lerp (lerper.x, 10f, .25f);
			not.transform.localPosition = lerper;
			if (lerper.y >= 65f){
				Destroy (not.gameObject);
			}
		}
		yield return null;
		StartCoroutine (update());
	}

	public IEnumerator addNotif(Sprite icon, string text){
		while (remove || adding){}
		adding = true;
		int count = transform.childCount;
		Vector3 posAdd = prefabPos;
		if (count > 0){
			posAdd = transform.GetChild (0).localPosition;
			posAdd.y -= (count * notifYSize);
			posAdd.x -= 100f;
		}
		GameObject obj = Instantiate (notifPrefab, posAdd, Quaternion.identity, transform);
		obj.transform.localPosition = posAdd;
		obj.transform.localRotation = Quaternion.identity;
		Notif notif = obj.GetComponent<Notif> ();
		notif.icon = icon;
		notif.text = text;
		adding = false;
		yield return null;
	}

	public static void push(Sprite icon, string text){
		self.StartCoroutine (self.addNotif (icon, text));
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (KeyCode.A)){
			StartCoroutine (addNotif (null, "Testing"));
		}
	}
}
