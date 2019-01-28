using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class heightI{
    public int[] ids;
    public float height;
    public heightI(int[] ids, float height) {
        this.ids = ids; this.height = height;
    }
}

public class Chunk : MonoBehaviour
{
    // Start is called before the first frame update
    Mesh mesh;
    MeshCollider col;
    MeshFilter filt;
    private Vector3[] vertices;
    public Vector2Int selfPosInt;
    public bool stillLoading = true;
    Queue<heightI> newHeights = new Queue<heightI>();
    //int size;
    void Start() {
        mesh = new Mesh();
        col = GetComponent<MeshCollider>();
        mesh.MarkDynamic();
        vertices = new Vector3[GManager.baseVerts.Length];
        System.Array.Copy(GManager.baseVerts, vertices, GManager.baseVerts.Length);
        mesh.vertices = vertices;
        int[] triangles;// = new int[size * size * 6];
        triangles = GManager.baseTrigs;
        mesh.triangles = triangles;
        //Testing
        //filt = transform.GetChild(0).GetComponent<MeshFilter>();
        //filt.sharedMesh = mesh;
        //End Testing
        col.sharedMesh = mesh;
    }

    public Coroutine r;

    void OnEnable() {
        r = StartCoroutine(updatePos());
    }

    void OnDisable() {
        if (r != null) {
            StopCoroutine(r);
            r = null;
        }
    }

    bool didStitchX = false, didStitchZ = false, didStitchC = false;
    bool didSomething = false;
    IEnumerator updatePos() {
        if (newHeights.Count() > 0) {
            heightI h;
            do {
                h = newHeights.Dequeue();
                foreach(int id in h.ids) {
                    vertices[id].y = h.height;
                }
            } while (newHeights.Count() > 0);
            didStitchX = false; didStitchZ = false; didStitchC = false;
            didSomething = true;
            yield return null;
        }
        if (!didStitchX) {
            if (Stitch(0)) {
                didStitchX = true;
                didSomething = true;
                yield return null;
            }
        }
        if (!didStitchZ) {
            if (Stitch(1)) {
                didStitchZ = true;
                didSomething = true;
                yield return null;
            }
        }
        if (!didStitchC) {
            if (Stitch(2)) {
                didStitchC = true;
                didSomething = true;
                yield return null;
            }
        }
        if (didSomething) {
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
            yield return null;
            col.sharedMesh = mesh;
            didSomething = false;
        }
        yield return null;
        r = StartCoroutine(updatePos());
    }

    public float[] GetSide(int side) {
        int size = (GManager.MAX_CHUNK_SIZE * 2) + 1;
        int sizeM = size - 1;
        float[] sideHeights = new float[size+1];
        int increm = (side == 0) ? 1 : size-1;
        for (int i = 0; i < size; i++) {
            sideHeights[i] = vertices[i * ((side==0)?1:(size))].y;
        }
        return sideHeights;
    }

    public float GetCorner() {
        return vertices[0].y;
    }

    public void setPos(Vector2Int pos) {
        selfPosInt = pos;
    }

    public bool Stitch(int wh) {
        Vector2Int posP = new Vector2Int(
                    selfPosInt.x + ((wh==1||wh==2)?GManager.MAX_CHUNK_SIZE:0), 
                    selfPosInt.y + ((wh==0||wh==2)?GManager.MAX_CHUNK_SIZE:0)
                );
        GManager.chunkScr.TryGetValue(posP, out Chunk chunkP);
        //Chunk chunkPZ = GManager.chunkScr.Where(t => t.Key.x == selfPosInt.z + 1).Select(t => t.Value).First();
        if (chunkP != null) {
            if (chunkP.gameObject.activeSelf) {
                try {
                    if (wh == 2) {
                        vertices[vertices.Length - 1].y = chunkP.GetCorner();
                    } else {
                        float[] side = chunkP.GetSide(wh);
                        int size = (GManager.MAX_CHUNK_SIZE * 2);
                        int start = (wh == 0) ? vertices.Length - size - 1 : size;
                        int increm = (wh == 0) ? 1 : size + 1;
                        for (int i = start, at = 0; at < size; i += increm, at++) {
                            vertices[i].y = side[at];
                        }
                    }
                    return true;
                } catch (NullReferenceException e) {
                    e.ToString();
                    return false;
                }
            }
        }
        return false;
    }

    public void pushHeight(Vector3 newPos) {
        Vector2Int intPos = new Vector2Int((int)newPos.x, (int)newPos.z);
        if (GManager.heightIMatrix.ContainsKey(intPos)) {
            GManager.heightIMatrix.TryGetValue(intPos, out int[] ids);
            if (ids != null) {
                newHeights.Enqueue(new heightI(ids, newPos.y + .25f));
            }
        }
    }

    public void setAllHeights(float[,] heights) {
        foreach (Vector2Int key in GManager.heightIMatrix.Keys) {
            int[] ids = GManager.heightIMatrix[key];
            foreach (int id in ids) {
                vertices[id].y = heights[key.x, key.y] + .25f;
            }
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        col.sharedMesh = mesh;
        GManager.chunkScr.Add(selfPosInt, this);
        if (stillLoading) {
            stillLoading = false;
        }
    }

    // Update is called once per frame
    /*void Update()
    {
        vertices[0].y+=.1f;
        vertices[1].y+=.1f;
        vertices[size + 1].y+=.1f;
        vertices[size + 2].y+=.1f;
        mesh.vertices = vertices;
    }*/
}
