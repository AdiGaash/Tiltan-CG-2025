using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshDebugger : MonoBehaviour
{
    // Reference to the mesh
    private Mesh mesh;

    void Start()
    {
        // Get the mesh from the MeshFilter
        mesh = GetComponent<MeshFilter>().mesh;

        // Show debug info
        PrintMeshInfo();
    }

    void PrintMeshInfo()
    {
        // Get vertices, normals, colors, UVs, and triangles
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Color[] colors = mesh.colors;
        Vector2[] uvs = mesh.uv;
        int[] triangles = mesh.triangles;

        Debug.Log($"Mesh Name: {mesh.name}");
        Debug.Log($"Total Vertices: {vertices.Length}");
        Debug.Log($"Total Triangles: {triangles.Length / 3}");

        // Loop through all vertices
        for (int i = 0; i < vertices.Length; i++)
        {
            string vertexInfo = $"Vertex {i}: Position {vertices[i]}";

            if (normals.Length > 0)
                vertexInfo += $", Normal {normals[i]}";

            if (colors.Length > 0)
                vertexInfo += $", Color {colors[i]}";

            if (uvs.Length > 0)
                vertexInfo += $", UV {uvs[i]}";

            Debug.Log(vertexInfo);
        }

        // Loop through all triangles
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int v0 = triangles[i];
            int v1 = triangles[i + 1];
            int v2 = triangles[i + 2];

            Debug.Log($"Triangle {i / 3}: Vertices [{v0}, {v1}, {v2}]");
        }
    }
}