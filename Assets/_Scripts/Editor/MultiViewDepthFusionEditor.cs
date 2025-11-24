using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MultiViewDepthFusion))]
public class MultiViewDepthFusionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // draw normal inspector

        MultiViewDepthFusion fusion = (MultiViewDepthFusion)target;
        if (GUILayout.Button("Generate Base Mesh"))
        {
            fusion.GenerateBaseMesh();
        }
    }
}