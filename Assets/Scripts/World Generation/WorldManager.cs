using UnityEngine;
using System;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour {

    private static WorldManager instance;
    public static WorldManager Instance {
        get { return instance; }
    }

    public readonly int chunkSize = 100;
    [Header("Chunk Settings")]
    public int chunksPerLine = 5;
    public int renderDistance = 5;

    [Header("Map Generation Settings")]
    public int seed = 0;//plant a seed and see what you get(sets a seed for the random number generator)
    public float heightMultiplier = 1f;
    public float scale = 1f;
    public int octaves;//each octave represents a coherent-noise function, which (in plural) the Perlin-noise function is made up of (integer of how many iterations)
    [Range(0, 1)]
    public float persistence;//scales between 0(barren fields) and 1(overly bumpy landscape), (should be set to 0.5)
    public float lacunarity;//a multiplier that determines how quickly the frequency increases for each successive octave in the Perlin-noise function
    public AnimationCurve heightCurve;
    public bool useFalloffMap = true;
    public Texture2D falloffMapTexture;

    [Header("Other")]
    public Transform player;
    public GameObject chunkPrefab;
    public Transform parent;

    private TerrainChunk[,] chunks;

    public int size;
    public int actualSize;
    public int actualChunkSize;

    public static Queue<ChunkData> drawMeshQueue = new Queue<ChunkData>();
    private Queue<ChunkPos> createChunkQueue = new Queue<ChunkPos>();

    private bool ready = false;
    private bool chunksReady = false;

    [Header("Editor Generation")]
    [Range(1, 5)]
    public int chunksPerLineInEditor = 1;

    void Awake() {
        instance = this;

#if CLIENT
        Initialize(); // Initialize locally if this is the client
#endif
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
        if (chunksPerLine < 1) {
            chunksPerLine = 1;
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

    public void Initialize() {
        print("Initializing world manager...");

        size = chunkSize * chunksPerLine;
        actualSize = size + chunksPerLine;
        actualChunkSize = chunkSize + 1;

        // Setup height map generator
        HeightMapGenerator.seed = seed;
        HeightMapGenerator.scale = scale;
        HeightMapGenerator.octaves = octaves;
        HeightMapGenerator.persistence = persistence;
        HeightMapGenerator.lacunarity = lacunarity;
        HeightMapGenerator.heightMultiplier = heightMultiplier;
        HeightMapGenerator.heightCurve = heightCurve;
        HeightMapGenerator.useFalloffMap = useFalloffMap;

        FalloffMapGenerator.mapSize = actualSize;
        FalloffMapGenerator.texture = falloffMapTexture;

        BiomeMapGenerator.mapSize = actualSize;
    }

#if CLIENT || UNITY_EDITOR

    public void CreateTerrainChunks() {
        print("Creating terrain chunks...");
        chunks = new TerrainChunk[chunksPerLine, chunksPerLine];
        for (int y = 0; y < chunksPerLine; y++) {
            for (int x = 0; x < chunksPerLine; x++) {
                createChunkQueue.Enqueue(new ChunkPos(x, y));
            }
        }
        chunksReady = true;
    }

    private void CreateTerrainChunk(int x, int y, bool editor = false) {

        // Caculate spawn position for this chunk
        float startX = transform.position.x - (size / 2) + (chunkSize / 2);
        float startZ = transform.position.z + (size / 2) - (chunkSize / 2);
        Vector3 spawnPos = new Vector3(startX + chunkSize * x, 0, startZ - chunkSize * y);

        // Spawn the chunk
        TerrainChunk chunk = Instantiate(chunkPrefab, spawnPos, Quaternion.identity).GetComponent<TerrainChunk>();
        chunk.transform.parent = parent;

        // Initialize the chunk
        int offsetX = x * chunkSize;
        int offsetY = y * chunkSize;
        chunk.Initialize(actualChunkSize, offsetX, offsetY, editor);

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
                    if (dist < 2)
                        chunks[x, y].BuildVegetation();
                    else
                        chunks[x, y].ResetVegetation();
                } else {
                    UpdateChunk(x, y, false, 24);
                    chunks[x, y].ResetVegetation();
                }

            }
        }

    }

    private int GetLOD(int dist) {
        switch (dist) {
            case 0:
            case 1:
                return 4;
            case 2:
                return 4;
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
#endif

#if UNITY_EDITOR
    // ******************************
    //         EDITOR STUFF
    // ******************************
    public void OnEditorGenerate() {
        OnEditorReset();
        int temp = chunksPerLine;
        chunksPerLine = chunksPerLineInEditor;
        Initialize();
        CreateTerrainChunks();
        chunksPerLine = temp;

        while (createChunkQueue.Count > 0) {
            ChunkPos chunkPos = createChunkQueue.Dequeue();
            CreateTerrainChunk(chunkPos.x, chunkPos.y, true);
        }
    }

    public void OnEditorReset() {
        foreach (GameObject chunk in GameObject.FindGameObjectsWithTag("TerrainChunk")) {
            DestroyImmediate(chunk);
        }
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