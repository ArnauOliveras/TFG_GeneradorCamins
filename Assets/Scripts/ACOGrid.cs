using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;


public class ACOGrid : MonoBehaviour
{
    public static List<ACONodeGrid> nodeGridsList; 

    bool showGuizmos = false; 

    private void OnDrawGizmos()
    {
        if (showGuizmos)
        {
            foreach (ACONodeGrid nodeGrid in nodeGridsList)
            {
                if (nodeGrid.slopePoint < 25f)
                    Gizmos.color = Color.cyan;
                else
                    Gizmos.color = Color.yellow;
                
                if (nodeGrid.initialNode)
                    Gizmos.color = Color.black;
                if (nodeGrid.destinationNode)
                    Gizmos.color = Color.black;

                Gizmos.DrawSphere(nodeGrid.position, 1);
            }
        }
    }

    public List<ACONodeGrid> GetACOGrid()
    {
        return nodeGridsList;
    }
    public void SetACOGrid(List<ACONodeGrid> grid)
    {
        nodeGridsList = grid;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        int i = 0;

        foreach (ACONodeGrid nodeGrid in nodeGridsList)
        {
            vertices.Add(nodeGrid.position);
            triangles.Add(1);
            i++;
        }
    }

    public void ACOShowGrid()
    {
        showGuizmos = true;
        Debug.Log("Show Grid");
    }

    public void ACOHideGrid()
    {
        showGuizmos = false;
        Debug.Log("Hide Grid");
    }
    public void DestroyThisGrid()
    {
        DestroyImmediate(this);
    }
}
