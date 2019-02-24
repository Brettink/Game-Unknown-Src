using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

//Also a world manager
public class ChunkManager : MonoBehaviour
{
    public static ChunkManager self;
    public static List<ChunkArea> currAreas = new List<ChunkArea>();
    public static Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
    private bool myDone = false;
    private bool canLoad = false;
    public static string persisPath = string.Empty;
    public static string chunksPath = string.Empty;
    public static WorldInfo worldInfo;
    BinaryFormatter binFormatter = new BinaryFormatter();
    IEnumerator chunkCheck() {

        yield return null;
    }

    public void Awake() {
        self = this;
        string lvlPath = Path.Combine(Application.persistentDataPath, GManager.worldName + "/");
        string chPath = Path.Combine(lvlPath, "c/");
        if (!Directory.Exists(lvlPath)) {
            Directory.CreateDirectory(lvlPath);
            Directory.CreateDirectory(chPath);
            canLoad = false;
        } else {
            canLoad = true;
        }
        persisPath = lvlPath;
        chunksPath = chPath;
    }

    public void SaveAndUnload() {

    }

    public void SaveWorldAndLoadedChunks() {
        int size = GManager.MAX_CHUNK_SIZE;
        foreach (KeyValuePair<Vector2Int, Chunk> ch in chunks) {
            int iX = ch.Key.x / size;
            int iZ = ch.Key.y / size;
            ChunkArea area = currAreas.Find(a => a.iX == iX && a.iZ == iZ);
            ChunkInfo chI = ch.Value.getChunkInfo();
            if (area != null) {
                Vector3 pos = new Vector3(ch.Value.selfPosInt.x, 0, ch.Value.selfPosInt.y);
                area.chunks.TryGetValue(pos, out ChunkInfo inf);
                if (inf == null) {
                    area.chunks.Add(pos, chI);
                } else {
                    area.chunks[pos] = chI;
                }
            }
        }
        currAreas.ForEach(a => {
            using (FileStream fileStream = 
            File.Open(chunksPath + "chA" + a.iX + "" + a.iZ + ".dat", FileMode.OpenOrCreate)) {
                binFormatter.Serialize(fileStream, a);
            }
        });
        SaveWorld();
    }

    public void SaveWorld() {
        worldInfo.playerPos = GManager.playerLocation.position;
        using (FileStream fileStream = File.Open(persisPath + "w.dat", FileMode.OpenOrCreate)) {
            binFormatter.Serialize(fileStream, worldInfo);
        }
    }
    public static bool doneFirstload = false;
    public IEnumerator LoadChunk(Vector3 posIn, ChunkInfo info) {
        Vector3Int chunkStart = new Vector3Int((int)posIn.x, 0, (int)posIn.z);
        GameObject newChunk = Instantiate(GManager.self.empty, chunkStart, Quaternion.identity, transform);
        Chunk ch = newChunk.GetComponent<Chunk>();
        newChunk.name = "chunk" + posIn.x + " " + posIn.z + "";
        int selName = 0;
        Loader.loading = true;
        float[,] heights = new float[GManager.MAX_CHUNK_SIZE, GManager.MAX_CHUNK_SIZE];
        for (int i = 0; i < info.tiles.Keys.Count; i++, selName++) {
            info.tiles.TryGetValue(i, out TileInfo tile);
            float z = (int)(i / 25f);
            float x = (int)(i % 25f);
            Vector3 pos = new Vector3(x-12f, tile.height, z-12f);
            GameObject newTile = Instantiate(GManager.self.TilePrefab, pos, Quaternion.identity, newChunk.transform);
            adjustment adj = newTile.GetComponent<adjustment>();
            GameObject arrSel = newTile.transform.GetChild(3).gameObject;
            arrSel.name = posIn.x + "" + posIn.z + selName;
            adj.type = tile.type;
            if (tile.placerName != string.Empty) {
                adj.SetPlacer(GManager.items[tile.placerName] as IPlaceable);
            }
            heights[(int)x, (int)z] = pos.y;
            if ((int)x == GManager.MAX_CHUNK_SIZE-1) { yield return null; }
        }
        ch.setAllHeights(heights);
        chunks.Add(new Vector2Int((int)posIn.x, (int)posIn.z), ch);
        Loader.loading = false;
        if (!doneFirstload) {
            doneFirstload = true;
            GManager.playerLocation.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            GManager.GSState = GS.N;
        }
        yield return null;
    }

    public void Update() {
        if (GManager.done && !myDone) {
            int size = GManager.MAX_CHUNK_SIZE;
            if (!canLoad) {
                StartCoroutine(GManager.self.generate(Vector2.zero));
            } else {
                using (FileStream fileStream = File.Open(persisPath + "w.dat",FileMode.Open)) {
                    worldInfo = (WorldInfo)binFormatter.Deserialize(fileStream);
                    GManager.self.LoadDefault(worldInfo);
                }
                int aX = (int)worldInfo.playerPos.x / (size * size);
                int aZ = (int)worldInfo.playerPos.z / (size * size);
                using (FileStream fileStream = File.Open(chunksPath + "chA" +aX+""+aZ+".dat", FileMode.Open)) {
                    ChunkArea chArea = (ChunkArea)binFormatter.Deserialize(fileStream);
                    chArea.chunks.Where(ch => Vector3.Distance(worldInfo.playerPos, ch.Key) < 50f)
                        .ToList().ForEach(ch => StartCoroutine(LoadChunk(ch.Key, ch.Value)));
                    currAreas.Add(chArea);
                }
                GManager.self.LoadPlayerInfo();
            }
            myDone = true;
        }
        if (myDone) {
            if (Input.GetKeyUp(KeyCode.S)) {
                SaveWorldAndLoadedChunks();
            }
        }
    }


}
