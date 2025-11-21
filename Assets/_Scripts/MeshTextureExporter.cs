using UnityEngine;
using System.IO;

public class MeshTextureExporter : MonoBehaviour
{
    public MeshFilter meshFilter;  // Assign your mesh here
    public int textureSize = 512;  // Texture resolution
    public string savePath = "Assets/";

    void Start()
    {
        Mesh mesh = meshFilter.sharedMesh;
        CreateHardEdgeTexture(mesh);
        CreateUVTexture(mesh);
    }

    void CreateHardEdgeTexture(Mesh mesh)
    {
        Texture2D texture = new Texture2D(textureSize, textureSize);
        
        // Fill background white
        Color[] pixels = new Color[textureSize * textureSize];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.white;
        texture.SetPixels(pixels);

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Draw edges
        for (int i = 0; i < triangles.Length; i += 3)
        {
            DrawLine(texture, vertices[triangles[i]], vertices[triangles[i + 1]]);
            DrawLine(texture, vertices[triangles[i + 1]], vertices[triangles[i + 2]]);
            DrawLine(texture, vertices[triangles[i + 2]], vertices[triangles[i]]);
        }

        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(savePath, "HardEdgeTexture.png"), bytes);
        Debug.Log("Hard Edge Texture Saved!");
    }

    void CreateUVTexture(Mesh mesh)
    {
        Texture2D texture = new Texture2D(textureSize, textureSize);

        // Fill background white
        Color[] pixels = new Color[textureSize * textureSize];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Color.white;
        texture.SetPixels(pixels);

        Vector2[] uvs = mesh.uv;
        int[] triangles = mesh.triangles;

        // Color triangles based on UV mapping (example: black)
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector2 uv0 = uvs[triangles[i]];
            Vector2 uv1 = uvs[triangles[i + 1]];
            Vector2 uv2 = uvs[triangles[i + 2]];
            DrawTriangle(texture, uv0, uv1, uv2, Color.black);
        }

        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(savePath, "UVTexture.png"), bytes);
        Debug.Log("UV Texture Saved!");
    }

    // Helper: Draw a line on the texture between two vertices
    void DrawLine(Texture2D tex, Vector3 v1, Vector3 v2)
    {
        int x0 = (int)((v1.x + 0.5f) * textureSize);
        int y0 = (int)((v1.y + 0.5f) * textureSize);
        int x1 = (int)((v2.x + 0.5f) * textureSize);
        int y1 = (int)((v2.y + 0.5f) * textureSize);

        int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx + dy, e2;

        while (true)
        {
            if (x0 >= 0 && x0 < textureSize && y0 >= 0 && y0 < textureSize)
                tex.SetPixel(x0, y0, Color.black);

            if (x0 == x1 && y0 == y1) break;
            e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; }
            if (e2 <= dx) { err += dx; y0 += sy; }
        }
    }

    // Helper: Draw a simple triangle based on UVs (approximation)
    void DrawTriangle(Texture2D tex, Vector2 uv0, Vector2 uv1, Vector2 uv2, Color col)
    {
        int x0 = (int)(uv0.x * textureSize);
        int y0 = (int)(uv0.y * textureSize);
        int x1 = (int)(uv1.x * textureSize);
        int y1 = (int)(uv1.y * textureSize);
        int x2 = (int)(uv2.x * textureSize);
        int y2 = (int)(uv2.y * textureSize);

        // Draw edges for simplicity
        DrawLine(tex, new Vector3(x0, y0, 0), new Vector3(x1, y1, 0));
        DrawLine(tex, new Vector3(x1, y1, 0), new Vector3(x2, y2, 0));
        DrawLine(tex, new Vector3(x2, y2, 0), new Vector3(x0, y0, 0));
    }
}
