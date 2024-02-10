using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ACOGridTerrainSettings))]
public class ACOGridSettingsButtons : Editor
{
    public override void OnInspectorGUI()
    {
        ACOGridTerrainSettings gridTerrainSettings = (ACOGridTerrainSettings)target;

        base.OnInspectorGUI();

        if (GUILayout.Button("Bake Grid"))
        {
            gridTerrainSettings.ACOBakeGrid();
        }
    }
}
