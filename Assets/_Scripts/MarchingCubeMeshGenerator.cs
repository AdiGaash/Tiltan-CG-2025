
using System.Collections.Generic;
using UnityEngine;

public class MarchingCubeMeshGenerator : MonoBehaviour
{
    [Header("Mesh Generation")]
    public int gridSize = 32;
    public float voxelSize = 1.0f;
    public float surfaceLevel = 0.0f;
    
    [Header("Noise Settings")]
    public float noiseScale = 0.1f;
    public Vector3 noiseOffset = Vector3.zero;
    public Shader shader;
    
    private MarchingCubes marchingCubes;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    
    void Start()
    {
        // Initialize components
        marchingCubes = new MarchingCubes(surfaceLevel);
        
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();
            
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(shader);
        }
        
        GenerateMesh();
    }
    
    [ContextMenu("Regenerate Mesh")]
    public void GenerateMesh()
    {
        // Create voxel data
        float[,,] voxelData = GenerateVoxelData();
        
        // Generate mesh using marching cubes
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        
        marchingCubes.Generate(voxelData, vertices, triangles);
        
        // Create Unity mesh
        Mesh mesh = new Mesh();
        mesh.name = "Marching Cubes Mesh";
        
        // Scale vertices by voxel size
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] *= voxelSize;
        }
        
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        meshFilter.mesh = mesh;
    }
    
    private float[,,] GenerateVoxelData()
    {
        float[,,] data = new float[gridSize, gridSize, gridSize];
        
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    Vector3 worldPos = new Vector3(x, y, z) + noiseOffset;
                    
                    // Generate density using 3D Perlin noise
                    float density = GenerateDensity(worldPos);
                    data[x, y, z] = density;
                }
            }
        }
        
        return data;
    }
    
    private float GenerateDensity(Vector3 position)
    {
        // Example: Sphere with noise
        Vector3 center = Vector3.one * (gridSize * 0.5f);
        float distanceFromCenter = Vector3.Distance(position, center);
        float sphereRadius = gridSize * 0.3f;
        
        // Basic sphere
        float sphereDensity = sphereRadius - distanceFromCenter;
        
        // Add 3D noise for organic variation
        float noise = Mathf.PerlinNoise(position.x * noiseScale, position.y * noiseScale) * 
                      Mathf.PerlinNoise(position.z * noiseScale, position.x * noiseScale);
        
        return sphereDensity + (noise * 5.0f);
    }
}