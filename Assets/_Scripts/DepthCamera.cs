using UnityEngine;

[System.Serializable] // Allows it to show in the Inspector
public class DepthCamera
{
    public Texture2D depthMap;      // The depth map texture
    public Vector3 position;        // Camera position in world space
    public Vector3 rotationEuler;   // Camera rotation in Euler angles (degrees)

    // Optional: you can add near/far planes per camera
    public float nearPlane = 0.1f;
    public float farPlane = 10f;
}