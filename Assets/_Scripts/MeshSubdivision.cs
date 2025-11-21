using UnityEngine;
using System.Collections.Generic;

public class MeshSubdivision : MonoBehaviour
{
    public MeshFilter meshFilter;
    public int subdivisions = 1;

    void Start()
    {
        Mesh mesh = CreateInitialMesh(); // Start with a simple triangle or quad
        for (int i = 0; i < subdivisions; i++)
        {
            mesh = Subdivide(mesh);
        }
        meshFilter.mesh = mesh;
    }

    Mesh CreateInitialMesh()
    {
        Mesh mesh = new Mesh();

        // Simple triangle
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0,0,0),
            new Vector3(1,0,0),
            new Vector3(0.5f,1,0)
        };

        int[] triangles = new int[] { 0, 1, 2 };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }

    Mesh Subdivide(Mesh mesh)
    {
        Vector3[] oldVertices = mesh.vertices;
        int[] oldTriangles = mesh.triangles;

        List<Vector3> newVertices = new List<Vector3>(oldVertices);
        List<int> newTriangles = new List<int>();
        Dictionary<string, int> midpointCache = new Dictionary<string, int>();

        for (int i = 0; i < oldTriangles.Length; i += 3)
        {
            int v0 = oldTriangles[i];
            int v1 = oldTriangles[i + 1];
            int v2 = oldTriangles[i + 2];

            // Get midpoints (add if not exists)
            int m0 = GetMidpoint(v0, v1, oldVertices, newVertices, midpointCache);
            int m1 = GetMidpoint(v1, v2, oldVertices, newVertices, midpointCache);
            int m2 = GetMidpoint(v2, v0, oldVertices, newVertices, midpointCache);

            // Create 4 new triangles
            newTriangles.AddRange(new int[] { v0, m0, m2 });
            newTriangles.AddRange(new int[] { m0, v1, m1 });
            newTriangles.AddRange(new int[] { m2, m1, v2 });
            newTriangles.AddRange(new int[] { m0, m1, m2 });
        }

        Mesh newMesh = new Mesh();
        newMesh.vertices = newVertices.ToArray();
        newMesh.triangles = newTriangles.ToArray();
        newMesh.RecalculateNormals();
        return newMesh;
    }

    int GetMidpoint(int a, int b, Vector3[] oldVertices, List<Vector3> newVertices, Dictionary<string,int> cache)
    {
        string key = a < b ? a + "_" + b : b + "_" + a;
        if (cache.ContainsKey(key))
            return cache[key];

        Vector3 midpoint = (oldVertices[a] + oldVertices[b]) / 2f;
        newVertices.Add(midpoint);
        int index = newVertices.Count - 1;
        cache[key] = index;
        return index;
    }
}
