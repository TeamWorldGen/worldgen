using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RockGenerator
{

    public static List<RockSpawnInfo> GetSpawnList(GameObject[] rocks, int size, int offsetX, int offsetY, int startAmount, int maxAmount, int maxFailedAttempts)
    {
        if (rocks.Length < 1)
            return null;

        //List<SpawnInfo> prefabs = new List<SpawnInfo>();
        List<RockSpawnInfo> spawnList = new List<RockSpawnInfo>();

        // Create a new random using the world seed + chunk offset
        System.Random rn = new System.Random(WorldManager.Instance.seed + offsetX + offsetY);

        // Start with a few random placed rocks
        int count = startAmount;
        while (count > 0)
        {
            // Get random x and y values
            int x = rn.Next(0, size);
            int y = rn.Next(0, size);

            
            // Add rock to spawn list
            GameObject gameObject = rocks[rn.Next(0, rocks.Length)];
            spawnList.Add(new RockSpawnInfo(gameObject, x, y));
            count--;
        }

        return spawnList;
    }

}

public struct RockSpawnInfo
{
    public GameObject gameObject;
    public int x, y;
    public RockSpawnInfo(GameObject gameObject, int x, int y)
    {
        this.gameObject = gameObject;
        this.x = x;
        this.y = y;
    }
}