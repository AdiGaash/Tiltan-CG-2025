using System.Collections.Generic;
using UnityEngine;

public class EdgeDetector
{
    public static Dictionary<(int,int), int> GetEdges(Mesh mesh)
    {
        int[] tris = mesh.triangles;
        Dictionary<(int,int), int> edgeCount = new Dictionary<(int,int), int>();

        // helper function to register an edge
        void AddEdge(int a, int b)
        {
            // always store with the lower index first
            var edge = (Mathf.Min(a, b), Mathf.Max(a, b));

            if (!edgeCount.ContainsKey(edge))
                edgeCount[edge] = 1;
            else
                edgeCount[edge]++;
        }

        // iterate over triangles
        for (int i = 0; i < tris.Length; i += 3)
        {
            int i0 = tris[i];
            int i1 = tris[i+1];
            int i2 = tris[i+2];

            AddEdge(i0, i1);
            AddEdge(i1, i2);
            AddEdge(i2, i0);
        }

        return edgeCount; // internal and boundary edges
    }
}