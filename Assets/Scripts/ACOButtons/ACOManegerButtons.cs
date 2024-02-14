using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ACOManeger))]
public class ACOManegerButtons : Editor
{
    public override void OnInspectorGUI()
    {
        ACOManeger maneger = (ACOManeger)target;

        base.OnInspectorGUI();

        if (GUILayout.Button("Bake Grid"))
        {
            maneger.ACOBakeGrid();
        }
        if (GUILayout.Button("Create Pathway"))
        {
            maneger.CreatePathway();
        }
        if (GUILayout.Button("Draw Path Ant Selected"))
        {
            maneger.DrawPathAnt();
        }
        if (GUILayout.Button("Draw Texture"))
        {
            maneger.DrawTexture();
        }
    }
}
