using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;

public class GManager : MonoBehaviour {

    public bool doTesting = false;
	public static GManager self;
    public static int tick = 0;
	public static float speed = 0.015f;
    public static string worldName = "Testing";
	private float dist = 15f;
    public float aspectRatio = 0f;
	public const int MAX_CHUNK_SIZE = 25;
	public GameObject empty;
	float randX, randY, randZ, gRandX, gRandY, gRandZ;
    public GameObject GridObject, TilePrefab, PickupPrefab;
    public Transform playerHead;
	public GameObject ambience;
	public static GameObject selectBox, selBox, selCirc;
    public static Vector2 notZero = new Vector2(0.00001f, 0.00001f);
    public static Color[] colorMap = new Color[7];
    public static Vector3[] baseMainVerts;
    public static Dictionary<SIDE, HashSet<int>> 
        sideVerts = new Dictionary<SIDE, HashSet<int>>();

	public static Transform playerLocation, playerFollowL;
	public static Animator playerAnim;

	//public static List<GameObject> enemies;

	public GameObject UINormal, UIAction, UIEdit, UIInventory, UIMove, UIChat, UIStats, chatCam;
	public GameObject UIHaS, UICompass, UIOptions, UIPivot;

    private Dictionary<EnemyType, GameObject> enemyPrefabs = new Dictionary<EnemyType, GameObject>();
	private static Dictionary <string, GameObject> UIs= new Dictionary<string, GameObject>();
	private static Dictionary <string, GameObject> worldUIs = new Dictionary<string, GameObject>();
	public static Dictionary<string, Chats> npcChats = new Dictionary<string, Chats>();
	public static List<Pos> pickups = new List<Pos> ();
	public static List<Pos> enemiesL = new List<Pos> ();
    public static Dictionary<string, Item> items = new Dictionary<string, Item>();
	public static List<GameObject> selectedTiles = new List<GameObject> ();
	public static AudioSource soundUI, soundEquip;
	public static Inventory inventory, equipment;
	private Vector3 selDefScale = new Vector3(0.5f, 1f, 0.5f), selPrev;
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
        self.UICompass.SetActive(false);
        self.UIOptions.SetActive(false);
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
                self.UICompass.SetActive(true);
				break;
			}
			case GS.I:{
				self.UIInventory.SetActive (true);
				self.UIInventory.SendMessage ("show", true);
				break;
			}
			case GS.C:{
                playerLocation.SendMessage("endMove");
                chCam.enabled = true;
				self.UIChat.SetActive (true);
				self.UIChat.SendMessage ("show", true);
				break;
			}
			case GS.E:{
                playerLocation.SendMessage("endMove");
				self.UINormal.SetActive (true);
				self.UIEdit.SetActive (true);
				self.UIMove.SetActive (true);
				self.UIHaS.SetActive (false);
				self.UIMove.transform.localPosition = new Vector3 (4.2f, -6.1f);
				selectBox.SetActive (true);
				break;
			}
            case GS.O: {
                self.UIOptions.SetActive(true);
                self.UIHaS.SetActive(false);
                playerFollowL.SendMessage("endMove");
                self.UIOptions.SendMessage("Show", true);
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

    public static void convertToSides(Mesh inMesh, Vector3[] baseVerts, ref Dictionary<SIDE, HashSet<int>> dict) {
        dict.Clear();
        Color[] colorsB = inMesh.colors;
        for (int i = 0; i < colorMap.Length + 1; i++) {
            dict.Add((SIDE)(int)i, new HashSet<int>());
        }
        for (int i = 0; i < colorsB.Length; i++) {
            if (!colorsB[i].Equals(Color.white)) {
                SIDE index = (SIDE)Array.IndexOf(colorMap, colorsB[i]);
                if (index < 0) { index = SIDE.NW; }
                dict.TryGetValue(index, out HashSet<int> setVal);
                setVal.Add(i);
                for (int z = 0; z < baseVerts.Length; z++) {
                    if (i != z) {
                        if (baseVerts[z].Equals(baseVerts[i])) {
                            setVal.Add(z);
                        }
                    }
                }
                dict[index] = setVal;
            }
        }
    }

	// Use this for initialization
	void Awake () {
        Input.backButtonLeavesApp = true;
		self = this;
        colorMap[0] = Color.green;
        colorMap[1] = Color.blue;
        colorMap[2] = new Color(0f, 1f, 1f);
        colorMap[3] = Color.black;
        colorMap[4] = Color.red;
        colorMap[5] = new Color(1f, 0f, 1f);
        colorMap[6] = new Color(1f, 1f, 0f);
        Mesh tileMesh = TilePrefab.GetComponent<MeshFilter>().sharedMesh;
        baseMainVerts = tileMesh.vertices;
        convertToSides(tileMesh, baseMainVerts, ref sideVerts);
        tileMesh.vertices = baseMainVerts;
		sideView = false;
		inventory = new Inventory (GameObject.FindGameObjectWithTag ("inventory"), false);
		equipment = new Inventory (GameObject.FindGameObjectWithTag ("equipS"), true);

		myCam = GameObject.FindGameObjectWithTag ("Cam2").GetComponent<Camera> ();
		mainCam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();

		soundUI = myCam.transform.GetChild (1).GetComponent<AudioSource> ();
		soundEquip = myCam.transform.GetChild (2).GetComponent<AudioSource> ();

		playerLocation = GameObject.FindGameObjectWithTag ("Player").GetComponent<Transform> ();
		//playerAnim = playerLocation.GetComponent<Animator> ();
		//playerFollowL = GameObject.FindGameObjectWithTag ("PlayerFollow").GetComponent<Transform> ();
		UIInventory.SetActive (false);
		UINormal.SetActive (false);
		UIEdit.SetActive (false);
		UIMove.SetActive (false);
		UIChat.SetActive (false);
		UIStats.SetActive (false);
		UIHaS.SetActive (false);
        UICompass.SetActive(false);
        UIOptions.SetActive(false);
		chCam = chatCam.GetComponent<Camera> ();
		chCam.enabled = false;
		//chatCam.SetActive (false);
		selBox = GameObject.FindGameObjectWithTag ("selectBox");
		selCirc = GameObject.FindGameObjectWithTag ("selectCircle");
		selBox.SetActive (false);
		selCirc.SetActive (false);
		selectBox = selBox;
        foreach (EnemyType type in Enum.GetValues(EnemyType.archer.GetType())) {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/enemy/" + type.ToString());
            enemyPrefabs.Add(type, prefab);
        }
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
		tilesToEnable.ForEach (t=>t.myTile.SetActive (true));
		//yield return new WaitForSeconds (1f);
		yield return null;
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

	public void addPickup(GameObject obj, IEBaseMesh item){
		addPickup (obj, item, Vector3.zero);
	}

	public void addPickup(GameObject obj, IEBaseMesh item, Vector3 toVect){
		obj.AddComponent<Pickup>().setItem (item);
		toVect.y = 500f;
		obj.GetComponent<Rigidbody> ().AddRelativeForce (toVect);
	}

    public void LoadDefault(WorldInfo info = null) {
        if (info != null) {
            playerLocation.position = info.playerPos;
            randX = info.seeds[0]; randY = info.seeds[1]; randZ = info.seeds[2];
            gRandX = info.seeds[3]; gRandY = info.seeds[4]; gRandZ = info.seeds[5];
        } else {
            randX = UnityEngine.Random.Range(-100000f, 100000f);
            randY = UnityEngine.Random.Range(-100000f, 100000f);
            randZ = UnityEngine.Random.Range(-100000f, 100000f);
            gRandX = UnityEngine.Random.Range(-100000f, 100000f);
            gRandY = UnityEngine.Random.Range(-100000f, 100000f);
            gRandZ = UnityEngine.Random.Range(-100000f, 100000f);
            ChunkManager.worldInfo = 
                new WorldInfo(new float[] { randX, randY, randZ, gRandX, gRandY, gRandZ });
        }
        string path = Application.streamingAssetsPath;
        string[] dirs = Enum.GetNames(NPCType.goblin.GetType());
        foreach (string dir in dirs) {
            Chats chatD = new Chats();
            String dir2 = path + "/chats/" + dir + "/chat";
            bool brk = false;
            for (int i = 0; !brk; i++) {
                string[] fileData = getData(dir2 + i + ".txt", out brk).Split("\n"[0]);
                if (!brk) {
                    chatD.AddChat(fileData);
                }
            }
            npcChats.Add(Path.GetFileName(dir), chatD);
        }
        bool noth;
        string data = getData(path + "/items.json", out noth);
        ItemList listItems = JsonUtility.FromJson<ItemList>(data);
        listItems.weapons.ForEach(w => items.Add(w.name, new IWeapon(w)));
        listItems.armours.ForEach(a => items.Add(a.name, new IArmor(a)));
        listItems.placeAbles.ForEach(p => items.Add(p.name, new IPlaceable(p)));
        listItems.consumables.ForEach(c => items.Add(c.name, new IConsume(c)));
        StartCoroutine(doCheck(pickups, 0f));
        StartCoroutine(doCheck(enemiesL, -5f));
    }

    public void LoadPlayerInfo() {
        Item newItem = items["Fire Sword"].copy(1);
        Item newItem2 = items["Aegis Sword"].copy(1);
        Item newItem3 = items["Leather Chaps"].copy(1);
        Item newItem4 = items["Leather Chest"].copy(1);
        Item newItem5 = items["Leather Helm"].copy(1);
        StartCoroutine(inventory.addItem(newItem));
        StartCoroutine(inventory.addItem(newItem2));
        StartCoroutine(inventory.addItem(newItem3));
        StartCoroutine(inventory.addItem(newItem4));
        StartCoroutine(inventory.addItem(newItem5));
        ambience.GetComponent<AudioSource>().Play();
        GameObject chest = GameObject.FindGameObjectWithTag("Respawn");
        chest.GetComponent<Rigidbody>().isKinematic = false;
    }

	public IEnumerator generate(Vector2 start){
		start = start * MAX_CHUNK_SIZE;
        Vector2Int startInt = new Vector2Int((int)start.x, (int)start.y);
		if (start == Vector2Int.zero) {
            LoadDefault();
        }
		int selName = 0;
		Vector3Int chunkStart = new Vector3Int ((int)start.x, 0, (int)start.y);
		GameObject newChunk = Instantiate (empty, chunkStart,
									Quaternion.identity, GridObject.transform);
        Chunk ch = newChunk.GetComponent<Chunk>();
        ch.setPos(startInt);
        newChunk.name = "chunk" + start.x + " " + start.y + "";
		ChunkManager.chunks.Add (startInt, ch);
		Vector2 scaleS = ((Vector2)start)/ 10f;
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
                float tile = 0f, tile2 = 0f;
                tile = Mathf.PerlinNoise((x + scaleS.x + randX + randZ), (z + scaleS.y + randY + randZ)) * 5f;
                tile2 = Mathf.PerlinNoise((x + scaleS.x + gRandX + gRandZ), (z + scaleS.y + gRandY + gRandZ)) * 4f;
                //tile = (float)Math.Round(tile, 3);
                tile += prevTiles[at];
                prevTiles[at] = tile;

                int t = (int)clampF(tile2, 0, 3);
                if (tile <= 0f) { tile = 0.01f; }
				Vector3 pos = new Vector3 (x*10f, tile, z*10f);
				if (x == 0f && z == 0f && start == Vector2.zero){
					playerLocation.position = Vector3.up * (tile + 1f);
				}
				/*if (x %1f == 0f && z == 1f){
					GameObject newPickup = Instantiate (PickupPrefab, pos, Quaternion.identity, newChunk.transform);
					newPickup.transform.localPosition = new Vector3 (pos.x, tile + .5f, pos.z);
					int type = UnityEngine.Random.Range (0, items.Length);
					Item copy = items [type].copy (1);
					addPickup (newPickup, copy, Vector3.zero);
					pickups.Add (new Pos(newPickup));
					//newPickup.SetActive (false);
				}*/
				GameObject newTile = Instantiate (TilePrefab, pos, Quaternion.identity, newChunk.transform);
				adjustment adj = newTile.GetComponent<adjustment> ();

				adj.type = t;
				newTile.transform.localPosition = pos;
				GameObject arrSel = newTile.transform.GetChild (3).gameObject;
				arrSel.name = start.x + "" + start.y + selName;
				selName++;
                heights[at, at1] = tile;
                
                float chance = (tile + tile2);
                if (chance >= 3.595f && chance <= 3.6f) {
                    adj.SetPlacer(items["Path1"] as IPlaceable);
                    int id = (int)map(chance, 3.595f, 3.6f, 0, enemyPrefabs.Count()-1);
                    GameObject enemySpawn = Instantiate(enemyPrefabs[(EnemyType)id]);
                    enemySpawn.GetComponent<Rigidbody>().isKinematic = false;
                    Vector3 pp = Vector3.up*2f;
                    enemySpawn.transform.position = pos+(chunkStart) + pp;
                    enemySpawn.SetActive(false);
                    enemiesL.Add(new Pos(enemySpawn));
                }
			}
            yield return null;
		}
        ch.setAllHeights(heights);
        yield return null;
        Loader.loading = false;
		if (start == Vector2.zero) {
            ChunkManager.worldInfo.playerPos = GManager.playerLocation.position;
            ChunkManager.currAreas.Add(new ChunkArea(0, 0));
            ChunkManager.self.SaveWorld();
            LoadPlayerInfo();
            playerLocation.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            ChunkManager.doneFirstload = true;
            GSState = GS.N;
        }
		yield return null;
	}
	public static bool done = false;
    public static Vector3[] baseVerts;
    public static int[] baseTrigs;
    public void setupDefPlane() {
        int size = (MAX_CHUNK_SIZE * 2);
        float hfSize = (MAX_CHUNK_SIZE / 2f) - .25f;
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
                baseVerts[i] = new Vector3((x / 2f) - hfSize, 0f, (y / 2f) - hfSize);
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

    void FixedUpdate() {
        if (ChunkManager.doneFirstload)tick++;
        if (tick > 24000) { tick = 0; }
    }

	// Update is called once per frame
	void Update () {
        Debug.Log(tick);
		if (!done){
            if (!doTesting) {
                setupDefPlane();
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
                        try {
                            UIs.TryGetValue(hit.collider.name, out GameObject UI);
                            UI.SendMessage("UIAction.doAction", new MParams(hit, touchPhase));
                        } catch (Exception e) {
                            e.ToString();
                        }
					} else  {
						Ray rayCast2 = myCam.ScreenPointToRay (pointer);
						if (Physics.Raycast (rayCast2, out hit)) {
							if (worldUIs.ContainsKey (hit.collider.name)){
								worldUIs.TryGetValue (hit.collider.name, out GameObject UI);
								UI.SendMessage ("UIAction.doAction", new MParams(hit, touchPhase));
							} else if (hit.collider.name == "Cube(Clone)") {
								if (GSState == GS.E && (editType == ES.SS || editType == ES.CS)) {
									Vector3 addIn = hit.transform.position;
									Vector3 loggy = selDefScale;
									loggy.x -= 0.05f; loggy.z -= 0.05f;
									if (touchPhase == TouchPhase.Began) {
										selectedTiles.Clear ();
										selectedTiles.Add (hit.collider.gameObject);
										hit.collider.gameObject.SendMessage ("selEnable");
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
											newScale.y = 1f;
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
					if (vec.x <= 0.5f && vec.y<2f/3f) {
						showMove = true;
						vec.x = GManager.map (vec.x, 0, 0.5f, 0f, 14f);
						vec.x = GManager.clampF (vec.x, 4f, 10.5f);
						vec.y = GManager.map (vec.y, 1f, 0f, 0f, -16f);
						vec.y = GManager.clampF (vec.y, -12f, -8f);
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
