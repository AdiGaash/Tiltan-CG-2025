using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class UVOutlineExporter : MonoBehaviour
{
    public MeshFilter meshFilter;       // Assign your MeshFilter manually
    public int textureSize = 1024;      // Output resolution
    public string savePath = "Assets/UV_Outline.png";

    void Start()
    {
        Mesh mesh = meshFilter.sharedMesh;
        CreateUVOutlineTexture(mesh);
    }

    void CreateUVOutlineTexture(Mesh mesh)
    {
        // Create white texture
        Texture2D tex = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        Color[] blank = new Color[textureSize * textureSize];
        for (int i = 0; i < blank.Length; i++) blank[i] = Color.white;
        tex.SetPixels(blank);

        Vector2[] uv = mesh.uv;
        int[] triangles = mesh.triangles;

        // A dictionary for counting shared edges
        Dictionary<Edge, int> edgeUseCount = new Dictionary<Edge, int>();

        // Step 1: Collect all edges and count how many triangles use each one
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int a = triangles[i];
            int b = triangles[i + 1];
            int c = triangles[i + 2];

            AddEdge(edgeUseCount, new Edge(a, b));
            AddEdge(edgeUseCount, new Edge(b, c));
            AddEdge(edgeUseCount, new Edge(c, a));
        }

        // Step 2: Draw only edges that are used by exactly 1 triangle (UV island borders)
        foreach (var kvp in edgeUseCount)
        {
            if (kvp.Value == 1) // Boundary edge
            {
                Edge e = kvp.Key;

                Vector2 uvA = uv[e.a];
                Vector2 uvB = uv[e.b];

                DrawUVLine(tex, uvA, uvB, Color.black);
            }
        }

        // Save PNG
        tex.Apply();
        File.WriteAllBytes(savePath, tex.EncodeToPNG());
        Debug.Log("UV outline texture saved to: " + savePath);
    }

    // Count edges
    void AddEdge(Dictionary<Edge, int> dict, Edge e)
    {
        if (dict.ContainsKey(e))
            dict[e]++;
        else
            dict[e] = 1;
    }

    // Draw a line from UV(A) to UV(B)
    void DrawUVLine(Texture2D tex, Vector2 uvA, Vector2 uvB, Color col)
    {
        int x0 = (int)(uvA.x * textureSize);
        int y0 = (int)(uvA.y * textureSize);
        int x1 = (int)(uvB.x * textureSize);
        int y1 = (int)(uvB.y * textureSize);

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        int err = dx - dy;

        while (true)
        {
            tex.SetPixel(x0, y0, col);

            if (x0 == x1 && y0 == y1) break;

            int e2 = err * 2;

            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    // Helper struct ensures edges are undirected
    public struct Edge
    {
        public int a;
        public int b;

        public Edge(int a, int b)
        {
            // Sort so edge AB = BA
            if (a < b) { this.a = a; this.b = b; }
            else { this.a = b; this.b = a; }
        }

        public override int GetHashCode()
        {
            return a * 73856093 ^ b * 19349663;
        }

        public override bool Equals(object obj)
        {
            Edge other = (Edge)obj;
            return this.a == other.a && this.b == other.b;
        }
    }
}
