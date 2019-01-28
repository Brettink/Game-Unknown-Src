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

	public EquipObject(MeshFilter mesh, MeshRenderer mat){
		this.mesh = mesh; this.mat = mat;
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
	public ItemFile[] items;

    public void Dispose()
    {
        items = null;
    }
}

[Serializable]
public class ItemFile: IDisposable {
	public string name = "";
	public int maxCount = 0;
	public string model = "";
	public Type type = Type.Equip;
	public EType equipType = EType.None;
	public List<Modifier> mods = new List<Modifier> ();

    public void Dispose()
    {
        mods = null;
    }
}

public class Item: IDisposable {
	public string name = "";
	public int maxCount = 0, count = 0;
	public Mesh mesh;
	public Material[] mats;
	public Sprite icon;
	public Type type = Type.Equip;
	public EType equipType = EType.None;
	private WType wType = WType.slotR;
	public List<Modifier> mods = new List<Modifier> ();

	public Item(){}
	public Item(ItemFile itemIn){
		name = itemIn.name;
		mods = new List<Modifier> (itemIn.mods);
		maxCount = itemIn.maxCount;
		string baseLocation = "items/" + itemIn.model + "/";
		string modelLocation = baseLocation + itemIn.model + "M";
		string iconLocation = baseLocation + itemIn.model + "I";
		mesh = Resources.Load<Mesh> (modelLocation);
		Material[] matNames = Resources.LoadAll<Material> (modelLocation);
		int i = 0;
		mats = new Material[matNames.Length];
		string matsLocation = baseLocation + itemIn.model + "Mat/";
		foreach(Material mat in matNames){
			mats [i] = Resources.Load<Material> (matsLocation + mat.name);
			i++;
		}
		icon = Resources.Load<Sprite> (iconLocation);
		type = itemIn.type;
		equipType = itemIn.equipType;
	}

	public Item copy(int count){
		Item itemC = new Item ();
		itemC.name = name; itemC.maxCount = maxCount; itemC.count = count; itemC.mesh = mesh;
		itemC.mats = mats; itemC.icon = icon; itemC.type = type; itemC.equipType = equipType;
		itemC.mods = mods;
		return itemC;
	}

	public void setWepType(WType type){
		wType = type;
	}

	public string getWepType(){
		return wType.ToString ();
	}

    public void Dispose()
    {
        mesh.Clear(); mesh = null;
        mats = null;
        mods = null;
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
