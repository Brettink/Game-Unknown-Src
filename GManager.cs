using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using UnityEngine.UI;

public class GManager : MonoBehaviour {

    public bool doTesting = false;
	public static GManager self;
	public static float speed = 0.015f;
	private float dist = 15f;
	public const int MAX_CHUNK_SIZE = 25;
	public GameObject empty;
	float randX, randY, randZ, gRandX, gRandY, gRandZ;
	public GameObject GridObject, TilePrefab, PickupPrefab;
	public GameObject ambience;
	public static GameObject selectBox, selBox, selCirc;

	public static Transform playerLocation, playerFollowL;
	public static Animator playerAnim;

	public static List<GameObject> enemies;

	public GameObject UINormal, UIAction, UIEdit, UIInventory, UIMove, UIChat, UIStats, chatCam;
	public GameObject UIHaS;

	private static Dictionary <string, GameObject> UIs= new Dictionary<string, GameObject>();
	private static Dictionary <string, GameObject> worldUIs = new Dictionary<string, GameObject>();
	public static Dictionary<string, Chats> npcChats = new Dictionary<string, Chats>();
	public static List<Pos> chunks = new List<Pos> ();
	public static List<Pos> pickups = new List<Pos> ();
	public static List<Pos> enemiesL = new List<Pos> ();
	public static Item[] items;
	public static List<GameObject> selectedTiles = new List<GameObject> ();
	public static AudioSource soundUI, soundEquip;
	public static Inventory inventory, equipment;
	private Vector3 selDefScale = new Vector3(0.5f, 0.25f, 0.5f), selPrev;
	public static Vector3 chatCamPos = new Vector3 (0, 1.72f, 2.15f), chatCamRot = new Vector3(0f, -180f, 0f);
	private static GS state = GS.L;
	public static Camera myCam, mainCam, chCam;
	private List<Vector2> chunkGen = new List<Vector2>();
    public static Dictionary<Vector2Int, Chunk> chunkScr = new Dictionary<Vector2Int, Chunk>();
    public static Dictionary<Vector2Int, int[]> heightIMatrix = new Dictionary<Vector2Int, int[]>();

    public static GS GSState{
		get { return state; }
		set {
			if (state != value)
				soundUI.Play ();
			state = value;
			HideShow ();
		}
	}

	private static ES editT = ES.SS;
	public static ES editType{
		get { return editT; }
		set {
			if (editT !=value)
				soundUI.Play ();
			editT = value;
			if (value == ES.SS || value == ES.CS) {
				selectedTiles.ForEach ((GameObject obj) => {
					obj.SendMessage ("arrowEnable", false);
					obj.SendMessage ("selDisable");
				});
			}
			switch (value){
				case ES.SS: { selectBox.SetActive (false); selectBox = selBox; selectBox.SetActive (true); break; }
				case ES.CS: { selectBox.SetActive (false); selectBox = selCirc; selectBox.SetActive (true); break; }
				case ES.LB: {
					selectedTiles.ForEach ((GameObject obj) => {
						obj.SendMessage ("arrowEnable", true);
					});
					break;
				}
			}
		}
	}

	private static bool sideV = false;
	public static bool sideView{
		get { return sideV; }
		set {
			if (sideV != value)
				soundUI.Play ();
			sideV = value;
			if (!value){
				playerFollowL =  GameObject.FindGameObjectWithTag ("Cam3").transform;
			} else {
				playerFollowL =  GameObject.FindGameObjectWithTag ("Cam4").transform;
			}
		}
	}

	private static bool showMoveV = false;
	public static bool showMove{
		get { return showMoveV; }
		set {
			showMoveV = value;
			self.UIMove.SetActive (value);
		}
	}

	private static float moveToVal = 1f;
	public static float moveTo{
		get { return moveToVal; }
		set {
			moveToVal = value;
			selectedTiles.ForEach ((GameObject obj) => {
				obj.SendMessage ("moveTileTo", value);
			});
		}
	}

