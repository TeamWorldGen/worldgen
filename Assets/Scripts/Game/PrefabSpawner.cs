using UnityEngine;
using UnityEngine.Networking;

public class PrefabSpawner : MonoBehaviour {

    private static PrefabSpawner instance;
    public static PrefabSpawner Instance {
        get { return instance; }
    }

    public GameObject[] trees;

    void Awake() {
        instance = this;
    }

    void Start() {
        Debug.Log("Registering prefabs");
        foreach (GameObject rock in trees) {
            ClientScene.RegisterPrefab(rock);
        }
    }

    public void SpawnTrees(int maxAmount, float lowerY, float upperY) {
        int size = WorldManager.Instance.size;
        float[,] heightMap = WorldManager.Instance.heightMap;
        float[,] falloffMap = WorldManager.Instance.falloffMap;
        
        int halfSize = size / 2;

        for (int i = 0; i < maxAmount; i++) {

            int x = Random.Range(0, heightMap.GetLength(0) - 1);
            int z = Random.Range(0, heightMap.GetLength(1) - 1);

            float xPos = x - halfSize;
            float zPos = z - halfSize;
            float yPos = heightMap[x, z];

            Vector3 pos = new Vector3(xPos, yPos, halfSize-z);
            SpawnTree(pos);
        }
    }

    void SpawnTree(Vector3 pos) {
        GameObject prefab = trees[Random.Range(0, trees.Length)];
        float scale = Random.Range(0.5f, 1.5f);
        Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        GameObject tree = Instantiate(prefab, pos, rotation);
        NetworkServer.Spawn(tree);
    }

}
