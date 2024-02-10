using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(ACOGrid))]
public class ACOGridButtons : Editor
{
    public override void OnInspectorGUI()
    {
        ACOGrid grid = (ACOGrid)target;

        base.OnInspectorGUI();

        if (GUILayout.Button("Show Grid"))
        {
            grid.ACOShowGrid();
        }
        if (GUILayout.Button("Hide Grid"))
        {
            grid.ACOHideGrid();
        }
        if (GUILayout.Button("Destroy This Grid"))
        {
            grid.DestroyThisGrid();
        }
    }
}
