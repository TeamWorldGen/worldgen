using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BiomeMapGenerator {

    public static int mapSize;

    public static float[,] GetLocalTempMap(int size, int offsetX, int offsetY) {
        float[,] tempMap = new float[size, size];
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                float sampleX = (float)(offsetX + x) / mapSize * 2f;
                float sampleY = (float)(offsetY + y) / mapSize * 2f;
                tempMap[x, y] = Mathf.PerlinNoise(sampleX, sampleY);
            }
        }
        return tempMap;
    }

    public static float[,] GetLocalHumidityMap(int size, int offsetX, int offsetY) {
        float[,] humidityMap = new float[size, size];
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                float sampleX = (float)(offsetX + x) / mapSize * 5f;
                float sampleY = (float)(offsetY + y) / mapSize * 5f;
                humidityMap[x, y] = Mathf.PerlinNoise(sampleX, sampleY);
            }
        }
        return humidityMap;
    }

}
