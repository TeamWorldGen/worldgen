using UnityEngine;

public static class MeshGenerator {

    public static MeshData GenerateTerrainMesh(float[,] heightMap, int lod) {
        int size = heightMap.GetLength(0);
        float startX = (size - 1) / -2f;
        float startZ = (size - 1) / 2f;

        int verticesPerLine = (size - 1) / 1 + 1;

        MeshData meshData = new MeshData(verticesPerLine);
        int vertexIndex = 0;

        for (int y = 0; y < size; y += lod) {
            for (int x = 0; x < size; x += lod) {

                if (x < size - 1 && y < size - 1) {

                    // Add triangle 1
                    meshData.vertices[vertexIndex] = new Vector3(startX + x, heightMap[x, y], startZ - y);
                    meshData.vertices[vertexIndex + 1] = new Vector3(startX + x + lod, heightMap[x + lod, y + lod], startZ - y - lod);
                    meshData.vertices[vertexIndex + 2] = new Vector3(startX + x, heightMap[x, y + lod], startZ - y - lod);
                    meshData.uv[vertexIndex] = new Vector2(x / (float)size, y / (float)size);
                    meshData.uv[vertexIndex + 1] = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);
                    meshData.uv[vertexIndex + 2] = new Vector2(x / (float)size, (y + 1) / (float)size);
                    meshData.AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + 2);
                    vertexIndex += 3;

                    // Add triangle 2
                    meshData.vertices[vertexIndex] = new Vector3(startX + x + lod, heightMap[x + lod, y + lod], startZ - y - lod);
                    meshData.vertices[vertexIndex + 1] = new Vector3(startX + x, heightMap[x, y], startZ - y);
                    meshData.vertices[vertexIndex + 2] = new Vector3(startX + x + lod, heightMap[x + lod, y], startZ - y);
                    meshData.uv[vertexIndex] = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);
                    meshData.uv[vertexIndex + 1] = new Vector2(x / (float)size, y / (float)size);
                    meshData.uv[vertexIndex + 2] = new Vector2((x + 1) / (float)size, y / (float)size);
                    meshData.AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + 2);
                    vertexIndex += 3;

                }

            }
        }

        return meshData;
    }

}

public class MeshData {
    public Vector3[] vertices;
    public Vector2[] uv;
    public int[] triangles;

    public MeshData(int size) {
        int length = (size - 1) * (size - 1) * 6;
        vertices = new Vector3[length];
        uv = new Vector2[length];
        triangles = new int[length];
    }

    private int triangleIndex;
    public void AddTriangle(int a, int b, int c) {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }

}