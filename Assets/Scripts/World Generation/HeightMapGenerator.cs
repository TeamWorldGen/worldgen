using UnityEngine;

public static class HeightMapGenerator {

    public static int seed;
    public static float scale;
    public static int octaves;
    public static float persistence;
    public static float lacunarity;
    public static float heightMultiplier;
    public static AnimationCurve heightCurve;
    public static bool useFalloffMap;

    public static float[,] GetHeightMap(int size, int offsetX, int offsetY, float[,] falloffMap) {
        float[,] heightMap = new float[size, size];

        float amplitude = 1;
        float maxHeight = 0;

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++) {
            float octaveOffsetX = prng.Next(-100000, 100000) + offsetX;
            float octaveOffsetY = prng.Next(-100000, 100000) + offsetY;
            octaveOffsets[i] = new Vector2(octaveOffsetX, octaveOffsetY);

            maxHeight += amplitude;
            amplitude *= persistence;
        }

        //avoiding division-with-zero error
        if (scale <= 0) {
            scale = 0.0001f;
        }

        //loop through the noise map
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {

                amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                //work done for each iteration, each on more specific level than the previous, calculated by amplitude, frequency and noiseHeight
                for (int i = 0; i < octaves; i++) {

                    //sample the width and height values
                    float sampleX = (x + octaveOffsets[i].x) * (1 / scale) * frequency;
                    float sampleY = (y + octaveOffsets[i].y) * (1 / scale) * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;//(*2-1) lets the PerlinValue be able to be less than 0, as PerlinNoise generally just gives a number between 0 and 1
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;//decreases
                    frequency *= lacunarity;//increases
                }

                heightMap[x, y] = noiseHeight;

                float falloffMultiplier = 1;
                if (useFalloffMap && falloffMap != null)
                    falloffMultiplier = falloffMap[x, y];

                float normalized = (heightMap[x, y] + 1) / (maxHeight);
                heightMap[x, y] = heightCurve.Evaluate(Mathf.Clamp(normalized, 0, int.MaxValue)) * heightMultiplier * falloffMultiplier;
            }
        }

        return heightMap;
    }

}