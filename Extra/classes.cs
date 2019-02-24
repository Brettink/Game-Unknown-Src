using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;
using UnityEngine.Rendering;
using System.IO;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;

public class EquipObject{
	public MeshFilter mesh;
	public MeshRenderer mat;
    public SkinnedMeshRenderer rend;
    public BoxCollider col;

	public EquipObject(MeshFilter mesh, MeshRenderer mat, BoxCollider col){
		this.mesh = mesh; this.mat = mat;
        this.col = col;
	}
    public EquipObject(SkinnedMeshRenderer rend, BoxCollider col) {
        this.rend = rend;
        this.col = col;
    }
}

public class MParams{
	public RaycastHit hit;
	public TouchPhase phase;
	public MParams(RaycastHit hit, TouchPhase phase){
		this.hit = hit;
		this.phase = phase;
	}
}

public class IParams{
	public Inventory invObj;
	public Item invItem;
	public GameObject slot;
	public IParams(Inventory invObj, Item invItem, GameObject slot){
		this.invObj = invObj;
		this.invItem = invItem;
		this.slot = slot;
	}
}

[Serializable]
public class Modifier{
	[SerializeField]
	public STAT stat = STAT.STR;
	[SerializeField]
	public float val = 0f;
	public Modifier(STAT stat, float val){this.stat = stat; this.val = val;}
}

[Serializable]
public class ItemList: IDisposable {
	public List<ItemWeaponFile> weapons;
    public List<ItemArmourFile> armours;
    public List<ItemPlaceableFile> placeAbles;
    public List<ItemConsumeFile> consumables;

    public void Dispose()
    {
        weapons = null;
        armours = null;
        placeAbles = null;
    }
}

[Serializable]
public class ItemBaseFile: IDisposable {
	public string name = "";
	public int maxCount = 0;
	public string model = "";

    public void Dispose(){}
}
[Serializable]
public class ItemEquipFile: ItemBaseFile{
    public List<Modifier> mods = new List<Modifier>();
    new public void Dispose() {
        base.Dispose();
        mods = null;
    }
}
[Serializable]
public class ItemWeaponFile: ItemEquipFile {
    new public void Dispose() {
        base.Dispose();
    }
}
[Serializable]
public class ItemArmourFile: ItemEquipFile {
    public EType type = EType.chestS;
    public List<string> bones = new List<string>();
    new public void Dispose() {
        base.Dispose();
        bones = null;
    }
}
[Serializable]
public class ItemPlaceableFile: ItemBaseFile {
    new public void Dispose() {
        base.Dispose();
    }
}
[Serializable]
public class ItemConsumeFile: ItemEquipFile {
    new public void Dispose() {
        base.Dispose();
    }
}

[Serializable]
public class WorldInfo {
    public float[] seeds;
    public SerialVector3 playerPos;
    public WorldInfo(float[] seeds, SerialVector3 playerPos) {
        this.seeds = seeds;
        this.playerPos = playerPos;
    }
    public WorldInfo(float[] seeds) {
        this.seeds = seeds;
    }
}

[Serializable]
public class EnemyInfo {
    public SerialVector3 spawnPos = Vector3.zero;
    public SerialVector3 currPos = Vector3.zero;
    int type = 0;
    float percHealth;
    public EnemyInfo(SerialVector3 spawnPos, int type, float percHealth) {
        this.spawnPos = spawnPos; this.type = type; this.percHealth = percHealth;
    }
}

[Serializable]
public class ChunkArea {
    public int iX, iZ;
    public Dictionary<SerialVector3, ChunkInfo> chunks = new Dictionary<SerialVector3, ChunkInfo>();
    public ChunkArea(int iX, int iZ, Dictionary<SerialVector3, ChunkInfo> chunks) {
        Set(iX, iZ);
        this.chunks = chunks;
    }
    public ChunkArea(int iX, int iZ) {
        Set(iX, iZ);
    }
    public void Set(int iX, int iZ) {
        this.iX = iX; this.iZ = iZ;
    }
}

[Serializable]
public class ChunkInfo {
    public Dictionary<int, TileInfo> tiles = new Dictionary<int, TileInfo>();
    public ChunkInfo (Dictionary<int, TileInfo> tiles) {
        this.tiles = tiles;
    }
}

[Serializable]
public class TileInfo {
    public float height;
    public int type;
    public string placerName;
    public TileInfo(float height, int type, string placerName) {
        this.height = height; this.type = type;
        this.placerName = placerName;
    }
}

public class Item: IDisposable {
	public string name = "";
	public int maxCount = 0, count = 0;
	public Sprite icon;
    public string addin = string.Empty, prefix = string.Empty;
    public string baseLocation;

	public Item(){}
	public void Setup(string name, int maxCount, string model){
		this.name = name;
		this.maxCount = maxCount;
        this.baseLocation = "items/" + prefix + model + "/";
        string iconLocation = baseLocation + model + addin + "I";
        icon = Resources.Load<Sprite>(iconLocation);
    }

	public Item copy(int count){
		Item itemC = new Item ();
		itemC.name = name; itemC.maxCount = maxCount; itemC.count = count;
        itemC.icon = icon;
		return itemC;
	}

    public void Dispose(){}
}

public class IEBaseMesh: Item {
    public Mesh mesh;
    public Material[] mats;
    public void Setup(ItemBaseFile itemIn) {
        base.Setup(itemIn.name, itemIn.maxCount, itemIn.model);
        string modelLocation = baseLocation + itemIn.model + ((addin == string.Empty) ? "M" : addin);
        string matsLocation = baseLocation + itemIn.model + "Mat/";
        Material[] matNames = Resources.LoadAll<Material>
            ((addin == string.Empty) ? modelLocation : matsLocation);
        mats = new Material[matNames.Length];
        int i = 0;
        foreach (Material mat in matNames) {
            mats[i] = Resources.Load<Material>(matsLocation + mat.name);
            i++;
        }
        mesh = Resources.Load<Mesh>(modelLocation);
    }
}

