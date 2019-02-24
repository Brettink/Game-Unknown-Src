using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
#if (UNITY_EDITOR)
public class MeshCreator : MonoBehaviour
{
    public GameObject RootMesh;
    public int index = 0;
    public string nameToSave;
    public bool saveToDisk = false;
    //private Dictionary<string, Transform> allBonesM = new Dictionary<string, Transform>();
    // Start is called before the first frame update
    void Start()
    {
        Vector3 newScale = new Vector3(1.125f, 1f, 1.125f);
        SkinnedMeshRenderer rend = RootMesh.transform.GetChild(index).GetComponent<SkinnedMeshRenderer>();
        //GetBones(RootMesh.transform, ref allBonesM);
       // Debug.Log(allBonesM.Count());
        SkinnedMeshRenderer rend2 = transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>();
        // rend2.transform.localScale = newScale;
        Vector3 center = Vector3.left * .5f;
        Quaternion rot1 = new Quaternion(), rot2 = new Quaternion(), rot3 = new Quaternion();
        rot1.eulerAngles = new Vector3(0.01f, 0f, 0.0f);
        rot3.eulerAngles = new Vector3(-0.02f, -5f, 0f);
        rot2.eulerAngles = new Vector3(0.0f, -2f, 0.001f);
        Mesh newMesh = (Mesh)Instantiate(rend.sharedMesh);
        newMesh.MarkDynamic();
        /*newMesh.
        int[] tris = new int[rend.sharedMesh.triangles.Length];
        Vector3[] verts = new Vector3[rend.sharedMesh.vertices.Length];
        BoneWeight[] weights = new BoneWeight[rend.sharedMesh.boneWeights.Length];
        System.Array.Copy(rend.sharedMesh.triangles, tris, rend.sharedMesh.triangles.Length);
        System.Array.Copy(rend.sharedMesh.vertices, verts, rend.sharedMesh.vertices.Length);
        System.Array.Copy(rend.sharedMesh.boneWeights, weights, rend.sharedMesh.boneWeights.Length);
        newMesh.vertices = verts;
        newMesh.triangles = tris;
        newMesh.boneWeights = weights;*/
        //rend2.bones = new Transform[rend2.bones.Length];

        Vector3 bbF = new Vector3(0f, 0.5789f, 0f);
        Vector3 bb = 
            GManager.playerLocation.GetComponent<PlayerController>().RootBone.transform.position;
        string[] boneNames = rend.bones.Select(t => t.name).ToArray();
        Vector3[] vertic = newMesh.vertices;
        if (rend.name.Contains("Bottom") || rend.name.Contains("Helmet")) {
            for (int i = 0; i < newMesh.vertices.Length; i++) {
                if (rend.name.Contains("Bottom")){
                    if (vertic[i].x < 0f) {
                        vertic[i] = RotatePointAroundPivot(vertic[i], -bb + vertic[i], rot1.eulerAngles);
                        if (vertic[i].y < -.00308f) {
                            vertic[i] = RotatePointAroundPivot(vertic[i], -bb + vertic[i], rot3.eulerAngles);
                            vertic[i].x /= 1.17f;
                            vertic[i].z /= 1.17f;
                        } else {
                            vertic[i].x /= 1.01f;
                            vertic[i].z /= 1.08f;
                        }

                    } else if (vertic[i].x > 0f) {
                        vertic[i] = RotatePointAroundPivot(vertic[i], -bb + vertic[i], rot2.eulerAngles);

                        if (vertic[i].y < -.00308f) {
                            vertic[i].x /= 1.17f;
                            vertic[i].z /= 1.17f;
                        } else {
                            vertic[i].x /= 1.01f;
                            vertic[i].z /= 1.08f;
                        }
                    }
                    vertic[i].Scale(newScale);
                } else {
                    vertic[i].x *= 1.1f;
                    vertic[i].z *= 1.1f;
                }
            }
            newMesh.vertices = vertic;
            newMesh.RecalculateBounds();
        } else if (rend.name.Contains("Body")) {
            int i = System.Array.IndexOf(boneNames, "mixamorig:Head");
            boneNames[i] = "mixamorig:chestS";
        }
        rend2.sharedMesh = newMesh;
        rend2.sharedMaterials = rend.sharedMaterials;

        Transform[] bones = new Transform[boneNames.Length];
        string buff = string.Empty;
        for (int i = 0; i < boneNames.Length; i++) {
            PlayerController.allBones.TryGetValue(boneNames[i], out Transform bone);
            // allBonesM.TryGetValue(boneNames[i], out Transform bone2);
            //Debug.Log(bone2.position - bone.position);
            bones[i] = bone;
            Debug.Log(boneNames[i]);
            buff += boneNames[i] + "\n";
        }
        Debug.Log(buff);
        rend2.bones = bones;
        Debug.Log(rend2.bones.Length);
        if (saveToDisk) {
            AssetDatabase.CreateAsset(rend2.sharedMesh, "Assets/Resources/items/" + nameToSave + ".asset");
            AssetDatabase.SaveAssets();
        }
    }

    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }
}
#endif