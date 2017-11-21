using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;

public class TerrainChunk : MonoBehaviour {

    public Material sourceMaterial;
    public TerrainType[] regions;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    private int size;
    private int lod = 25;

    private float[,] heightMap;

    public void Initialize(float[,] heightMap) {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        this.heightMap = heightMap;
        size = heightMap.GetLength(0) - 1;

        // Create collider for this chunk
        CreateCollider();

        // Set the base texture for this chunk
        SetTexture(GetColorMap());

        // Update the mesh of this chunk
        UpdateMesh(GetLOD(lod));
    }

    public void UpdateChunk(int lod, bool active) {
        if (this.lod != lod) {
            this.lod = lod;

            ThreadStart thread = delegate {
                MeshData meshData = GetLOD(lod);
                ChunkData chunkData = new ChunkData(this, meshData);
                lock(WorldManager.drawMeshQueue) {
                    WorldManager.drawMeshQueue.Enqueue(chunkData);
                }
            };
            new Thread(thread).Start();
        }

        gameObject.SetActive(active);
    }

    private MeshData GetLOD(int lod) {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(heightMap, lod);
        return meshData;
    }

    private Color[] GetColorMap() {
        Color[] colorMap = new Color[size * size];
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
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(heightMap, 1);
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
