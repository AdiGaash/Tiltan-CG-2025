using UnityEngine;
using System.IO;

public class UVFillTextureGenerator : MonoBehaviour
{
    public MeshFilter meshFilter;      // Assign your mesh here
    public int textureSize = 1024;     // Output resolution
    public string savePath = "Assets/UV_Filled.png";

    void Start()
    {
        Mesh mesh = meshFilter.sharedMesh;
        CreateFilledUVTexture(mesh);
    }

    void CreateFilledUVTexture(Mesh mesh)
    {
        Texture2D tex = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);

        // Fill background white
        Color[] blank = new Color[textureSize * textureSize];
        for (int i = 0; i < blank.Length; i++) blank[i] = Color.white;
        tex.SetPixels(blank);

        Vector2[] uv = mesh.uv;
        int[] triangles = mesh.triangles;

        // Process every triangle in the mesh
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector2 uv0 = uv[triangles[i]];
            Vector2 uv1 = uv[triangles[i + 1]];
            Vector2 uv2 = uv[triangles[i + 2]];

            FillTriangle(tex, uv0, uv1, uv2, Color.black);
        }

        // Apply and save
        tex.Apply();
        File.WriteAllBytes(savePath, tex.EncodeToPNG());
        Debug.Log("Filled UV texture saved to: " + savePath);
    }

    // Converts UVs to pixel coordinates
    Vector2Int UVToPixel(Vector2 uv)
    {
        int x = Mathf.RoundToInt(uv.x * (textureSize - 1));
        int y = Mathf.RoundToInt(uv.y * (textureSize - 1));
        return new Vector2Int(x, y);
    }

    // Barycentric triangle fill
    void FillTriangle(Texture2D tex, Vector2 uv0, Vector2 uv1, Vector2 uv2, Color color)
    {
        // Convert UVs to pixel positions
        Vector2Int p0 = UVToPixel(uv0);
        Vector2Int p1 = UVToPixel(uv1);
        Vector2Int p2 = UVToPixel(uv2);

        // Compute bounding box to limit pixel checks
        int minX = Mathf.Min(p0.x, Mathf.Min(p1.x, p2.x));
        int maxX = Mathf.Max(p0.x, Mathf.Max(p1.x, p2.x));
        int minY = Mathf.Min(p0.y, Mathf.Min(p1.y, p2.y));
        int maxY = Mathf.Max(p0.y, Mathf.Max(p1.y, p2.y));

        // Clamp to texture boundaries
        minX = Mathf.Clamp(minX, 0, textureSize - 1);
        maxX = Mathf.Clamp(maxX, 0, textureSize - 1);
        minY = Mathf.Clamp(minY, 0, textureSize - 1);
        maxY = Mathf.Clamp(maxY, 0, textureSize - 1);

        // Precompute triangle area
        float area = EdgeFunction(p0, p1, p2);

        // Skip degenerate triangles
        if (Mathf.Abs(area) < 0.0001f) return;

        // Test each pixel in bounding box
        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                Vector2Int p = new Vector2Int(x, y);

                // Barycentric coordinates
                float w0 = EdgeFunction(p1, p2, p);
                float w1 = EdgeFunction(p2, p0, p);
                float w2 = EdgeFunction(p0, p1, p);

                // Check if pixel is inside triangle (all barycentrics have same sign)
                if ((w0 >= 0 && w1 >= 0 && w2 >= 0) ||
                    (w0 <= 0 && w1 <= 0 && w2 <= 0))
                {
                    tex.SetPixel(x, y, color);
                }
            }
        }
    }

    // Edge function for barycentric test
    float EdgeFunction(Vector2Int a, Vector2Int b, Vector2Int c)
    {
        return (c.x - a.x) * (b.y - a.y) -
               (c.y - a.y) * (b.x - a.x);
    }
}
