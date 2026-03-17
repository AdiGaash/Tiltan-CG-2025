using UnityEngine;

public class MarchingSquares : MonoBehaviour
{
    public int width = 20;
    public int height = 20;
    public float threshold = 0.5f;
    public bool ShowPoints = true;
    public bool ShowSquares = true;

    float[,] values;

    void Start()
    {
        GenerateField();
    }

    void GenerateField()
    {
        values = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Generate scalar field using Perlin Noise
                values[x, y] = Mathf.PerlinNoise(x * 0.1f, y * 0.1f);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (values == null) return;
        if(ShowPoints)
            DrawPoints();
        if(ShowSquares)
            DrawSquares();
    }

    // 🟢 Draw scalar field points
    void DrawPoints()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // White = inside, Black = outside
                Gizmos.color = values[x, y] > threshold ? Color.white : Color.black;

                // Draw small sphere at each grid point
                Gizmos.DrawSphere(new Vector3(x, y, 0), 0.1f);
            }
        }
    }

    // 🟦 Draw grid cells as wireframe squares
    void DrawSquares()
    {
        Gizmos.color = Color.gray;

        // Note: width - 1 and height - 1 because each square uses 4 points
        for (int x = 0; x < width - 1; x++)
        {
            for (int y = 0; y < height - 1; y++)
            {
                // Bottom-left corner of the square
                Vector3 bl = new Vector3(x, y, 0);
                Vector3 br = new Vector3(x + 1, y, 0);
                Vector3 tr = new Vector3(x + 1, y + 1, 0);
                Vector3 tl = new Vector3(x, y + 1, 0);

                // Draw square using lines
                Gizmos.DrawLine(bl, br);
                Gizmos.DrawLine(br, tr);
                Gizmos.DrawLine(tr, tl);
                Gizmos.DrawLine(tl, bl);
            }
        }
    }
}