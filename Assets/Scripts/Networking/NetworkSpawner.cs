using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkSpawner : MonoBehaviour {

    private static NetworkSpawner instance;
    public static NetworkSpawner Instance {
        get { return instance; }
    }

    public List<GameObject> objectsInWorld;

    [Header("Vegetation Params")]
    public int startAmountMultiplier = 1;
    public int maxAmountMultiplier = 1;
    public float treeLine = 40f;
    public int maxFailedAttemptsMultiplier = 1;
    public Plantable[] vegetation;

    private float[,] heightMap, tempMap, humidityMap;
    private Transform parent;

    void Awake() {
        instance = this;

        // Register prefabs
        RegisterPrefabs();

        parent = GameObject.Find("Environment").transform;
    }

    public void Initialize() {

        objectsInWorld = new List<GameObject>();

        int size = WorldManager.Instance.actualSize;
        float[,] falloffMap = FalloffMapGenerator.GetFalloffMap(size, 0, 0);
        heightMap = HeightMapGenerator.GetHeightMap(size, 0, 0, falloffMap);
        tempMap = BiomeMapGenerator.GetLocalTempMap(size, 0, 0);
        humidityMap = BiomeMapGenerator.GetLocalHumidityMap(size, 0, 0);
    }

    public void RegisterPrefabs() {
        foreach (Plantable plantable in vegetation) {
            RegisterPrefab(plantable.gameObject);
        }
    }

    public void RegisterPrefab(GameObject prefab) {
        ClientScene.RegisterPrefab(prefab);
    }

    public void BuildVegetation() {

        int size = WorldManager.Instance.size;

        int startAmount = 10 * WorldManager.Instance.chunksPerLine * startAmountMultiplier;
        int maxAmount = 100 * WorldManager.Instance.chunksPerLine * maxAmountMultiplier;
        int maxFailedAttempts = 20 * WorldManager.Instance.chunksPerLine * maxFailedAttemptsMultiplier;

        // Get spawn list
        List<PlantableSpawnInfo> spawnList = VegetationGenerator.GetSpawnList(vegetation, size, 0, 0, startAmount, maxAmount, maxFailedAttempts, treeLine, heightMap, tempMap, humidityMap);

        if (spawnList == null)
            return;

        // Spawn plantables from spawn list
        int halfSize = WorldManager.Instance.size / 2;
        for (int i = 0; i < spawnList.Count; i++) {
            PlantableSpawnInfo spawnInfo = spawnList[i];
            float xPos = spawnInfo.x - halfSize;
            float zPos = halfSize - spawnInfo.y;
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
        objectsInWorld.Add(obj);
        NetworkServer.Spawn(obj);
    }

}
