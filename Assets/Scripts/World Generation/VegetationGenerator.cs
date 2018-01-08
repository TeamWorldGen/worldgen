﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VegetationGenerator {

    public static List<PlantableSpawnInfo> GetSpawnList(Plantable[] vegetation, int size, int offsetX, int offsetY, int startAmount, int maxAmount, int maxFailedAttempts, float treeLine, float[,] heightMap, float[,] tempMap, float[,] humidityMap) {
        if (vegetation.Length < 1)
            return null;

        //List<SpawnInfo> prefabs = new List<SpawnInfo>();
        List<PlantableSpawnInfo> spawnList = new List<PlantableSpawnInfo>();

        // Create a new random using the world seed + chunk offset
        System.Random rn = new System.Random(WorldManager.Instance.seed + offsetX + offsetY);

        // Start with a few random placed plantables
        int count = startAmount;
        int failedAttempts = 0;
        while (count > 0 && failedAttempts < maxFailedAttempts) {

            // Get random x and y values
            int x = rn.Next(0, size);
            int y = rn.Next(0, size);

            // Check tree line
            float height = heightMap[x, y];
            Plantable plantable = vegetation[rn.Next(0, vegetation.Length)];
            float probability = CalculateSpawnProbability(plantable, x, y, tempMap, humidityMap);
            if (rn.Next(101) < probability * 100 && height <= treeLine) {
                // Add plantable to spawn list
                Quaternion rotation = Quaternion.Euler(new Vector3(0, rn.Next(0, 360), 0));
                spawnList.Add(new PlantableSpawnInfo(plantable, x, y, rotation));
                count--;
            } else {
                failedAttempts++;
            }
        }

        if (spawnList.Count == 0)
            return null;

        // Grow more plantables based on the ones already placed
        failedAttempts = 0;
        int currentPlantable = 0;
        while (spawnList.Count < maxAmount && failedAttempts < maxFailedAttempts) {
            PlantableSpawnInfo spawnInfo = spawnList[currentPlantable];

            // Calcualte x and y values for the new plantable to be planted
            int newX = -1, newY = -1;
            while (newX < 0 || newX > size || newY < 0 || newY > size) {
                int min = spawnInfo.plantable.spread;
                int max = spawnInfo.plantable.spread + (spawnInfo.plantable.spread * 2);
                newX = spawnInfo.x + rn.Next(min, max) * (rn.Next(0, 2) * 2 - 1);
                newY = spawnInfo.y + rn.Next(min, max) * (rn.Next(0, 2) * 2 - 1);
            }

            // Check tree line
            float height = heightMap[newX, newY];
            if (height <= treeLine) {

                // Spawn new plantable based on probability
                float probability = CalculateSpawnProbability(spawnInfo.plantable, newX, newY, tempMap, humidityMap);
                if (rn.Next(101) < probability * 100) {
                    // Add plantable to spawn list
                    Quaternion rotation = Quaternion.Euler(new Vector3(0, rn.Next(0, 360), 0));
                    PlantableSpawnInfo newSpawnInfo = new PlantableSpawnInfo(spawnInfo.plantable, newX, newY, rotation);
                    spawnList.Add(newSpawnInfo);
                } else {
                    failedAttempts++;
                }

            } else {
                failedAttempts++;
            }

            if (++currentPlantable == spawnList.Count)
                currentPlantable = 0;
        }

        return spawnList;
    }

    // Calculate spawn probability based on biome and plantable data
    public static float CalculateSpawnProbability(Plantable plantable, int x, int y, float[,] tempMap, float[,] humidityMap) {
        float temp = tempMap[x, y];
        if (temp < plantable.minTemp || temp > plantable.maxTemp)
            return 0;

        float humidity = humidityMap[x, y];
        float fertility = plantable.fertility;

        return humidity * fertility;
    }

}

public struct PlantableSpawnInfo {
    public Plantable plantable;
    public int x, y;
    public Quaternion rotation;
    public PlantableSpawnInfo(Plantable plantable, int x, int y, Quaternion rotation) {
        this.plantable = plantable;
        this.x = x;
        this.y = y;
        this.rotation = rotation;
    }
}