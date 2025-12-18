using UnityEngine;
using System.Collections.Generic;

public class UVIslandOutlineGizmo : MonoBehaviour
{
    public float gizmoScale = 1f;
    public Vector3 offset = Vector3.zero;
    public Color outlineColor = Color.yellow;

    // Controls how precise UV comparison is
    public float uvPrecision = 10000f;

    void OnDrawGizmos()
    {
        // TODO: Get the mesh (MeshFilter or SkinnedMeshRenderer)
        Mesh mesh = null;

        // TODO: Get UVs and triangles from the mesh
        Vector2[] uvs = null;
        int[] triangles = null;
        
        Gizmos.color = outlineColor;

        // logic to find boundary edges in UV space
        
    }

    

    Vector2 QuantizeUV(Vector2 uv)
    {
        return new Vector2( Mathf.Round(uv.x * uvPrecision), Mathf.Round(uv.y * uvPrecision) );
    }

    //void ParseAndDrawEdge(string key)
    //{
        //Gizmos.DrawLine(UVToWorld(a), UVToWorld(b));
    //}

    Vector3 UVToWorld(Vector2 uv)
    {
        return transform.position + offset + new Vector3(uv.x, uv.y, 0) * gizmoScale;
    }

    Vector2 ParseUV(string s)
    {
        // TODO: Parse a string into a Vector2
        return Vector2.zero;
    }
}