public class Modable: IEBaseMesh {
    public List<Modifier> mods;
    public void Setup(ItemEquipFile itemIn) {
        base.Setup(itemIn);
    }
}

public class IConsume: Modable {
    public IConsume(ItemConsumeFile itemIn) {
        prefix = "Consumeables/";
        base.Setup(itemIn);
        mods = new List<Modifier>(itemIn.mods);
    }
}

public class IWeapon: Modable{

    private WType wType = WType.slotR;

    public IWeapon(ItemWeaponFile itemIn) {
        prefix = "Weapons/";
        base.Setup(itemIn);
        mods = new List<Modifier>(itemIn.mods);
    }

    public void setWepType(WType type) {
        this.wType = type;
    }

    public string getWepType() {
        return this.wType.ToString();
    }
}

public class IPlaceable: IEBaseMesh {
    public Dictionary<SIDE, HashSet<int>> sideIds = new Dictionary<SIDE, HashSet<int>>();
    public Vector3[] baseVerts;
    public IPlaceable() { }
    public IPlaceable(ItemPlaceableFile itemIn) {
        prefix = "Placeables/";
        addin = "M";
        base.Setup(itemIn);
        baseVerts = mesh.vertices;
        GManager.convertToSides(mesh, baseVerts, ref sideIds);
    }

    public bool isEqual(IPlaceable other) {
        return this.name.Equals(other.name);
    }
    /*public IPlaceable copy() {
        IPlaceable place = new IPlaceable();

    }*/
}

public class IArmor: Modable {
    public EType eType = EType.chestS;
    public List<string> bones = new List<string>();
    public IArmor(ItemArmourFile itemIn) {
        prefix = "Armors/";
        eType = itemIn.type;
        addin = ((int)eType) + string.Empty;
        base.Setup(itemIn);
        mods = itemIn.mods;
        bones = itemIn.bones;
    }
   
}

public class Inventory{
	private Dictionary<GameObject, Item> slots = new Dictionary<GameObject, Item>();
	private bool inv = false;

	public Dictionary<GameObject, Item> getSlots(){
		return slots;
	}

	public Inventory(GameObject invParent, bool inv){
		this.inv = inv;
		for(int i = 0; i < invParent.transform.childCount; i++){
			if (invParent.transform.GetChild (i).name != "charBack") {
				slots.Add (invParent.transform.GetChild (i).GetChild (0).gameObject, null);
			}
		}
	}

	public IEnumerator addItem (Item item){
		bool didCountAdd = false;
		if (slots.ContainsValue (item)){
			var allMatches = slots.Where (kvp => kvp.Value == item && kvp.Value.count < kvp.Value.maxCount);

		}
		if (!slots.ContainsValue (item) && !didCountAdd){
			var allMatches = firstEmpty ();
			slots [allMatches] = item;
			allMatches.GetComponent<SpriteRenderer> ().sprite = item.icon;
			updateCount (allMatches, item.count);
		}
		yield return null;
	}

	private void updateCount(GameObject parent, int count){
		if (!inv) {
			var c = (count > 0) ? count+"" : "";
			parent.transform.GetChild (0).GetComponent<Text> ().text = "" + c;
		}
	}

	public void insertItem(GameObject key, Item item){
		slots [key] = item;
		key.GetComponent<SpriteRenderer> ().sprite = item.icon;
		updateCount (key, item.count);
	}

	public GameObject firstEmpty(){
		return slots.Where (kvp => kvp.Value == null).Select (kvp =>kvp.Key).First ();
	}

	public GameObject findByName(string name){
		return slots.Keys.Where (key => key.transform.parent.name == name).First ();
	}

	public Item getItem(GameObject clickedUI){
		if (slots[clickedUI] != null){
			return slots[clickedUI];
		} else {return null; }
	}

	public void removeItem(GameObject key){
		key.GetComponent<SpriteRenderer> ().sprite = null;
		slots [key] = null;
		updateCount (key, 0);
	}
}

public class Pos {
	public GameObject myTile;
	public Transform transF;
	public Pos(GameObject tile){
		myTile = tile;
		transF = tile.transform;
	}

}

public class ChatY {
	public string msg; public bool me;
	public ChatY(string msg, bool me){this.msg = msg; this.me = me;}
	public string toString(){return this.msg + " is npc" + me;}
}

public class Chats{
	public List<ChatY[]> seqs = new List<ChatY[]>();
	public Chats(){}

	public void AddChat(string[] msgs){
		ChatY[] seq = new ChatY[msgs.Length];
		int i = 0;
		foreach (string msg in msgs){
			String[] chat = msg.Split(":"[0]);
			bool me = chat [0].Contains ("Me");
			seq [i] = new ChatY (chat [1], me);
			i++;
		}
		seqs.Add (seq);
	}
}
[Serializable]
public struct SerialVector3 {
    public float x, y, z;

    public SerialVector3(float rX, float rY, float rZ) {
        x = rX; y = rY; z = rZ;
    }

    public override string ToString() {
        return String.Format("[{0}, {1}, {2}]", x, y, z);
    }

    public static implicit operator Vector3(SerialVector3 rValue) {
        return new Vector3(rValue.x, rValue.y, rValue.z);
    }

    public static implicit operator SerialVector3(Vector3 rValue) {
        return new SerialVector3(rValue.x, rValue.y, rValue.z);
    }
}
