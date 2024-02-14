using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;


public class ACOGrid : MonoBehaviour
{
    public List<ACONodeGrid> nodeGridsList; 

    public List<ACONodeGrid> GetACOGrid()
    {
        return nodeGridsList;
    }
    public void SetACOGrid(List<ACONodeGrid> grid)
    {
        nodeGridsList = grid;
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













    bool showGuizmos = false; 
    private void OnDrawGizmos()
    {
        if (showGuizmos)
        {
            foreach (ACONodeGrid nodeGrid in nodeGridsList)
            {
                if (nodeGrid.slopePoint < 35f)
                    Gizmos.color = Color.cyan;
                else
                    Gizmos.color = Color.red;

                if (nodeGrid.initialNode)
                    Gizmos.color = Color.black;
                if (nodeGrid.destinationNode)
                    Gizmos.color = Color.black;

                Gizmos.DrawWireSphere(nodeGrid.position, 1);
            }
        }
    }
}
