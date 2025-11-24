using UnityEngine;

public class GridMeshGenerator : MonoBehaviour
{
    public int width = 100;   // vertices in X
    public int height = 100;  // vertices in Z
    public float size = 1f;   // distance between vertices

    public MeshFilter meshFilter;

    void Start()
    {
        meshFilter.mesh = GenerateGridMesh(width, height, size);
    }

    Mesh GenerateGridMesh(int w, int h, float s)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[w * h];
        Vector2[] uvs = new Vector2[w * h];
        int[] triangles = new int[(w - 1) * (h - 1) * 6];

        // Vertices & UVs
        for (int z = 0; z < h; z++)
        {
            for (int x = 0; x < w; x++)
            {
                vertices[z * w + x] = new Vector3(x * s, 0, z * s);
                uvs[z * w + x] = new Vector2((float)x / (w - 1), (float)z / (h - 1));
            }
        }

        // Triangles
        int triIndex = 0;
        for (int z = 0; z < h - 1; z++)
        {
            for (int x = 0; x < w - 1; x++)
            {
                int i = z * w + x;
                triangles[triIndex++] = i;
                triangles[triIndex++] = i + w;
                triangles[triIndex++] = i + 1;

                triangles[triIndex++] = i + 1;
                triangles[triIndex++] = i + w;
                triangles[triIndex++] = i + w + 1;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
}