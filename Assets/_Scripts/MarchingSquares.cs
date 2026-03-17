using UnityEngine;

public class MarchingSquares : MonoBehaviour
{
    public int width = 20;
    public int height = 20;
    public float threshold = 0.5f;
    public bool ShowPoints = true;
    public bool ShowSquares = true;
    public bool DrawMarchingSquares = true;

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
        if (DrawMarchingSquares)
        {
            DoMarchingSquares();
            return;
        }

        if(ShowPoints)
            DrawPoints();
        if(ShowSquares)
            DrawSquares();
        
        for (int x = 0; x < width - 1; x++)
        {
            for (int y = 0; y < height - 1; y++)
            {
                int caseIndex = GetCase(x, y);

                Vector3 center = new Vector3(x + 0.5f, y + 0.5f, 0);

                UnityEditor.Handles.Label(center, caseIndex.ToString());
            }
        }
    }

    void DoMarchingSquares()
    {
        for (int x = 0; x < width - 1; x++)
        {
            for (int y = 0; y < height - 1; y++)
            {
                int caseIndex = GetCase(x, y);
                DrawCase(x, y, caseIndex);
            }
        }
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
    
    
    int GetCase(int x, int y)
    {
        int caseIndex = 0;

        // Each corner contributes a bit to the case index
        // We use bitwise OR (|=) to "turn on" bits

        // Bottom-left corner (bit 0 → value 1)
        if (values[x, y] > threshold)
            caseIndex |= 1;

        // Bottom-right corner (bit 1 → value 2)
        if (values[x + 1, y] > threshold)
            caseIndex |= 2;

        // Top-right corner (bit 2 → value 4)
        if (values[x + 1, y + 1] > threshold)
            caseIndex |= 4;

        // Top-left corner (bit 3 → value 8)
        if (values[x, y + 1] > threshold)
            caseIndex |= 8;

        return caseIndex;
    }
    
    
    void DrawCase(int x, int y, int caseIndex)
    {
        // Corner positions
        Vector3 bl = new Vector3(x, y, 0);         // bottom-left
        Vector3 br = new Vector3(x + 1, y, 0);     // bottom-right
        Vector3 tr = new Vector3(x + 1, y + 1, 0); // top-right
        Vector3 tl = new Vector3(x, y + 1, 0);     // top-left

        // Edge midpoints (simple version - no interpolation yet)
        Vector3 midLeft   = (bl + tl) * 0.5f;
        Vector3 midRight  = (br + tr) * 0.5f;
        Vector3 midTop    = (tl + tr) * 0.5f;
        Vector3 midBottom = (bl + br) * 0.5f;

        Gizmos.color = Color.green;

        switch (caseIndex)
        {
            case 0:
            case 15:
                // No lines
                break;

            case 1:
                Gizmos.DrawLine(midLeft, midBottom);
                break;

            case 2:
                Gizmos.DrawLine(midBottom, midRight);
                break;

            case 3:
                Gizmos.DrawLine(midLeft, midRight);
                break;

            case 4:
                Gizmos.DrawLine(midRight, midTop);
                break;

            case 5:
                Gizmos.DrawLine(midLeft, midTop);
                Gizmos.DrawLine(midBottom, midRight);
                break;

            case 6:
                Gizmos.DrawLine(midBottom, midTop);
                break;

            case 7:
                Gizmos.DrawLine(midLeft, midTop);
                break;

            case 8:
                Gizmos.DrawLine(midTop, midLeft);
                break;

            case 9:
                Gizmos.DrawLine(midBottom, midTop);
                break;

            case 10:
                Gizmos.DrawLine(midLeft, midBottom);
                Gizmos.DrawLine(midRight, midTop);
                break;

            case 11:
                Gizmos.DrawLine(midRight, midTop);
                break;

            case 12:
                Gizmos.DrawLine(midLeft, midRight);
                break;

            case 13:
                Gizmos.DrawLine(midBottom, midRight);
                break;

            case 14:
                Gizmos.DrawLine(midLeft, midBottom);
                break;
        }
    }
}