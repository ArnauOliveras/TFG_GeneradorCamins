using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(ACOGraph))]
public class ACOGraphButtons : Editor
{
    public override void OnInspectorGUI()
    {
        ACOGraph graph = (ACOGraph)target;

        base.OnInspectorGUI();

        if (GUILayout.Button("Show Graph"))
        {
            graph.ACOShowGraph();
        }
        if (GUILayout.Button("Hide Graph"))
        {
            graph.ACOHideGraph();
        }
        if (GUILayout.Button("Destroy This Graph"))
        {
            graph.DestroyThisGraph();
        }
    }
}
