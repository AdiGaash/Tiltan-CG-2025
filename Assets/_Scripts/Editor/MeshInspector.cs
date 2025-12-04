using UnityEditor;
using UnityEngine;

public class MeshInspector : MonoBehaviour
{
    public GameObject targetGameObject;
    public bool showTriangles = true;
    public bool showNormals = true;
    public bool showVertexNumbers = false; // New parameter to control vertex number display
    public int maxVertexNumberToShow = int.MaxValue; // New parameter to limit the maximum vertex number shown
    public int maxTrianglesToShow = int.MaxValue; // New parameter to limit the maximum triangles shown
    private Renderer meshRenderer;
    private MeshFilter meshFilter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FindMeshFilter();
        if (meshFilter != null)
        {
            meshRenderer = meshFilter.GetComponent<Renderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    private void FindMeshFilter()
    {
        if (targetGameObject == null)
        {
            Debug.LogWarning("Target GameObject is not assigned.");
            return;
        }

        meshFilter = targetGameObject.GetComponent<MeshFilter>();

        if (meshFilter == null)
        {
            meshFilter = targetGameObject.GetComponentInChildren<MeshFilter>();
            if (meshFilter == null)
            {
                Debug.LogError("No MeshFilter found on or in children of the target GameObject.");
            }
        }
    }

    private void OnValidate()
    {
        // Update mesh filter and renderer when targetGameObject changes
        if (targetGameObject != null)
        {
            FindMeshFilter();
            if (meshFilter != null)
            {
                meshRenderer = meshFilter.GetComponent<Renderer>();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (meshFilter == null || meshFilter.sharedMesh == null)
            return;

        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector3[] normals = mesh.normals;

        // Temporarily disable the renderer
        bool wasRendererEnabled = false;
        if (meshRenderer != null)
        {
            wasRendererEnabled = meshRenderer.enabled;
            if (showTriangles || showNormals || showVertexNumbers)
            {
                meshRenderer.enabled = false;
            }
        }

        if (showTriangles)
        {
            int maxTriangles = Mathf.Min(maxTrianglesToShow, triangles.Length / 3);
            for (int i = 0; i < maxTriangles; i++)
            {
                int index = i * 3;
                int vertexIndex0 = triangles[index];
                int vertexIndex1 = triangles[index + 1];
                int vertexIndex2 = triangles[index + 2];
                
                // Ensure we don't exceed vertex array bounds
                if (vertexIndex0 >= vertices.Length || vertexIndex1 >= vertices.Length || vertexIndex2 >= vertices.Length)
                {
                    Debug.LogWarning($"Triangle {i} has invalid vertex indices. Skipping.");
                    continue;
                }

                Vector3 v0 = meshFilter.transform.TransformPoint(vertices[vertexIndex0]);
                Vector3 v1 = meshFilter.transform.TransformPoint(vertices[vertexIndex1]);
                Vector3 v2 = meshFilter.transform.TransformPoint(vertices[vertexIndex2]);
                
                Gizmos.color = Color.HSVToRGB(i / (float)maxTriangles, 1, 1);
                
                Gizmos.DrawLine(v0, v1);
                Gizmos.DrawLine(v1, v2);
                Gizmos.DrawLine(v2, v0);
                    
                // Debug: Print triangle info to console
                Debug.Log($"Triangle {i}: Vertex indices {vertexIndex0}, {vertexIndex1}, {vertexIndex2}");
            }
        }

        if (showNormals)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(meshFilter.transform.TransformPoint(vertices[i]), meshFilter.transform.TransformPoint(vertices[i] + normals[i]));
            }
        }

        // Draw vertex numbers if enabled
        if (showVertexNumbers)
        {
            int maxVertexToShow = Mathf.Min(maxVertexNumberToShow, vertices.Length);
            for (int i = 0; i < maxVertexToShow; i++)
            {
                Vector3 vertexPosition = meshFilter.transform.TransformPoint(vertices[i]);
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(vertexPosition, 0.02f);
                
                // Display vertex number
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.white;
                style.fontSize = 8;
                style.fontStyle = FontStyle.Bold;
                Handles.Label(vertexPosition, i.ToString(), style);
            }
        }

        // Re-enable the renderer if it was enabled before
        if (meshRenderer != null)
        {
            meshRenderer.enabled = wasRendererEnabled;
        }
    }
}