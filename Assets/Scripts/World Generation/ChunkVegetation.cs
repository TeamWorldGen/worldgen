using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TerrainChunk))]
public class ChunkVegetation : MonoBehaviour {

    public int startAmount = 10;
    public int maxAmount = 100;
    public float treeLine = 40f;
    public int maxFailedAttempts = 20;
    public Plantable[] vegetation;
    private float[,] heightMap, tempMap, humidityMap;
    private int size, actualSize, offsetX, offsetY;
    private List<GameObject> spawnedPrefabs;
    private Transform parent;

    public void Initialize() {
        TerrainChunk chunk = GetComponent<TerrainChunk>();
        heightMap = chunk.heightMap;
        tempMap = chunk.tempMap;
        humidityMap = chunk.humidityMap;
        size = chunk.size - 1;
        offsetX = chunk.offsetX;
        offsetY = chunk.offsetY;

        spawnedPrefabs = new List<GameObject>();

        parent = GameObject.Find("Environment").transform;
    }

    public void BuildVegetation() {
        SpawnPrefabs();
    }

    public void ResetVegetation() {
        foreach (GameObject prefab in spawnedPrefabs) {
            Destroy(prefab);
        }
        spawnedPrefabs.Clear();
    }

    public void SpawnPrefabs() {
        if (vegetation.Length < 1)
            return;

        int halfSize = WorldManager.Instance.size / 2;

        //List<SpawnInfo> prefabs = new List<SpawnInfo>();
        List<SpawnInfo> spawnInfoList = new List<SpawnInfo>();

        // Create a new random using the world seed + chunk offset
        System.Random rn = new System.Random(WorldManager.Instance.seed + offsetX + offsetY);

        // Start with a few random placed plantables
        int count = startAmount;
        while (count > 0) {
            // Get random x and y values
            int x = rn.Next(0, size);
            int y = rn.Next(0, size);

            // Check tree line
            float height = heightMap[x, y];
            if (height <= treeLine) {
                // Add plantable to spawn list
                Plantable plantable = vegetation[rn.Next(0, vegetation.Length)];
                spawnInfoList.Add(new SpawnInfo(plantable, x, y));
                count--;
            }
        }

        // Grow more plantables based on the ones already placed
        int currentPlantable = 0, failedAttempts = 0;
        while(spawnInfoList.Count < maxAmount && failedAttempts < maxFailedAttempts) {
            SpawnInfo spawnInfo = spawnInfoList[currentPlantable];

            // Calcualte x and y values for the new plantable to be planted
            int newX = -1, newY = -1;
            while (newX < 0 || newX > size || newY < 0 || newY > size) {
                newX = spawnInfo.x + rn.Next(5, 20) * (rn.Next(0, 2) * 2 - 1);
                newY = spawnInfo.y + rn.Next(5, 20) * (rn.Next(0, 2) * 2 - 1);
            }

            // Check tree line
            float height = heightMap[newX, newY];
            if (height <= treeLine) {

                // Spawn new plantable based on probability
                float probability = CalculateSpawnProbability(spawnInfo.plantable, newX, newY);
                if (rn.Next(101) < probability * 100) {
                    // Add plantable to spawn list
                    SpawnInfo newSpawnInfo = new SpawnInfo(spawnInfo.plantable, newX, newY);
                    spawnInfoList.Add(newSpawnInfo);
                } else {
                    failedAttempts++;
                }

            } else {
                failedAttempts++;
            }

            if (++currentPlantable == spawnInfoList.Count)
                currentPlantable = 0;
        }

        for (int i = 0; i < spawnInfoList.Count; i++) {
            SpawnInfo spawnInfo = spawnInfoList[i];
            float xPos = (offsetX + spawnInfo.x) - halfSize;
            float zPos = halfSize - (offsetY + spawnInfo.y);
            float yPos = heightMap[spawnInfo.x, spawnInfo.y];
            Vector3 spawnPos = new Vector3(xPos, yPos, zPos);
            SpawnPrefab(spawnInfo.plantable.gameObject, spawnPos);
        }

    }

    // Spawn a prefab with random rotation and a small variation in scale
    public void SpawnPrefab(GameObject prefab, Vector3 pos) {
        float scale = Random.Range(0.8f, 1.2f);
        Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        GameObject obj = Instantiate(prefab, pos, rotation);
        if (parent != null)
            obj.transform.parent = parent;
        obj.transform.localScale = Vector3.one * scale;
        spawnedPrefabs.Add(obj);
    }

    // Calculate spawn probability based on biome and plantable data
    public float CalculateSpawnProbability(Plantable plantable, int x, int y) {
        float temp = tempMap[x, y];
        if (temp < plantable.minTemp || temp > plantable.maxTemp)
            return 0;

        float humidity = humidityMap[x, y];
        float fertility = plantable.fertility;

        return humidity * fertility;
    }

}

public struct SpawnInfo {
    public Plantable plantable;
    public int x, y;
    public SpawnInfo(Plantable plantable, int x, int y) {
        this.plantable = plantable;
        this.x = x;
        this.y = y;
    }
}

[System.Serializable]
public struct VegetationZone {
    public float minTemp, maxTemp;
    public GameObject[] prefabs;
}