	public static void HideShow(){
		self.UIAction.SetActive (false);
		self.UIEdit.SetActive (false);
		self.UINormal.SetActive (false);
		self.UIInventory.SetActive (false);
		self.UIMove.SetActive (false);
		self.UIChat.SetActive (false);
		self.UIHaS.SetActive (true);
		selectBox.SetActive (false);
		chCam.enabled = false;
		//chatCam.SetActive (false);
		selectedTiles.ForEach ((GameObject obj) => {
			obj.SendMessage ("arrowEnable", false);
			obj.SendMessage ("selDisable");
		});
		selectedTiles.Clear ();
		switch (GSState){
			case GS.N:{
				selectBox.SetActive (false);
				self.UINormal.SetActive (true);
				self.UIAction.SetActive (true);
				break;
			}
			case GS.I:{
				self.UIInventory.SetActive (true);
				self.UIInventory.SendMessage ("show", true);
				break;
			}
			case GS.C:{
				chCam.enabled = true;
				self.UIChat.SetActive (true);
				self.UIChat.SendMessage ("show", true);
				break;
			}
			case GS.E:{
				self.UINormal.SetActive (true);
				self.UIEdit.SetActive (true);
				self.UIMove.SetActive (true);
				self.UIHaS.SetActive (false);
				self.UIMove.transform.localPosition = new Vector3 (4.2f, -6.1f);
				selectBox.SetActive (true);
				break;
			}
		}
	}

	public static void SetLayerRec(GameObject obj, int newLayer){
		obj.layer = newLayer;
		foreach (Transform child in obj.transform){
			SetLayerRec (child.gameObject, newLayer);
		}
	}

	// Use this for initialization
	void Awake () {
		self = this;
		randX = UnityEngine.Random.Range(-100000f, 100000f);
		randY = UnityEngine.Random.Range(-100000f, 100000f);
		randZ = UnityEngine.Random.Range(-100000f, 100000f);
		gRandX = UnityEngine.Random.Range(-100000f, 100000f);
		gRandY = UnityEngine.Random.Range(-100000f, 100000f);
		gRandZ = UnityEngine.Random.Range(-100000f, 100000f);
		sideView = false;
		inventory = new Inventory (GameObject.FindGameObjectWithTag ("inventory"), false);
		equipment = new Inventory (GameObject.FindGameObjectWithTag ("equipS"), true);

		myCam = GameObject.FindGameObjectWithTag ("Cam2").GetComponent<Camera> ();
		mainCam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
		soundUI = myCam.transform.GetChild (1).GetComponent<AudioSource> ();
		soundEquip = myCam.transform.GetChild (2).GetComponent<AudioSource> ();

		playerLocation = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ();
		//playerAnim = playerLocation.GetComponent<Animator> ();
		enemies = GameObject.FindGameObjectsWithTag ("Enemy").ToList ();
		enemies.ForEach (t=>enemiesL.Add (new Pos(t)));
		//playerFollowL = GameObject.FindGameObjectWithTag ("PlayerFollow").GetComponent<Transform> ();
		UIInventory.SetActive (false);
		UINormal.SetActive (false);
		UIEdit.SetActive (false);
		UIMove.SetActive (false);
		UIChat.SetActive (false);
		UIStats.SetActive (false);
		UIHaS.SetActive (false);
		chCam = chatCam.GetComponent<Camera> ();
		chCam.enabled = false;
		//chatCam.SetActive (false);
		selBox = GameObject.FindGameObjectWithTag ("selectBox");
		selCirc = GameObject.FindGameObjectWithTag ("selectCircle");
		selBox.SetActive (false);
		selCirc.SetActive (false);
		selectBox = selBox;
	}

	void OnSheath(bool sheath){
		StartCoroutine (waitUntilReady (sheath));
	}

	IEnumerator waitUntilReady(bool sheath){
		while (!UIAction.activeSelf){
			yield return null;
		}
		UIAction.BroadcastMessage ("OnSheath", sheath);
		yield return null;
	}

	IEnumerator doCheck(List<Pos> toCheck, float minusTDist){
		int newDist = (int)(dist - minusTDist);
		Vector3 playerPos = playerLocation.position;
		var tilesToEnable = toCheck.Where (t =>
			(int)Vector3.Distance(t.transF.position, playerPos) < newDist &&
			!t.myTile.activeSelf
		).ToList ();
		yield return null;
		var tilesToDisable = toCheck.Where (t =>
			(int)Vector3.Distance(t.transF.position, playerPos) < newDist*2
			&& !((int)Vector3.Distance(t.transF.position, playerPos) < newDist)
			&& t.myTile.activeSelf
		).ToList ();
		yield return null;
		tilesToDisable.ForEach (t=> {
                bool cont = true;
                if (toCheck == chunks) {
                    Chunk ch = t.myTile.GetComponent<Chunk>();
                    if (ch.stillLoading) {
                        cont = false;
                    } else {
                        if (ch.r != null) {
                            ch.StopCoroutine(ch.r);
                        }
                    }
                }
                if (cont) {
                    t.myTile.SetActive(false);
                }
            }
        );
		yield return null;
		tilesToEnable.ForEach (t=>t.myTile.SetActive (true));
		//yield return new WaitForSeconds (1f);
		yield return null;
		if (toCheck == chunks) {
			int playerX = (int)playerPos.x / MAX_CHUNK_SIZE;
			int playerZ = (int)playerPos.z / MAX_CHUNK_SIZE;
			for (int x = playerX - 1; x <= playerX + 1; x++) {
				for (int z = playerZ - 1; z <= playerZ + 1; z++) {
					Vector2 chunkPos = new Vector2 (x, z);
					if (!chunkGen.Contains (chunkPos)){
						chunkGen.Add (chunkPos);
						StartCoroutine (generate(chunkPos));
						yield return null;
					}
				}
			}
		}
		StartCoroutine (doCheck(toCheck, minusTDist));
		yield return null;
	}

