using UnityEngine;

public class MeshInspector : MonoBehaviour
{
    public GameObject targetGameObject;
    public bool showTriangles = true;
    public bool showNormals = true;
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
            if (showTriangles || showNormals)
            {
                meshRenderer.enabled = false;
            }
        }

        if (showTriangles)
        {
            for (int i = 0; i < triangles.Length; i += 3)
            {
                Gizmos.color = Color.HSVToRGB(i / (float)triangles.Length, 1, 1);
                Gizmos.DrawLine(meshFilter.transform.TransformPoint(vertices[triangles[i]]), meshFilter.transform.TransformPoint(vertices[triangles[i + 1]]));
                Gizmos.DrawLine(meshFilter.transform.TransformPoint(vertices[triangles[i + 1]]), meshFilter.transform.TransformPoint(vertices[triangles[i + 2]]));
                Gizmos.DrawLine(meshFilter.transform.TransformPoint(vertices[triangles[i + 2]]), meshFilter.transform.TransformPoint(vertices[triangles[i]]));
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

        // Re-enable the renderer if it was enabled before
        if (meshRenderer != null)
        {
            meshRenderer.enabled = wasRendererEnabled;
        }
    }
}