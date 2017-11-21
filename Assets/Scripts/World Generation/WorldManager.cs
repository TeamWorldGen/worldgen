using UnityEngine;
using System;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour {

    private static WorldManager instance;
    public static WorldManager Instance {
        get { return instance; }
    }

    private int chunkSize = 100;
    [Header("Chunk Settings")]
    public int chunksPerLine = 5;
    public int renderDistance = 5;

    [Header("Map Generation Settings")]
    public int seed = 0;//plant a seed and see what you get(sets a seed for the random number generator)
    public float heightMultiplier = 1f;
    public float scale = 1f;
    public int octaves;//each octave represents a coherent-noise function, which (in plural) the Perlin-noise function is made up of (integer of how many iterations)
    [Range(0,1)]
    public float persistance;//scales between 0(barren fields) and 1(overly bumpy landscape), (should be set to 0.5)
    public float lacunarity;//a multiplier that determines how quickly the frequency increases for each successive octave in the Perlin-noise function
    public AnimationCurve heightCurve;

    [Header("Other")]
    public Transform player;
    public GameObject chunkPrefab;
    public Transform parent;

    private TerrainChunk[,] chunks;

    public int size;
    private int actualSize;
    private int actualChunkSize;

    public float[,] heightMap;
    public float[,] falloffMap;

    public static Queue<ChunkData> drawMeshQueue = new Queue<ChunkData>();
    private Queue<ChunkPos> createChunkQueue = new Queue<ChunkPos>();

    private bool ready = false;
    private bool chunksReady = false;

    void Awake() {
        instance = this;

        Initialize();
    }

    void OnValidate() {
        if (lacunarity < 1) {//because setting lacunarity to <1 would cause the earth to be flat, and that's just absurd, isn't it?
            lacunarity = 1;
        }
        if (octaves < 0) {//there can't be a negative number of iterations. 0 iterations results in 0 terrain
            octaves = 0;
        }
        if (renderDistance < 1) { // Render distance must be greater than 0
            renderDistance = 1;
        }
    }

#if CLIENT
    int chunkCount = 1;
    void Update() {
        if (!chunksReady)
            return;

        if (createChunkQueue.Count > 0) {
            LoadingScreen.SetSubtitle("Creating chunk " + chunkCount);
            ChunkPos chunkPos = createChunkQueue.Dequeue();
            CreateTerrainChunk(chunkPos.x, chunkPos.y);
            chunkCount++;
        } else if (!ready) {
            NetworkController.Instance.OnChunksReady();
            ready = true;
        }

        if (!ready || player == null)
            return;

        UpdateTerrainChunks(player.position);

        // Only create one mesh per frame to prevent freeze n shit
        if (drawMeshQueue.Count > 0) {
            ChunkData data = drawMeshQueue.Dequeue();
            data.chunk.UpdateMesh(data.meshData);
        }
    }
#endif

    private void Initialize() {
        size = chunkSize * chunksPerLine;
        int length = size / chunkSize;
        actualSize = size + length;
        actualChunkSize = chunkSize + 1;

        falloffMap = FalloffMapGenerator.GenerateFalloffMap(actualSize);
        heightMap = HeightMapGenerator.GenerateHeightMap(actualSize, seed, scale, octaves, persistance, lacunarity, heightMultiplier, heightCurve, falloffMap);
    }

#if CLIENT || UNITY_EDITOR
    public void CreateTerrainChunks() {
        chunks = new TerrainChunk[chunksPerLine, chunksPerLine];
        for (int y = 0; y < chunksPerLine; y++) {
            for (int x = 0; x < chunksPerLine; x++) {
                createChunkQueue.Enqueue(new ChunkPos(x, y));
            }
        }
        chunksReady = true;
    }

    private void CreateTerrainChunk(int x, int y) {
        // Get the chunk map for this chunk
        float[,] chunkMap = GetChunkMap(heightMap, chunkSize * x, chunkSize * y);

        // Caculate spawn position for this chunk
        float startX = transform.position.x - (size / 2) + (chunkSize / 2);
        float startZ = transform.position.z + (size / 2) - (chunkSize / 2);
        Vector3 spawnPos = new Vector3(startX + chunkSize * x, 0, startZ - chunkSize * y);

        // Spawn the chunk and generate its terrain
        TerrainChunk chunk = Instantiate(chunkPrefab, spawnPos, Quaternion.identity).GetComponent<TerrainChunk>();
        chunk.transform.parent = parent;
        chunk.Initialize(chunkMap);
        //chunk.UpdateChunk(1, false);

        chunks[x, y] = chunk;
    }

    public void UpdateTerrainChunks(Vector3 playerPos) {

        // Find what chunk player is located in
        int currentX = -1, currentY = -1;
        for (int y = 0; y < chunksPerLine; y++) {
            for (int x = 0; x < chunksPerLine; x++) {
                if (chunks[x, y].PosInChunk(playerPos)) {
                    currentX = x;
                    currentY = y;
                }
            }
        }

        // Return if player is not in a chunk (probably out swimming)
        if (currentX < 0 || currentY < 0)
            return;

        // Loop through all chunks and update them based on player position
        for (int y = 0; y < chunksPerLine; y++) {
            for (int x = 0; x < chunksPerLine; x++) {

                int xDistFromPlayer = Math.Abs(currentX - x);
                int yDistFromPlayer = Math.Abs(currentY - y);

                int dist = Mathf.Max(xDistFromPlayer, yDistFromPlayer);
                if (dist <= renderDistance) {
                    int lod = GetLOD(dist);
                    UpdateChunk(x, y, true, lod);
                } else {
                    UpdateChunk(x, y, false, 24);
                }

            }
        }

    }

    private int GetLOD(int dist) {
        switch (dist) {
            case 0:
            case 1:
                return 1;
            case 2:
                return 2;
            case 3:
                return 4;
            case 4:
                return 5;
            case 5:
                return 10;
            default:
                return 25;
        }
    }

    // Update a selected chunk
    public void UpdateChunk(int x, int y, bool active, int lod) {
        if (x < 0 || x >= chunksPerLine || y < 0 || y >= chunksPerLine)
            return;
        chunks[x, y].UpdateChunk(lod, active);
    }

    // Called when a chunk is done updating and ready to be drawn
    public void OnChunkUpdated(ChunkData data) {
        drawMeshQueue.Enqueue(data); // Add data to draw queue
    }

    // Get the height map for a selected chunk
    private float[,] GetChunkMap(float[,] heightMap, int startX, int startY) {
        float[,] map = new float[actualChunkSize, actualChunkSize];
        for (int y = 0; y < actualChunkSize; y++) {
            for (int x = 0; x < actualChunkSize; x++) {
                map[x, y] = heightMap[startX + x, startY + y];
            }
        }
        return map;
    }
#endif

#if UNITY_EDITOR
    // ******************************
    //         EDITOR STUFF
    // ******************************
    public void OnEditorGenerate() {
        /*
        OnEditorReset();
        Initialize();
        CreateTerrainChunks();
        */
    }

    public void OnEditorReset() {
        /*
        foreach (GameObject chunk in GameObject.FindGameObjectsWithTag("TerrainChunk")) {
            DestroyImmediate(chunk);
        }
        */
    }
#endif

}

public struct ChunkPos {
    public readonly int x, y;
    public ChunkPos(int x, int y) {
        this.x = x;
        this.y = y;
    }
}