	private bool check(float minX, float maxX, float minZ, float maxZ, Vector3 pos){
		return (pos.x >= minX && pos.x <= maxX) &&
			(pos.z >= minZ && pos.z <= maxZ);
	}

	public static void addUI(string name, GameObject UI, bool is3D){
		if (!is3D) {
			UIs.Add (name, UI);
		} else {
			worldUIs.Add (name, UI);
		}
	}

	private string getData(string path, out bool hasError){
		hasError = false;
		string data = "";
		if (Application.platform == RuntimePlatform.Android){
#pragma warning disable CS0618 // Type or member is obsolete
            WWW reader = new WWW (path);
#pragma warning restore CS0618 // Type or member is obsolete
            while (!reader.isDone && string.IsNullOrEmpty (reader.error)) {}
			if (string.IsNullOrEmpty (reader.error)) {
				data = reader.text;
			} else {
				hasError = true;
			}
		} else {
			if (File.Exists (path)) {
				data = File.ReadAllText (path);
			} else {
				hasError = true;
			}
		}
		return data;
	}

	public void addPickup(GameObject obj, Item item){
		addPickup (obj, item, Vector3.zero);
	}

	public void addPickup(GameObject obj, Item item, Vector3 toVect){
		obj.AddComponent<Pickup>().setItem (item);
		toVect.y = 500f;
		obj.GetComponent<Rigidbody> ().AddRelativeForce (toVect);
	}

