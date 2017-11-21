using UnityEngine;

public static class FalloffMapGenerator {

    // TODO: Improve this ugly function
    public static float[,] GenerateFalloffMap(int size) {
        float[,] falloffMap = new float[size, size];

        int halfSize = size / 2;

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {

                float distX = (halfSize - x) * (halfSize - x);
                float distY = (halfSize - y) * (halfSize - y);

                float distToCenter = Mathf.Sqrt(distX + distY) / halfSize;

                if (distToCenter > 1)
                    distToCenter = 1;
                else if (distToCenter < 0)
                    distToCenter = 0;

                falloffMap[x, y] = distToCenter;
            }
        }

        return falloffMap;
    }

}
