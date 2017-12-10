using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BiomeMapGenerator {

    public static float[,] GetLocalTempMap(int size, int offsetX, int offsetY) {
        float[,] tempMap = new float[size, size];
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                float sampleX = (float)(offsetX + x) / size * 0.2f;
                float sampleY = (float)(offsetY + y) / size * 0.2f;
                tempMap[x, y] = Mathf.PerlinNoise(sampleX, sampleY);
            }
        }
        return tempMap;
    }

    public static float[,] GetLocalHumidityMap(int size, int offsetX, int offsetY) {
        float[,] humidityMap = new float[size, size];
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                float sampleX = (float)(offsetX + x) / size;
                float sampleY = (float)(offsetY + y) / size;
                humidityMap[x, y] = Mathf.PerlinNoise(sampleX, sampleY);
            }
        }
        return humidityMap;
    }

}
