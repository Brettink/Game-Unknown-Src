using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class PlayerController : Controller {
    public Transform RootBone;
	public GameObject[] equipItems, equipItemsMore;
	public GameObject[] sheathRPos, sheathLPos;
    private Bounds defBounds = new Bounds(Vector3.zero, Vector3.one);
	public static Material[] defaultMat;
    public static Dictionary<string, Transform> allBones = new Dictionary<string, Transform>();
    private static Dictionary<GameObject, EquipObject> equipModels = new Dictionary<GameObject, EquipObject> ();
    private static Dictionary<GameObject, EquipObject> equipModelsMore = new Dictionary<GameObject, EquipObject>();
    new void Start(){
		base.Start ();
        GetBones(RootBone.transform, ref allBones);
        Debug.Log(allBones.Count());
        GManager.playerAnim = selfAnimator;
		foreach (GameObject obj in equipItems){
			MeshFilter mesh = obj.GetComponent<MeshFilter> ();
			MeshRenderer mat = obj.GetComponent<MeshRenderer> ();
            BoxCollider col = obj.GetComponent<BoxCollider>();
			if (defaultMat == null)
				defaultMat = mat.materials;
			equipModels.Add (obj, new EquipObject(mesh, mat, col));
		}
        foreach (GameObject obj in equipItemsMore) {
            SkinnedMeshRenderer rend = obj.GetComponent<SkinnedMeshRenderer>();
            BoxCollider col = obj.GetComponent<BoxCollider>();
            equipModelsMore.Add(obj, new EquipObject(rend, col));
        }

    }

	public void remEquip(Item item){
		addEquip (item, true);
	}

	public void addEquip(Item item){
		addEquip(item, false);
	}

	public void addEquip(Item itemI, bool doDelete){
		EquipObject eObj;
		if (itemI is IWeapon) {
            IWeapon item = itemI as IWeapon;
			eObj = equipModels.Where (kvp => kvp.Key.name == item.getWepType ()).Select (kvp => kvp.Value).First ();
			selfAnimator.SetLayerWeight (selfAnimator.GetLayerIndex (item.getWepType () + "Wep"), 
				(doDelete)?0f:(sheath)?0f:1f);
			selfAnimator.SetLayerWeight (selfAnimator.GetLayerIndex (item.getWepType () + "Sh"), 
				(doDelete) ? 0f : 1f);
            eObj.mesh.mesh = (doDelete) ? null : item.mesh;
            eObj.mat.materials = (doDelete) ? defaultMat : item.mats;
            Bounds bounds = (doDelete) ? defBounds : item.mesh.bounds;
            eObj.col.center = bounds.center;
            eObj.col.size = bounds.size;
        } else if (itemI is IArmor) {
            IArmor item = itemI as IArmor;
            eObj = equipModelsMore.Where (kvp => kvp.Key.name == item.eType.ToString ()).Select (kvp=>kvp.Value).First ();
            eObj.rend.sharedMesh = (doDelete) ? null : item.mesh;
            eObj.rend.materials = (doDelete) ? defaultMat : item.mats;
            Transform[] bones = (doDelete) ? null : new Transform[item.bones.Count];
            if (!doDelete) {
                int ind = 0;
                item.bones.ForEach(bone => {
                    allBones.TryGetValue(bone, out bones[ind]);
                    ind++;
                });
            }
            eObj.rend.bones = bones;
        }

		checkWeaponStatus ();
		GManager.self.SendMessage("OnSheath", sheath);
	}

	public static void checkWeaponStatus(){
		List<EquipObject> eObjs = equipModels.Where (kvp => kvp.Key.name.Contains ("slotR") ||
			kvp.Key.name.Contains ("slotL"))
			.Select (kvp => kvp.Value).ToList ();
		bool hasWeaponIn = false;
		eObjs.ForEach ((EquipObject e)=>{
            Debug.Log(e.mesh.name);
			if (e.mesh.mesh.name != "slotR" && e.mesh.mesh.name != "slotL"){
				hasWeaponIn = true;
			}
		});
		hasWeapon = hasWeaponIn;
	}

	new void Update(){
		base.Update ();
	}

	void DoSheath(string layerName, int equipItem, GameObject[] to, int pos, float weight){
		equipItems [equipItem].transform.parent = to[pos].transform;
		equipItems [equipItem].transform.localPosition = Vector3.zero;
		equipItems [equipItem].transform.localRotation = Quaternion.identity;
		selfAnimator.SetLayerWeight (selfAnimator.GetLayerIndex (layerName), weight);
	}

	new void FixedUpdate(){
		base.FixedUpdate ();
		if (CheckPercent ("sheath", sheathAnimation, 4, 9)){
			equipModels.TryGetValue (equipItems [0], out EquipObject eqO);
			equipModels.TryGetValue (equipItems [1], out EquipObject eqO2);
			bool isEquipR = (eqO.mesh.mesh.name != "slotR");
			bool isEquipL = (eqO2.mesh.mesh.name != "slotL");
            Debug.Log(sheath);
			DoSheath ("slotRWep", 0, sheathRPos, (!sheath)?0:1, (!isEquipR)?0f:(sheath)?0f:1f);
			DoSheath ("slotLWep", 1, sheathLPos, (!sheath)?0:1, (!isEquipL)?0f:(sheath)?0f:1f);
		}
	}

    public void GetBones(Transform parent, ref Dictionary<string, Transform> dict) {
        if (parent.name.Contains("mixamo")) {
            dict.Add(parent.name, parent);
        }
        for (int i = 0; i < parent.childCount; i++) {
            GetBones(parent.GetChild(i), ref dict);
        }
    }
}
