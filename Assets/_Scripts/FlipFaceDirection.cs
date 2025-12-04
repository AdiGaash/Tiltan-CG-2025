using UnityEngine;

public class FlipFaceDirection : MonoBehaviour
{
    void Start()
    {
        // Get the mesh from this object
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        // Get triangle indices (each 3 numbers make one triangle)
        int[] tris = mesh.triangles;

        // Loop through triangles and flip the order
        for (int i = 0; i < tris.Length; i += 3)
        {
            // Swap the last two indices
            // This reverses the winding order
            int temp = tris[i + 1];
            tris[i + 1] = tris[i + 2];
            tris[i + 2] = temp;
        }

        // Apply the modified triangle order
        mesh.triangles = tris;

        // Recalculate normals so lighting matches the new direction
        mesh.RecalculateNormals();
    }
}