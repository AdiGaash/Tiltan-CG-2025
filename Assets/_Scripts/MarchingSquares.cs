using UnityEngine;

public class MarchingSquares : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 20;
    public int height = 20;

    [Header("Marching Settings")]
    [Range(0f, 1f)]
    public float threshold = 0.5f;

    [Header("Debug View")]
    public bool showPoints = true;
    public bool showSquares = true;
    public bool drawMarchingSquares = true;

    float[,] values;

    void Start()
    {
        GenerateField();
    }

    void Update()
    {
        // Optional: animate threshold (great for teaching)
        // threshold = Mathf.PingPong(Time.time, 1f);
    }

    void GenerateField()
    {
        values = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                values[x, y] = Mathf.PerlinNoise(x * 0.1f, y * 0.1f);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (values == null) return;

        if (showPoints)
            DrawPoints();

        if (showSquares)
            DrawSquares();

        if (drawMarchingSquares)
            DoMarchingSquares();
    }

    // ---------------------------
    // 🟢 Draw scalar field points
    // ---------------------------
    void DrawPoints()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Grayscale visualization (better than black/white)
                float v = values[x, y];
                Gizmos.color = new Color(v, v, v);

                Gizmos.DrawSphere(new Vector3(x, y, 0), 0.1f);
            }
        }
    }

    // ---------------------------
    // 🟦 Draw grid squares
    // ---------------------------
    void DrawSquares()
    {
        Gizmos.color = Color.gray;

        for (int x = 0; x < width - 1; x++)
        {
            for (int y = 0; y < height - 1; y++)
            {
                Vector3 bl = new Vector3(x, y, 0);
                Vector3 br = new Vector3(x + 1, y, 0);
                Vector3 tr = new Vector3(x + 1, y + 1, 0);
                Vector3 tl = new Vector3(x, y + 1, 0);

                Gizmos.DrawLine(bl, br);
                Gizmos.DrawLine(br, tr);
                Gizmos.DrawLine(tr, tl);
                Gizmos.DrawLine(tl, bl);
            }
        }
    }

    // ---------------------------
    // 🧠 Marching Squares Logic
    // ---------------------------
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

    // ---------------------------
    // 🔢 Compute case index (0–15)
    // ---------------------------
    int GetCase(int x, int y)
    {
        int caseIndex = 0;

        if (values[x, y] > threshold) caseIndex |= 1;           // BL
        if (values[x + 1, y] > threshold) caseIndex |= 2;       // BR
        if (values[x + 1, y + 1] > threshold) caseIndex |= 4;   // TR
        if (values[x, y + 1] > threshold) caseIndex |= 8;       // TL

        return caseIndex;
    }

    // ---------------------------
    // 🔵 Interpolation function
    // ---------------------------
    Vector3 Interpolate(Vector3 p1, Vector3 p2, float v1, float v2)
    {
        if (Mathf.Abs(v1 - v2) < 0.0001f)
            return (p1 + p2) * 0.5f;

        float t = (threshold - v1) / (v2 - v1);
        t = Mathf.Clamp01(t);

        return Vector3.Lerp(p1, p2, t);
    }

    // ---------------------------
    // ✏️ Draw contour lines
    // ---------------------------
    void DrawCase(int x, int y, int caseIndex)
    {
        Vector3 bl = new Vector3(x, y, 0);
        Vector3 br = new Vector3(x + 1, y, 0);
        Vector3 tr = new Vector3(x + 1, y + 1, 0);
        Vector3 tl = new Vector3(x, y + 1, 0);

        // 🔥 Interpolated edge points
        Vector3 midLeft   = Interpolate(bl, tl, values[x, y], values[x, y + 1]);
        Vector3 midRight  = Interpolate(br, tr, values[x + 1, y], values[x + 1, y + 1]);
        Vector3 midTop    = Interpolate(tl, tr, values[x, y + 1], values[x + 1, y + 1]);
        Vector3 midBottom = Interpolate(bl, br, values[x, y], values[x + 1, y]);

        Gizmos.color = Color.green;

        switch (caseIndex)
        {
            case 0:
            case 15:
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