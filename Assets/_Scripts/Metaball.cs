using UnityEngine;
using UnityEngine.InputSystem;

public class Metaball : MonoBehaviour
{
    [Header("Metaball Settings")]
    [Range(0.1f, 2f)]
    public float intensity = 0.8f;
    
    [Range(0.5f, 5f)]
    public float radius = 2f;
    
    [Header("Control Settings")]
    public bool followMouse = true;
    public bool autoUpdate = true;
    
    private Vector2 position;
    private MarchingSquares marchingSquares;
    
    void Start()
    {
        // Find the MarchingSquares component to update
        marchingSquares = FindObjectOfType<MarchingSquares>();
        
        if (followMouse)
        {
            position = GetMouseWorldPosition();
        }
        else
        {
            position = new Vector2(transform.position.x, transform.position.y);
        }
    }
    
    void Update()
    {
        if (!autoUpdate) return;
        
        Vector2 newPosition = followMouse ? GetMouseWorldPosition() : 
                             new Vector2(transform.position.x, transform.position.y);
        
        if (Vector2.Distance(newPosition, position) > 0.01f)
        {
            position = newPosition;
            
            // Update the marching squares field
            if (marchingSquares != null)
            {
                marchingSquares.UpdateField();
            }
        }
    }
    
    
    
    
    
    public float CalculateInfluence(Vector2 point)
    {
        float distance = Vector2.Distance(point, position);
        
        // Smooth metaball falloff using exponential decay
        return Mathf.Exp(-distance * distance * intensity);
    }
    
    Vector2 GetMouseWorldPosition()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        
        Vector3 world = Camera.main.ScreenToWorldPoint(
            new Vector3(mouseScreen.x, mouseScreen.y, 0f));
            
        return new Vector2(world.x, world.y);
    }
    
   
}