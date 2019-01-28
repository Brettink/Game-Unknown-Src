using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : Controller {
    public GameObject RootBone;
	public GameObject[] equipItems;
	public GameObject[] sheathRPos, sheathLPos;
    private Dictionary<string, Transform> allBonesM = new Dictionary<string, Transform>();
    private Dictionary<string, Transform> allBones = new Dictionary<string, Transform>();
	Material[] defaultMat;
	private static Dictionary<GameObject, EquipObject> equipModels = new Dictionary<GameObject, EquipObject> ();
	new void Start(){
		base.Start ();
        GetBones(RootBone.transform, ref allBones);
        Debug.Log(allBones.Count());
        GameObject rendO = Resources.Load("items/leath") as GameObject;
        SkinnedMeshRenderer rend = rendO.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
        Transform rendY = Instantiate(rendO.transform.GetChild(3), RootBone.transform);
        Quaternion rot = new Quaternion();
        rot.eulerAngles = new Vector3(0f, -10f, -10f);
        rendY.localRotation = rot;
        rendY.localPosition -= Vector3.up * 1f;
        GetBones(rendY, ref allBonesM);
        Debug.Log(allBonesM.Count());
        SkinnedMeshRenderer rend2 = transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>();
        Vector3 center = Vector3.left * .5f;
        Quaternion rot1 = new Quaternion(), rot2 = new Quaternion();
        rot1.eulerAngles = new Vector3(-5f, 0f, 0f);
        rot2.eulerAngles = new Vector3(0f, 40f, 0f);
        Mesh newMesh = rend.sharedMesh;
        /*newMesh.MarkDynamic();
        Vector3[] vertic = newMesh.vertices;
        for (int i = 0; i < newMesh.vertices.Length; i++) {
            // if (vertic[i].x < 0f) {
            vertic[i] = rot1 * (vertic[i] - center) + center;
            vertic[i] = rot2 * vertic[i];
            vertic[i].x -= .0001f;
            vertic[i].z -= .0014f;
            // }
        }
        newMesh.vertices = vertic;*/
        rend2.sharedMesh = newMesh;
        rend2.sharedMaterials = rend.sharedMaterials;
        string[] boneNames = rend.bones.Select(t => t.name).ToArray();
        Transform[] bones = new Transform[boneNames.Length];
        for (int i = 0; i < boneNames.Length; i++) {
            allBones.TryGetValue(boneNames[i], out Transform bone);
            allBonesM.TryGetValue((boneNames[i].Contains("Hips"))?boneNames[i]+"(Clone)":boneNames[i], out Transform boneTo);
            Debug.Log(bone.name);
            boneTo.SetParent(bone);
            bones[i] = boneTo;
        }

        rend2.bones = bones;
        Debug.Log(rend2.bones.Length);
        GManager.playerAnim = selfAnimator;
		foreach (GameObject obj in equipItems){
			MeshFilter mesh = obj.GetComponent<MeshFilter> ();
			MeshRenderer mat = obj.GetComponent<MeshRenderer> ();
			if (defaultMat == null)
				defaultMat = mat.materials;
			equipModels.Add (obj, new EquipObject(mesh, mat));
		}
	}

	public void remEquip(Item item){
		addEquip (item, true);
	}

	public void addEquip(Item item){
		addEquip (item, false);
	}

    public void GetBones(Transform parent, ref Dictionary<string, Transform> dict) {
        if (parent.name.Contains("mixamo")) {
            dict.Add(parent.name, parent);
        }
        for (int i = 0; i < parent.childCount; i++) {
            GetBones(parent.GetChild(i), ref dict);
        }
    }

	public void addEquip(Item item, bool doDelete){
		EquipObject eObj;
		if (item.equipType == EType.weapon) {
			eObj = equipModels.Where (kvp => kvp.Key.name == item.getWepType ()).Select (kvp => kvp.Value).First ();
			selfAnimator.SetLayerWeight (selfAnimator.GetLayerIndex (item.getWepType () + "Wep"), 
				(doDelete)?0f:(sheath)?0f:1f);
			selfAnimator.SetLayerWeight (selfAnimator.GetLayerIndex (item.getWepType () + "Sh"), 
				(doDelete) ? 0f : 1f);
		} else {
			eObj = equipModels.Where (kvp => kvp.Key.name == item.equipType.ToString ()).Select (kvp=>kvp.Value).First ();
		}
		eObj.mesh.mesh = (doDelete)?null:item.mesh;
		eObj.mat.materials = (doDelete)?defaultMat:item.mats;
		checkWeaponStatus ();
		GManager.self.SendMessage("OnSheath", sheath);
	}

	public static void checkWeaponStatus(){
		List<EquipObject> eObjs = equipModels.Where (kvp => kvp.Key.name.Contains ("slotR") ||
			kvp.Key.name.Contains ("slotL"))
			.Select (kvp => kvp.Value).ToList ();
		bool hasWeaponIn = false;
		eObjs.ForEach ((EquipObject e)=>{
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
			EquipObject eqO, eqO2;
			equipModels.TryGetValue (equipItems [2], out eqO);
			equipModels.TryGetValue (equipItems [3], out eqO2);
			bool isEquipR = (eqO.mesh.mesh.name != "slotR");
			bool isEquipL = (eqO2.mesh.mesh.name != "slotL");
			DoSheath ("slotRWep", 2, sheathRPos, (!sheath)?0:1, (!isEquipR)?0f:(sheath)?0f:1f);
			DoSheath ("slotLWep", 3, sheathLPos, (!sheath)?0:1, (!isEquipL)?0f:(sheath)?0f:1f);
		}
	}
}