	IEnumerator generate(Vector2 start){
		start = start * MAX_CHUNK_SIZE;
		if (start == Vector2.zero) {
			string path = Application.streamingAssetsPath;
			string[] dirs = Enum.GetNames (NPCType.goblin.GetType ());
			foreach (string dir in dirs){
				Chats chatD = new Chats ();
				String dir2 = path + "/chats/" + dir + "/chat";
				bool brk = false;
				for (int i = 0; !brk; i++){
					string[] fileData = getData (dir2 + i + ".txt", out brk).Split ("\n" [0]);
					if (!brk) {
						chatD.AddChat (fileData);
					}
				}
				npcChats.Add (Path.GetFileName (dir), chatD);
			}
			bool noth;
			string data = getData (path + "/items.json", out noth);
			ItemList listItems = JsonUtility.FromJson<ItemList> (data);
			int id = 0;
			items = new Item[listItems.items.Length];
			foreach(ItemFile itemFile in listItems.items){
				items [id] = new Item (itemFile);
				id++;
			}
            StartCoroutine(doCheck(chunks, -30f));
            StartCoroutine(doCheck(pickups, 0f));
            StartCoroutine(doCheck(enemiesL, 0f));
        }
		int selName = 0;
		Vector3 chunkStart = new Vector3 (start.x, 0, start.y);
		GameObject newChunk = Instantiate (empty, chunkStart,
									Quaternion.identity, GridObject.transform);
        Chunk ch = newChunk.GetComponent<Chunk>();
        ch.setPos(new Vector2Int((int)start.x, (int)start.y));
        newChunk.name = "chunk" + start.x + " " + start.y + "";
		chunks.Add (new Pos(newChunk));
		Vector2 scaleS = start / 10f;
		float halfSize = (MAX_CHUNK_SIZE / 2) / 10f;
        int at1 = 0;
        float[,] heights = new float[MAX_CHUNK_SIZE, MAX_CHUNK_SIZE];
        for (float z = -halfSize; z <= halfSize; z+=.1f, at1++) {
            z = (float)Math.Round (z, 2);
			float[] prevTiles = new float[MAX_CHUNK_SIZE+1];
			int at = 0;
			for (float x = -halfSize; x <= halfSize; x+=.1f, at++) {
				Loader.loading = true;
				x = (float)Math.Round (x, 2);
				float tile = Mathf.PerlinNoise ((x + scaleS.x + randX + randZ), (z + scaleS.y + randY + randZ))*5f;
                //tile = (float)Math.Round(tile, 3);
                tile += prevTiles [at];
				prevTiles [at] = tile;
				if (tile <= 0f){ tile = 0.01f; }
				Vector3 pos = new Vector3 (x*10f, tile, z*10f);
				if (x == 0f && z == 0f && start == Vector2.zero){
					playerLocation.position = Vector3.up * (tile + 0.25f);
				}
				if (x %1f == 0f && z == 1f){
					GameObject newPickup = Instantiate (PickupPrefab, pos, Quaternion.identity, newChunk.transform);
					newPickup.transform.localPosition = new Vector3 (pos.x, tile + .5f, pos.z);
					int type = UnityEngine.Random.Range (0, items.Length);
					Item copy = items [type].copy (1);
					addPickup (newPickup, copy, Vector3.zero);
					pickups.Add (new Pos(newPickup));
					//newPickup.SetActive (false);
				}
				GameObject newTile = Instantiate (TilePrefab, pos, Quaternion.identity, newChunk.transform);
				adjustment adj = newTile.GetComponent<adjustment> ();
				float tile2 = Mathf.PerlinNoise ((x + scaleS.x + gRandX + gRandZ), (z + scaleS.y + gRandY + gRandZ))*4f;
				int t =  (int)clampF (tile2, 0, 3);
				adj.type = t;
				newTile.transform.localPosition = pos;
				GameObject arrSel = newTile.transform.GetChild (2).gameObject;
				arrSel.name = start.x + "" + start.y + selName;
				selName++;
                heights[at, at1] = tile;
			}
            yield return null;
		}
        ch.setAllHeights(heights);
        yield return null;
        Loader.loading = false;
		if (start == Vector2.zero) {
			playerLocation.gameObject.GetComponent<Rigidbody> ().isKinematic = false;
			GSState = GS.N;
			Item newItem = items [0].copy (1);
			Item newItem2 = items [1].copy (1);
			StartCoroutine (inventory.addItem (newItem));
			StartCoroutine (inventory.addItem (newItem2));
			ambience.GetComponent<AudioSource> ().Play ();
            GameObject chest = GameObject.FindGameObjectWithTag("Respawn");
            chest.GetComponent<Rigidbody>().isKinematic = false;
			enemies.ForEach ((GameObject obj) => {
				obj.GetComponent<Rigidbody> ().isKinematic = false;
				//obj.SendMessage ("move", (Vector3.back / 25f));
				//obj.SendMessage ("contToMove", true);
			});
		}
		yield return null;
	}
	bool done = false;
    public static Vector3[] baseVerts;
    public static int[] baseTrigs;
    public void setupDefPlane() {
        int size = (GManager.MAX_CHUNK_SIZE * 2);
        int aY = 0;
        for (int y = 0; y < size; y += 2, aY++) {
            int aX = 0;
            for (int x = 0; x < size; x += 2, aX++) {
                int i = x + (y * (size + 1));
                int[] ids = { i, i + 1, i + size + 1, i + size + 2 };
                heightIMatrix.Add(new Vector2Int(aX, aY), ids);
            }
        }
        baseVerts = new Vector3[(size + 1) * (size + 1)];
        for (int i = 0, y = 0; y <= size; y++) {
            for (int x = 0; x <= size; x++, i++) {
                baseVerts[i] = new Vector3((x / 2f) - 12.25f, 0f, (y / 2f) - 12.25f);
            }
        }
        baseTrigs = new int[size * size * 6];
        for (int ti = 0, vi = 0, y = 0; y < size; y++, vi++) {
            for (int x = 0; x < size; x++, ti += 6, vi++) {
                baseTrigs[ti] = vi;
                baseTrigs[ti + 3] = baseTrigs[ti + 2] = vi + 1;
                baseTrigs[ti + 4] = baseTrigs[ti + 1] = vi + size + 1;
                baseTrigs[ti + 5] = vi + size + 2;
            }
        }
    }

