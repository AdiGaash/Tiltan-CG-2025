using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDisplacement : MonoBehaviour
{
    public Renderer rend;
    public Transform collider;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rend.material.SetVector("_ColliderPosition", collider.transform.position);
        
    }
}
