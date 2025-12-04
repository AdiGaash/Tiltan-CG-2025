using UnityEngine;

public class TriangleFacingDetector : MonoBehaviour
{
    public Camera targetCamera;     // Camera used to determine facing
    private Mesh mesh;

    void Start()
    {
        // If no camera was assigned, just use the main camera
        if (targetCamera == null)
            targetCamera = Camera.main;

        // Get the mesh from the MeshFilter (vertices & triangles are in LOCAL SPACE)
        mesh = GetComponent<MeshFilter>().mesh;

        // Run detection once
        DetectTriangleFacing();
    }

    void DetectTriangleFacing()
    {
        // Get vertex and triangle data
        Vector3[] vertices = mesh.vertices;   // LOCAL SPACE vertices
        int[] triangles = mesh.triangles;     // Indices of triangles (winding order)

        // Loop through all triangles (each triangle = 3 indices)
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Get 3 vertex positions in LOCAL space
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i + 1]];
            Vector3 v2 = vertices[triangles[i + 2]];

            // Convert them to WORLD space (so we can compare with camera)
            v0 = transform.TransformPoint(v0);
            v1 = transform.TransformPoint(v1);
            v2 = transform.TransformPoint(v2);

            // Compute the face normal using cross product
            Vector3 edge1 = v1 - v0;
            Vector3 edge2 = v2 - v0;
            Vector3 faceNormal = Vector3.Cross(edge1, edge2).normalized;

            // Direction from the camera to the triangle
            Vector3 cameraToTriangle = (v0 - targetCamera.transform.position).normalized;

            // Dot product tells if triangle is front-facing or back-facing
            // If dot < 0 → triangle is FRONT-FACING toward the camera
            // If dot > 0 → triangle is BACK-FACING away from the camera
            float dot = Vector3.Dot(faceNormal, cameraToTriangle);

            bool frontFacing = dot < 0f;

            // Show result in Console
            Debug.Log(
                "Triangle " + (i / 3) +
                " | Normal: " + faceNormal +
                " | Facing Camera? " + frontFacing
            );
        }
    }
}
