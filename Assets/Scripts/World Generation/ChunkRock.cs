using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TerrainChunk))]
public class ChunkRock : MonoBehaviour
{

    public int startAmount = 10;
    public int maxAmount = 100;
    public int maxFailedAttempts = 20;
    public GameObject[] rocks;
    private float[,] heightMap;
    private int size, actualSize, offsetX, offsetY;
    private List<GameObject> spawnedPrefabs;
    private Transform parent;

    public void Initialize()
    {
        TerrainChunk chunk = GetComponent<TerrainChunk>();
        heightMap = chunk.heightMap;
        size = chunk.size - 1;
        offsetX = chunk.offsetX;
        offsetY = chunk.offsetY;

        spawnedPrefabs = new List<GameObject>();

        parent = GameObject.Find("Environment").transform;
    }

    public void BuildRock()
    {
        SpawnPrefabs();
    }

    public void ResetRock()
    {
        foreach (GameObject prefab in spawnedPrefabs)
        {
            Destroy(prefab);
        }
        spawnedPrefabs.Clear();
    }

    public void SpawnPrefabs()
    {

        // Get spawn list
        List<RockSpawnInfo> spawnList = RockGenerator.GetSpawnList(rocks, size, offsetX, offsetY, startAmount, maxAmount, maxFailedAttempts);

        // Spawn rocks from spawn list
        int halfSize = WorldManager.Instance.size / 2;
        for (int i = 0; i < spawnList.Count; i++)
        {
            RockSpawnInfo spawnInfo = spawnList[i];
            float xPos = (offsetX + spawnInfo.x) - halfSize;
            float zPos = halfSize - (offsetY + spawnInfo.y);
            float yPos = heightMap[spawnInfo.x, spawnInfo.y];
            Vector3 spawnPos = new Vector3(xPos, yPos, zPos);
            SpawnPrefab(spawnInfo.gameObject, spawnPos);
        }

    }

    // Spawn a prefab with random rotation and a small variation in scale
    public void SpawnPrefab(GameObject prefab, Vector3 pos)
    {
        float scale = Random.Range(0.8f, 1.2f);
        Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        GameObject obj = Instantiate(prefab, pos, rotation);
        if (parent != null)
            obj.transform.parent = parent;
        obj.transform.localScale = Vector3.one * scale;
        spawnedPrefabs.Add(obj);
    }

}

[System.Serializable]
public struct RockZone
{
    public float minTemp, maxTemp;
    public GameObject[] prefabs;
}
