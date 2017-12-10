using UnityEngine;

public static class FalloffMapGenerator {

    public static int mapSize;
    public static Texture2D texture;

    public static float[,] GetLocalFalloffMap(int size, int offsetX, int offsetY) {
        float[,] falloffMap = new float[size, size];

        float multiplier = (float)texture.width / (float)mapSize;

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {

                int hx = Mathf.FloorToInt((x + offsetX) * multiplier);
                int hy = Mathf.FloorToInt((y + offsetY) * multiplier);

                falloffMap[x, y] = texture.GetPixel(hx, hy).grayscale;
            }
        }

        return falloffMap;
    }

}
