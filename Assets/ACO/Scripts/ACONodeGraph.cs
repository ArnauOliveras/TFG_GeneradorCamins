using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ACONodeGraph
{
    public int ID = -1;
    public Vector3 position = Vector3.zero;
    public float slopePoint;

    public List<int> neighborNodesGraphList = new List<int>();
    public List<int> neighborNeighborNodesGraphList = new List<int>();
    bool haveNeighbor = false;

    public bool initialNode = false;
    public bool destinationNode = false;

    public float pheromonesPower;

    float maxSlope;

    public float probabilityToGoThisNode = 0;

    public ACONodeGraph(Vector3 l_position, float l_slope, int l_ID, float l_MaxSlope)
    {
        position = l_position;
        slopePoint = l_slope;
        ID = l_ID;
        pheromonesPower = 0;
        maxSlope = l_MaxSlope;
    }


    public void SetNeighborNodes(List<ACONodeGraph> l_graphNodes, int l_graphZ, int l_graphX)
    {
        if (!haveNeighbor)
        {
            List<ACONodeGraph> l_nodes = new List<ACONodeGraph>();
            for (int i = 0; i < 8; i++)
                l_nodes.Add(null);

            /*
                7   0   1
                6   @   2
                5   4   3
            */

            if (((float)ID + 1) % (float)l_graphX != 0)
            {
                l_nodes[0] = ID + 1 < l_graphNodes.Count ? l_graphNodes[ID + 1] : null;
                l_nodes[1] = ID + l_graphZ + 1 < l_graphNodes.Count ? l_graphNodes[ID + l_graphZ + 1] : null;
                l_nodes[7] = ID - l_graphZ + 1 >= 0 ? l_graphNodes[ID - l_graphZ + 1] : null;
            }

            if ((float)ID % (float)l_graphX != 0)
            {
                l_nodes[3] = ID + l_graphZ - 1 < l_graphNodes.Count ? l_graphNodes[ID + l_graphZ - 1] : null;
                l_nodes[4] = ID - 1 >= 0 ? l_graphNodes[ID - 1] : null;
                l_nodes[5] = ID - l_graphZ - 1 >= 0 ? l_graphNodes[ID - l_graphZ - 1] : null;
            }

            l_nodes[6] = ID - l_graphZ >= 0 ? l_graphNodes[ID - l_graphZ] : null;
            l_nodes[2] = ID + l_graphZ < l_graphNodes.Count ? l_graphNodes[ID + l_graphZ] : null;


            foreach (ACONodeGraph node in l_nodes)
            {
                if (node != null && node.slopePoint <= maxSlope)
                    neighborNodesGraphList.Add(node.ID);
                else
                    neighborNodesGraphList.Add(-1);
            }
            haveNeighbor = true;
            neighborNeighborNodesGraphList = neighborNodesGraphList;
        }
    }

    /*public void SetNeighborNeighborNodes(List<ACONodeGraph> acoNodes)
    {
        for (int i = 0; i < 3; ++i)
        {
            List<int> l_IDNodes = new List<int>();
            foreach (int node in neighborNeighborNodesGraphList)
            {
                if (node != -1)
                {
                    foreach (int n in acoNodes[node].neighborNodesGraphList)
                    {
                        if (!neighborNeighborNodesGraphList.Contains(n) && n != ID)
                            l_IDNodes.Add(n);
                    }
                }

            }
            if (l_IDNodes.Count > 0)
            {
                foreach (int n in l_IDNodes)
                {
                    neighborNeighborNodesGraphList.Add(n);
                }
            }
        }
        if (ID == 2000)
            Debug.Log(neighborNeighborNodesGraphList.Count);
    }*/
}