	// Update is called once per frame
	void Update () {
		if (!done){
            if (!doTesting) {
                setupDefPlane();
                chunkGen.Add(Vector2.zero);
                StartCoroutine(generate(Vector2.zero));
                //StartCoroutine (generate(new Vector2(1, 0)));
                done = true;
            } else {
                Loader.loading = false;
                GSState = GS.N;
            }
		}
		if ((Input.touchCount > 0) && GSState != GS.L) {
			for (int touch = 0; touch < Input.touchCount; touch++) {
				Touch point = Input.GetTouch (touch);
				Vector2 pointer = point.position;
				Ray rayCast = mainCam.ScreenPointToRay (pointer);
				RaycastHit hit;
				if (Physics.Raycast (rayCast, out hit)) {
					TouchPhase touchPhase = point.phase;
					if (UIs.ContainsKey (hit.collider.name)){
						GameObject UI;
						UIs.TryGetValue (hit.collider.name, out UI);
						UI.SendMessage ("UIAction.doAction", new MParams(hit, touchPhase));
					} else  {
						playerLocation.gameObject.SendMessage ("endMove");
						Ray rayCast2 = myCam.ScreenPointToRay (pointer);
						if (Physics.Raycast (rayCast2, out hit)) {
							if (worldUIs.ContainsKey (hit.collider.name)){
								GameObject UI;
								worldUIs.TryGetValue (hit.collider.name, out UI);
								UI.SendMessage ("UIAction.doAction", new MParams(hit, touchPhase));
							} else if (hit.collider.name == "Cube(Clone)") {
								if (GSState == GS.E && (editType == ES.SS || editType == ES.CS)) {
									Vector3 addIn = hit.transform.position;
									Vector3 loggy = selDefScale;
									loggy.x -= 0.05f; loggy.z -= 0.05f;
									if (touchPhase == TouchPhase.Began) {
										selectedTiles.Clear ();
										selectedTiles.Add (hit.collider.gameObject);
										hit.collider.SendMessage ("selEnable");
										addIn.y += 0.35f;
										selectBox.transform.localScale = loggy;
										selectBox.transform.position = addIn;
										selPrev = addIn;

									} else if (touchPhase == TouchPhase.Moved){
										if (hit.transform.position != selectBox.transform.position){
											Vector3 newScale;
											scaleandAbs (hit.transform.position, selPrev, out newScale);
											//float plusX = (hit.transform.position.x - selPrev.x);
											//float plusZ = (hit.transform.position.z - selPrev.z);
											newScale.x -= 0.05f;
											newScale.z -= 0.05f;
											newScale.y = 0.25f;
											selectBox.transform.localScale = newScale;
											//Vector3 newPos = new Vector3 (selPrev.x + plusX, selPrev.y, selPrev.z + plusZ);
											//selectBox.transform.position = newPos;
										}else {
											selectBox.transform.localScale = loggy;
										}
									}
								}
							}
						}
					}
				} else {
					playerLocation.gameObject.SendMessage ("endMove");
				}
				if (GSState != GS.E
						&& point.phase == TouchPhase.Began
						&& point.phase != TouchPhase.Moved) {
					Vector3 vec = Camera.main.ScreenToViewportPoint (pointer);
					if (vec.x <= 0.5f) {
						showMove = true;
						vec.x = GManager.map (vec.x, 0, 0.5f, 0f, 14f);
						vec.x = GManager.clampF (vec.x, 4f, 10.5f);
						vec.y = GManager.map (vec.y, 1f, 0f, 0f, -16f);
						vec.y = GManager.clampF (vec.y, -12f, -4f);
						UIMove.transform.localPosition = vec;
					}
				}
			}
		} else {
			if (showMove) showMove = false;
		}

	}

	void scaleandAbs(Vector3 value, Vector3 newScale, out Vector3 returnValue){
		returnValue = new Vector3 ();
		returnValue.x = (float)Math.Abs (value.x - newScale.x);
		returnValue.z = (float)Math.Abs (value.z - newScale.z);
		if (returnValue.x < 0.5f)
			returnValue.x = 0.5f;
		if (returnValue.z < 0.5f)
			returnValue.z = 0.5f;
	}

	public static Vector3 clampVector3(Vector3 value, Vector3 min, Vector3 max){
		value.x = clampF (value.x, min.x, max.x);
		value.y = clampF (value.y, min.y, max.y);
		value.z = clampF (value.z, min.z, max.z);
		return value;
	}

	public static float clampF(float val, float mini, float maxi){
		if (val > maxi)
			return maxi;
		else if (val < mini)
			return mini;
		else
			return val;
	}

	public static float map(float value, float valFrom, float valTo, float valMin, float valMax){
		return (((value - valFrom) / (valTo - valFrom)) *
			(valMax - valMin) + valMin);
	}
}
