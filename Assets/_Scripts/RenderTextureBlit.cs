
using UnityEngine;

public class RenderTextureBlitDemo : MonoBehaviour
{
    [Header("Source & Output")]
    public Texture sourceTexture;     // Any texture (input)
    public Material processMaterial;  // Material with a shader that modifies pixels
    public Renderer targetRenderer;   // Mesh renderer to display the result

    private RenderTexture renderTexture;

    void Start()
    {
        // 1. Create a RenderTexture
        renderTexture = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGB32);
        renderTexture.Create();

        // 2. Initialize RenderTexture with source texture
        Graphics.Blit(sourceTexture, renderTexture);

        // 3. Assign RenderTexture to a material (URP: _BaseMap)
        if (targetRenderer != null)
        {
            targetRenderer.material.SetTexture("_BaseMap", renderTexture);
        }
    }

    void Update()
    {
        // 4. Every frame: process the RenderTexture using Blit
        Graphics.Blit(renderTexture, renderTexture, processMaterial);
    }

    void OnDestroy()
    {
        // Clean up
        if (renderTexture != null)
        {
            renderTexture.Release();
        }
    }
}
