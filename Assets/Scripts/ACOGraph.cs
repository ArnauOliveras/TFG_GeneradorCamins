using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;


public class ACOGraph : MonoBehaviour
{
    public List<ACONodeGraph> nodeGraphList; 

    public List<ACONodeGraph> GetACOGraph()
    {
        return nodeGraphList;
    }
    public void SetACOGraph(List<ACONodeGraph> graph)
    {
        nodeGraphList = graph;
    }

    public void ACOShowGraph()
    {
        showGuizmos = true;
        Debug.Log("Show Graph");
    }

    public void ACOHideGraph()
    {
        showGuizmos = false;
        Debug.Log("Hide Graph");
    }
    public void DestroyThisGraph()
    {
        DestroyImmediate(this);
    }













    bool showGuizmos = false; 
    private void OnDrawGizmos()
    {
        if (showGuizmos)
        {
            foreach (ACONodeGraph nodeGraph in nodeGraphList)
            {
                if (nodeGraph.slopePoint < 35f)
                    Gizmos.color = Color.cyan;
                else
                    Gizmos.color = Color.red;

                if (nodeGraph.initialNode)
                    Gizmos.color = Color.black;
                if (nodeGraph.destinationNode)
                    Gizmos.color = Color.black;

                Gizmos.DrawWireSphere(nodeGraph.position, 1);
            }
        }
    }
}

[CustomEditor(typeof(ACOGraph))]
public class ACOGraphButtons : Editor
{
    public override void OnInspectorGUI()
    {
        ACOGraph graph = (ACOGraph)target;

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
        base.OnInspectorGUI();
    }
}