using UnityEngine;

public class MeshDisplacer : MonoBehaviour
{
    public MeshFilter meshFilter;   // Reference to the mesh to displace
    public Texture2D depthMap;      // Grayscale depth map
    public float heightMultiplier = 5f;

    private Mesh mesh;
    private Vector3[] originalVertices;

    void Start()
    {
        if(meshFilter == null || depthMap == null)
        {
            Debug.LogError("Assign MeshFilter and DepthMap!");
            return;
        }

        mesh = meshFilter.mesh;
        originalVertices = mesh.vertices;

        DisplaceVertices();
    }

    void DisplaceVertices()
    {
        Vector3[] vertices = new Vector3[originalVertices.Length];
        Vector2[] uvs = mesh.uv;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 v = originalVertices[i];
            Vector2 uv = uvs[i];
            Color pixel = depthMap.GetPixelBilinear(uv.x, uv.y);
            float depth = pixel.r; // Grayscale assumed in R channel
            v.y += depth * heightMultiplier;
            vertices[i] = v;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }
}