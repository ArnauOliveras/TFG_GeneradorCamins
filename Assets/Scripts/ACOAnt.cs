using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ACOAnt
{
    ACONodeGrid actualNode;
    public List<ACONodeGrid> nodeGridsList = new List<ACONodeGrid>();
    int availableNodes = 0;
    public List<ACONodeGrid> neighborNodesGridsList = new List<ACONodeGrid>();
    bool noWayFound = false;
    ACOManeger maneger;
    bool findDestination = false;

    public ACOAnt(ACONodeGrid l_initialNode, ACOManeger l_maneger)
    {
        actualNode = l_initialNode;
        maneger = l_maneger;
    }
    public bool UpdateAnt()
    {
        AnalyzeNeighborNodes();
        if(findDestination)
        {
            return true;
        }

        if (noWayFound)
        {
            int l_randomNumber = Random.Range(0, availableNodes);
            actualNode = neighborNodesGridsList[l_randomNumber];
            nodeGridsList.Add(neighborNodesGridsList[l_randomNumber]);
        }
        else
        {
            float l_closestDistance = Mathf.Infinity;
            ACONodeGrid l_nearestNode = actualNode;
            foreach (var node in neighborNodesGridsList)
            {
                float l_distance = Vector3.Distance(node.position, maneger.destinationNode.position);
                if (l_distance < l_closestDistance)
                {
                    l_closestDistance = l_distance;
                    l_nearestNode = node;
                }
            }
            actualNode = l_nearestNode;
            nodeGridsList.Add(l_nearestNode);
        }
        return false;
    }

    private void AnalyzeNeighborNodes()
    {
        availableNodes = 0;
        neighborNodesGridsList.Clear();
        //actualNode.SetNeighborNodes();

        foreach (var neighborNode in actualNode.neighborNodesGridsList)
        {
            if (neighborNode != null)
            {
                if (!nodeGridsList.Contains(neighborNode))
                {
                    neighborNodesGridsList.Add(neighborNode);
                    availableNodes++;
                    noWayFound = false;
                }
                if (neighborNode.destinationNode)
                {
                    findDestination = true;
                    actualNode = neighborNode;
                    nodeGridsList.Add(neighborNode);
                    return;
                }
            }

        }
        if (availableNodes == 0)
        {
            noWayFound = true;
            foreach (var neighborNode in actualNode.neighborNodesGridsList)
            {
                if (neighborNode != null)
                {
                    neighborNodesGridsList.Add(neighborNode);
                    availableNodes++;

                }
            }
        }

    }
}
