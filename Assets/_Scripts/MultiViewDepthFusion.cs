using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MultiViewDepthFusion : MonoBehaviour
{
    [Header("Base Mesh Settings")]
    public int resolutionX = 50; // vertices along X
    public int resolutionY = 50; // vertices along Y
    public int resolutionZ = 50; // vertices along Z
    public Vector3 volumeSize = new Vector3(1f, 1f, 1f); // size of the bounding volume

    [Header("Depth Cameras")]
    public List<DepthCamera> depthCameras = new List<DepthCamera>(); // any number of cameras

    [Header("Vertex Displacement")]
    public float displacementScale = 1f; // scaling factor for vertex displacement

    private MeshFilter meshFilter;
    private Mesh mesh;
    private Vector3[] vertices;

    public bool GizmoON = false;

   

    // Step 1: Generate base grid mesh inside volume
    
    public void GenerateBaseMesh()
    {
        mesh = new Mesh();
        mesh.name = "MultiViewGridMesh";

        int totalVerts = resolutionX * resolutionY * resolutionZ;
        vertices = new Vector3[totalVerts];

        float stepX = volumeSize.x / (resolutionX - 1);
        float stepY = volumeSize.y / (resolutionY - 1);
        float stepZ = volumeSize.z / (resolutionZ - 1);

        int index = 0;
        for(int x = 0; x < resolutionX; x++)
        {
            for(int y = 0; y < resolutionY; y++)
            {
                for(int z = 0; z < resolutionZ; z++)
                {
                    vertices[index++] = new Vector3(
                        x * stepX - volumeSize.x / 2f,
                        y * stepY - volumeSize.y / 2f,
                        z * stepZ - volumeSize.z / 2f
                    );
                }
            }
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    // Step 2: Fuse depth maps and generate voxel mesh
   [ContextMenu("Fuse Depth Cameras to Voxels")]
    void FuseDepthCameras()
    {
        if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();

        // 1. Clear previous mesh using sharedMesh (safe in edit mode)
        if (meshFilter.sharedMesh == null)
            meshFilter.sharedMesh = new Mesh();
        else
            meshFilter.sharedMesh.Clear();

        mesh = meshFilter.sharedMesh;

        // 2. Prepare new lists for vertices and triangles
        List<Vector3> meshVertices = new List<Vector3>();
        List<int> meshTriangles = new List<int>();
        int vertexOffset = 0;

        Vector3 volumeOrigin = transform.position - volumeSize / 2f;
        float stepX = volumeSize.x / (resolutionX - 1);
        float stepY = volumeSize.y / (resolutionY - 1);
        float stepZ = volumeSize.z / (resolutionZ - 1);

        // 3. Loop through all voxels
        for (int x = 0; x < resolutionX; x++)
        {
            for (int y = 0; y < resolutionY; y++)
            {
                for (int z = 0; z < resolutionZ; z++)
                {
                    Vector3 voxelCenter = volumeOrigin + new Vector3(x * stepX, y * stepY, z * stepZ);

                    // Check coverage by any depth camera
                    bool covered = false;
                    foreach (var cam in depthCameras)
                    {
                        Quaternion camRot = Quaternion.Euler(cam.rotationEuler);
                        Vector3 localPos = Quaternion.Inverse(camRot) * (voxelCenter - cam.position);

                        float u = (localPos.x / volumeSize.x) + 0.5f;
                        float v = (localPos.y / volumeSize.y) + 0.5f;
                        float zLocal = localPos.z;

                        if (u >= 0f && u <= 1f &&
                            v >= 0f && v <= 1f &&
                            zLocal >= cam.nearPlane && zLocal <= cam.farPlane)
                        {
                            covered = true;
                            break;
                        }
                    }

                    if (!covered) continue; // skip voxel if not hit

                    // 4. Add cube geometry for this voxel
                    Vector3[] cubeVerts = GetCubeVertices(voxelCenter, stepX, stepY, stepZ);
                    meshVertices.AddRange(cubeVerts);

                    int[] cubeTris = GetCubeTriangles(vertexOffset);
                    meshTriangles.AddRange(cubeTris);

                    vertexOffset += cubeVerts.Length;
                }
            }
        }

        // 5. Assign mesh data
        mesh.Clear();
        mesh.vertices = meshVertices.ToArray();
        mesh.triangles = meshTriangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        meshFilter.sharedMesh = mesh; // ensure sharedMesh is used

    #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(meshFilter);
        UnityEditor.SceneView.RepaintAll();
    #endif
    }
    
    
    // Returns 8 corners of a cube centered at 'center' with dimensions dx,dy,dz
    Vector3[] GetCubeVertices(Vector3 center, float dx, float dy, float dz)
    {
        float hx = dx * 0.5f;
        float hy = dy * 0.5f;
        float hz = dz * 0.5f;

        return new Vector3[]
        {
            center + new Vector3(-hx,-hy,-hz),
            center + new Vector3(hx,-hy,-hz),
            center + new Vector3(hx,hy,-hz),
            center + new Vector3(-hx,hy,-hz),
            center + new Vector3(-hx,-hy,hz),
            center + new Vector3(hx,-hy,hz),
            center + new Vector3(hx,hy,hz),
            center + new Vector3(-hx,hy,hz)
        };
    }

// Returns 36 indices for a cube using the given vertex offset
    int[] GetCubeTriangles(int offset)
    {
        return new int[]
        {
            offset+0,offset+2,offset+1, offset+0,offset+3,offset+2, // bottom
            offset+4,offset+5,offset+6, offset+4,offset+6,offset+7, // top
            offset+0,offset+1,offset+5, offset+0,offset+5,offset+4, // front
            offset+1,offset+2,offset+6, offset+1,offset+6,offset+5, // right
            offset+2,offset+3,offset+7, offset+2,offset+7,offset+6, // back
            offset+3,offset+0,offset+4, offset+3,offset+4,offset+7  // left
        };
    }
    
    void OnDrawGizmos()
    { 
        
    if (!GizmoON) return;
        
       

    // Draw bounding volume
    Gizmos.color = Color.white;
    Gizmos.DrawWireCube(transform.position, volumeSize);

    
    // Draw vertices based on mesh resolution
    // Draw vertices based on mesh resolution with coverage coloring
    float stepX = volumeSize.x / (resolutionX - 1);
    float stepY = volumeSize.y / (resolutionY - 1);
    float stepZ = volumeSize.z / (resolutionZ - 1);

    Vector3 volumeOrigin = transform.position - volumeSize / 2f;

    for (int x = 0; x < resolutionX; x++)
    {
        for (int y = 0; y < resolutionY; y++)
        {
            for (int z = 0; z < resolutionZ; z++)
            {
                Vector3 vertexPos = volumeOrigin + new Vector3(x * stepX, y * stepY, z * stepZ);

                bool covered = false;
                foreach (var cam in depthCameras)
                {
                    Quaternion camRot = Quaternion.Euler(cam.rotationEuler);
                    Vector3 localPos = Quaternion.Inverse(camRot) * (vertexPos - cam.position);

                    float u = (localPos.x / volumeSize.x) + 0.5f;
                    float vUV = (localPos.y / volumeSize.y) + 0.5f;
                    float zLocal = localPos.z; // distance along camera forward

                    // Check if vertex is inside camera projection and between near/far planes
                    if (u >= 0f && u <= 1f && vUV >= 0f && vUV <= 1f &&
                        zLocal >= cam.nearPlane && zLocal <= cam.farPlane)
                    {
                        covered = true;
                        break; // no need to check other cameras
                    }
                }

                // Set Gizmo color based on coverage
                Gizmos.color = covered ? Color.green : Color.red;
                Gizmos.DrawSphere(vertexPos, 0.005f * volumeSize.magnitude);
            }
        }
    }
    
    if (depthCameras == null || depthCameras.Count == 0) return;

    // Draw cameras and frustums
    foreach (var cam in depthCameras)
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(cam.position, 0.02f * volumeSize.magnitude);

        Quaternion camRot = Quaternion.Euler(cam.rotationEuler);
        Vector3 forward = camRot * Vector3.forward;
        Gizmos.DrawLine(cam.position, cam.position + forward * volumeSize.magnitude * 0.5f);

        // Draw simple frustum (optional, same as before)
        Vector3 camRight = camRot * Vector3.right * (volumeSize.x / 2f);
        Vector3 camUp = camRot * Vector3.up * (volumeSize.y / 2f);
        Vector3 camForward = forward * (cam.farPlane - cam.nearPlane);
        Vector3 nearCenter = cam.position + forward * cam.nearPlane;
        Vector3 farCenter = cam.position + forward * cam.farPlane;

        Vector3[] corners = new Vector3[8];
        corners[0] = nearCenter - camRight - camUp;
        corners[1] = nearCenter + camRight - camUp;
        corners[2] = nearCenter + camRight + camUp;
        corners[3] = nearCenter - camRight + camUp;
        corners[4] = farCenter - camRight - camUp;
        corners[5] = farCenter + camRight - camUp;
        corners[6] = farCenter + camRight + camUp;
        corners[7] = farCenter - camRight + camUp;

        for (int i = 0; i < 4; i++)
            Gizmos.DrawLine(corners[i], corners[i + 4]);

        Gizmos.DrawLine(corners[0], corners[1]);
        Gizmos.DrawLine(corners[1], corners[2]);
        Gizmos.DrawLine(corners[2], corners[3]);
        Gizmos.DrawLine(corners[3], corners[0]);
        Gizmos.DrawLine(corners[4], corners[5]);
        Gizmos.DrawLine(corners[5], corners[6]);
        Gizmos.DrawLine(corners[6], corners[7]);
        Gizmos.DrawLine(corners[7], corners[4]);
    }

    


    }

}


#if UNITY_EDITOR
#endif
