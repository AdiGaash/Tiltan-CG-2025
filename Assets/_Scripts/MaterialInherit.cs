using UnityEngine;

public class MaterialInherit : MonoBehaviour
{
    public Material parentMaterial; // Assign in inspector

    void Start()
    {
        // Create a new instance and copy all properties from parent
        Material newMat = new Material(parentMaterial);
        GetComponent<Renderer>().material = newMat;
    }
}