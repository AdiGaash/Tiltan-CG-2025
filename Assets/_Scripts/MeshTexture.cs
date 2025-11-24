using UnityEngine;

public class MeshTexturer : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Texture2D colorTexture; // Albedo / main texture

    void Start()
    {
        if(meshRenderer == null || colorTexture == null)
        {
            Debug.LogError("Assign MeshRenderer and Texture!");
            return;
        }

        ApplyTexture();
    }

    void ApplyTexture()
    {
        Material mat = new Material(Shader.Find("HDRP/Lit")); // Or URP/Lit
        mat.mainTexture = colorTexture;
        meshRenderer.material = mat;
    }
}