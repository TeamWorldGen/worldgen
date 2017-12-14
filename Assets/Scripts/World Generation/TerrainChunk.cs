﻿using UnityEngine;
using System.Threading;

public class TerrainChunk : MonoBehaviour {

    public Material sourceMaterial;
    public TerrainType[] regions;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    public int size;
    private int lod = 25;
    private bool editor; // true if chunk is created in editor mode
    private bool isVegetated = false;

    public float[,] heightMap;
    public float[,] tempMap;
    public float[,] humidityMap;

    public int offsetX, offsetY;

    private ChunkVegetation vegetation;

    public void Initialize(int size, int offsetX, int offsetY, bool editor = false) {
        this.editor = editor;

        // for debugging
        this.offsetX = offsetX;
        this.offsetY = offsetY;

        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        this.size = size;
        float[,] falloffMap = FalloffMapGenerator.GetFalloffMap(size, offsetX, offsetY);
        heightMap = HeightMapGenerator.GetHeightMap(size, offsetX, offsetY, falloffMap);

        // Create collider for this chunk
        CreateCollider();

        // Set the base texture for this chunk
        tempMap = BiomeMapGenerator.GetLocalTempMap(size, offsetX, offsetY);
        humidityMap = BiomeMapGenerator.GetLocalHumidityMap(size, offsetX, offsetY);
        SetTexture(GetColorMap(tempMap, humidityMap));

        // Update the mesh of this chunk
        UpdateMesh(GetLOD(lod));

        vegetation = GetComponent<ChunkVegetation>();
        if (vegetation != null)
            vegetation.Initialize();
    }

    public void UpdateChunk(int lod, bool active) {
        if (this.lod != lod) {
            this.lod = lod;

            ThreadStart thread = delegate {
                MeshData meshData = GetLOD(lod);
                ChunkData chunkData = new ChunkData(this, meshData);
                lock (WorldManager.drawMeshQueue) {
                    WorldManager.drawMeshQueue.Enqueue(chunkData);
                }
            };
            new Thread(thread).Start();

            /*
            ResetVegetation();
            if (lod <= 4)
                BuildVegetation();
            */

        }

        gameObject.SetActive(active);
    }

    private MeshData GetLOD(int lod) {
        MeshData meshData;
        if (editor)
            meshData = MeshGenerator.GenerateTerrainMesh(heightMap, 4);
        else
            meshData = MeshGenerator.GenerateTerrainMesh(heightMap, lod);
        return meshData;
    }

    private Color[] GetColorMap(float[,] tempMap, float[,] humidityMap) {
        Color[] colorMap = new Color[size * size];

        /*
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {

                float temp = tempMap[x, y];
                float humidity = humidityMap[x, y];

                float red = temp;
                float green = humidity;
                float blue = 1 - temp;

                colorMap[y * size + x] = new Color(red, 0, 0);
            }
        }
        */

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                float currentHeight = heightMap[x, y];
                for (int i = 0; i < regions.Length; i++) {
                    if (currentHeight <= regions[i].height) {
                        colorMap[y * size + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        return colorMap;
    }

    public void UpdateMesh(MeshData meshData) {
        Mesh mesh = meshData.CreateMesh();
        meshFilter.sharedMesh = mesh;
    }

    public void CreateCollider() {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(heightMap, 4);
        Mesh mesh = meshData.CreateMesh();
        meshCollider.sharedMesh = mesh;
    }

    public void SetTexture(Color[] colorMap) {
        Material material = new Material(sourceMaterial);
        meshRenderer.material = material;
        Texture2D texture = TextureGenerator.TextureFromColorMap(colorMap, size, size);
        meshRenderer.sharedMaterial.mainTexture = texture;
    }

    public bool PosInChunk(Vector3 pos) {
        int halfSize = size / 2;
        bool x = pos.x >= transform.position.x - halfSize && pos.x < transform.position.x + halfSize;
        bool z = pos.z >= transform.position.z - halfSize && pos.z < transform.position.z + halfSize;
        return x && z;
    }

    public void BuildVegetation() {
        if (isVegetated)
            return;
        if (vegetation != null)
            vegetation.BuildVegetation();
        isVegetated = true;
    }

    public void ResetVegetation() {
        if (vegetation != null)
            vegetation.ResetVegetation();
        isVegetated = false;
    }

}

public struct ChunkData {
    public readonly TerrainChunk chunk;
    public readonly MeshData meshData;
    public ChunkData(TerrainChunk chunk, MeshData meshData) {
        this.chunk = chunk;
        this.meshData = meshData;
    }
}

[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color color;
}