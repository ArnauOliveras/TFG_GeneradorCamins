using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ACONodeGrid
{
    public int ID = -1;
    public Vector3 position;
    public float slopePoint;

    public List<int> neighborNodesGridsList = new List<int>();
    bool haveNeighbor = false;

    public bool initialNode = false;
    public bool destinationNode = false;


    public float pheromonesPower;

    float maxSlope;

    public ACONodeGrid(Vector3 l_position, float l_slope, int l_ID, float l_MaxSlope)
    {
        position = l_position;
        slopePoint = l_slope;
        ID = l_ID;
        pheromonesPower = 0;
        maxSlope = l_MaxSlope;
    }


    public void SetNeighborNodes(List<ACONodeGrid> l_gridNodes, int l_gridZ, int l_gridX)
    {
        if (!haveNeighbor)
        {
            List<ACONodeGrid> l_nodes = new List<ACONodeGrid>();
            for (int i = 0; i < 8; i++)
                l_nodes.Add(null);

            /*
                7   0   1
                6   @   2
                5   4   3
            */

            if (((float)ID + 1) % (float)l_gridX != 0)
            {
                l_nodes[0] = ID + 1 < l_gridNodes.Count ? l_gridNodes[ID + 1] : null;
                l_nodes[1] = ID + l_gridZ + 1 < l_gridNodes.Count ? l_gridNodes[ID + l_gridZ + 1] : null;
                l_nodes[7] = ID - l_gridZ + 1 >= 0 ? l_gridNodes[ID - l_gridZ + 1] : null;
            }

            if ((float)ID % (float)l_gridX != 0)
            {
                l_nodes[3] = ID + l_gridZ - 1 < l_gridNodes.Count ? l_gridNodes[ID + l_gridZ - 1] : null;
                l_nodes[4] = ID - 1 >= 0 ? l_gridNodes[ID - 1] : null;
                l_nodes[5] = ID - l_gridZ - 1 >= 0 ? l_gridNodes[ID - l_gridZ - 1] : null;
            }

            l_nodes[6] = ID - l_gridZ >= 0 ? l_gridNodes[ID - l_gridZ] : null;
            l_nodes[2] = ID + l_gridZ < l_gridNodes.Count ? l_gridNodes[ID + l_gridZ] : null;


            foreach (ACONodeGrid node in l_nodes)
            {
                if (node != null && node.slopePoint <= maxSlope)
                    neighborNodesGridsList.Add(node.ID);
                else
                    neighborNodesGridsList.Add(-1);
            }
            haveNeighbor = true;
        }
    }

}