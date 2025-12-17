
using UnityEngine;


public class MeshDebugger : MonoBehaviour
{
    // Reference to the mesh
    private Mesh mesh;

    void Start()
    {
        // Try to get mesh from MeshFilter first
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            mesh = meshFilter.sharedMesh;
        }
        else
        {
            // If no mesh filter or no mesh, try SkinnedMeshRenderer
            SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null)
            {
                mesh = skinnedMeshRenderer.sharedMesh;
            }
            else
            {
                Debug.LogError("No mesh found on GameObject. Please assign a mesh to either MeshFilter or SkinnedMeshRenderer.");
                return;
            }
        }

        // Show debug info
        PrintMeshInfo();
    }

    void PrintMeshInfo()
    {
        // Check if there are multiple submeshes
        int subMeshCount = mesh.subMeshCount;
        
        Debug.Log($"Submesh Count: {subMeshCount}");

        if (subMeshCount > 1)
        {
            // Loop through each submesh
            for (int submeshIndex = 0; submeshIndex < subMeshCount; submeshIndex++)
            {
                Debug.Log($"--- Submesh {submeshIndex} ---");
                
                // Get the triangles for this submesh
                int[] triangles = mesh.GetTriangles(submeshIndex);
                
                // Get vertices, normals, colors, UVs for this submesh
                Vector3[] vertices = mesh.vertices;
                Vector3[] normals = mesh.normals;
                Color[] colors = mesh.colors;
                Vector2[] uvs = mesh.uv;

                Debug.Log($"Submesh {submeshIndex} Triangle Count: {triangles.Length / 3}");
                Debug.Log($"Submesh {submeshIndex} UV Count: {uvs.Length}");


                // Loop through all triangles in this submesh
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int v0 = triangles[i];
                    int v1 = triangles[i + 1];
                    int v2 = triangles[i + 2];

                    Debug.Log($"Triangle {i / 3}: Vertices [{v0}, {v1}, {v2}]");
                }
            }
        }
        else
        {
            // Original functionality for single submesh
            // Get vertices, normals, colors, UVs, and triangles
            Vector3[] vertices = mesh.vertices;
            Vector3[] normals = mesh.normals;
            Color[] colors = mesh.colors;
            Vector2[] uvs = mesh.uv;
            int[] triangles = mesh.triangles;

            Debug.Log($"Mesh Name: {mesh.name}");
            Debug.Log($"Total Vertices: {vertices.Length}");
            Debug.Log($"Total Triangles: {triangles.Length / 3}");
            Debug.Log($"Total UVs: {uvs.Length}");

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
}