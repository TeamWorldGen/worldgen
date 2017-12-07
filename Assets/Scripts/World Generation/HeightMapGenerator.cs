using UnityEngine;

public static class HeightMapGenerator {

    public static float[,] GenerateHeightMap(int size, int seed, float scale, int octaves, float persistence, float lacunarity, float heightMultiplier, AnimationCurve heightCurve, float[,] falloffMap) {
        float[,] heightMap = new float[size, size];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++) {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        //avoiding division-with-zero error
        if (scale <= 0) {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        //makes changes of noise scale focus to the middle of the screen, instead of top right corner
        float halfWidth = size / 2f;
        float halfHeight = size / 2f;

        //loop through the noise map
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                //work done for each iteration, each on more specific level than the previous, calculated by amplitude, frequency and noiseHeight
                for (int i = 0; i < octaves; i++) {

                    //sample the width and height values
                    float sampleX = (x - halfWidth) * (1 / scale) * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) * (1 / scale) * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;//(*2-1) lets the PerlinValue be able to be less than 0, as PerlinNoise generally just gives a number between 0 and 1
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;//decreases
                    frequency *= lacunarity;//increases
                }
                //general handling for max and min values
                if (noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight = noiseHeight;
                } else if (noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                }
                heightMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                heightMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, heightMap[x, y]);//number between 0 and 1
                float falloffMultiplier = 1;
                if (falloffMap != null)
                    falloffMultiplier = 1 - falloffMap[x, y];
                heightMap[x, y] = heightCurve.Evaluate((heightMap[x, y])) * heightMultiplier * falloffMultiplier;
            }
        }

        return heightMap;
    }

}