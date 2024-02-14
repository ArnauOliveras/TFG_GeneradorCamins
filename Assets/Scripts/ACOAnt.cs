using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ACOAnt
{
    ACOManeger acoManeger;

    public int antID;
    int actualNode;

    int availableNodes = 0;
    bool noWayFound = false;

    public List<ACONodeGrid> nodeGridsList = new List<ACONodeGrid>();
    public List<ACONodeGrid> neighborNodesGridsList = new List<ACONodeGrid>();

    bool findDestination = false;

    public ACOAnt(int l_initialNode, ACOManeger l_maneger, int l_ID)
    {
        actualNode = l_initialNode;
        acoManeger = l_maneger;
        antID = l_ID;
    }
    public bool UpdateAnt()
    {
        AnalyzeNeighborNodes();

        if (findDestination)
            return true;

        MoveNextNode();

        return false;
    }

    private void MoveNextNode()
    {
        float l_pheromonesPower = 0;
        ACONodeGrid l_powersNode = null;
        if (antID != 1)
        {
            foreach (ACONodeGrid node in neighborNodesGridsList)
            {
                float l_power = node.pheromonesPower;
                if (l_power > l_pheromonesPower)
                {
                    l_pheromonesPower = l_power;
                    l_powersNode = node;
                }
            }
        }

        if (l_powersNode != null)
        {
            actualNode = l_powersNode.ID;
            if(!nodeGridsList.Contains(l_powersNode))
                nodeGridsList.Add(l_powersNode);
        }
        else
        {
            if (noWayFound)
            {
                int l_randomNumber = Random.Range(0, availableNodes);
                actualNode = neighborNodesGridsList[l_randomNumber].ID;
                if (!nodeGridsList.Contains(neighborNodesGridsList[l_randomNumber]))
                    nodeGridsList.Add(neighborNodesGridsList[l_randomNumber]);
            }
            else
            {
                float l_closestDistance = Mathf.Infinity;
                int l_nearestNode = acoManeger.actualNodeGridsList[actualNode].ID;
                foreach (var node in neighborNodesGridsList)
                {
                    float l_distance = Vector3.Distance(node.position, acoManeger.actualNodeGridsList[acoManeger.destinationNode].position);
                    if (l_distance < l_closestDistance)
                    {
                        l_closestDistance = l_distance;
                        l_nearestNode = node.ID;
                    }
                }
                actualNode = l_nearestNode;
                if (!nodeGridsList.Contains(acoManeger.actualNodeGridsList[l_nearestNode]))
                    nodeGridsList.Add(acoManeger.actualNodeGridsList[l_nearestNode]);
            }
        }
    }
    private void AnalyzeNeighborNodes()
    {
        availableNodes = 0;
        neighborNodesGridsList.Clear();

        foreach (int neighborNode in acoManeger.actualNodeGridsList[actualNode].neighborNodesGridsList)
        {
            if (neighborNode != -1 /*&& acoManeger.actualNodeGridsList[neighborNode].slopePoint <= acoManeger.MaxSlope*/)
            {
                if (!nodeGridsList.Contains(acoManeger.actualNodeGridsList[neighborNode]))
                {
                    neighborNodesGridsList.Add(acoManeger.actualNodeGridsList[neighborNode]);
                    availableNodes++;
                    noWayFound = false;
                }
                if (acoManeger.actualNodeGridsList[neighborNode].destinationNode)
                {
                    findDestination = true;
                    actualNode = neighborNode;
                    if (!nodeGridsList.Contains(acoManeger.actualNodeGridsList[neighborNode]))
                        nodeGridsList.Add(acoManeger.actualNodeGridsList[neighborNode]);
                    return;
                }
            }
        }
        if (availableNodes == 0)
        {
            noWayFound = true;
            foreach (var neighborNode in acoManeger.actualNodeGridsList[actualNode].neighborNodesGridsList)
            {
                if (neighborNode != -1/* && acoManeger.actualNodeGridsList[neighborNode].slopePoint <= acoManeger.MaxSlope*/)
                {
                    neighborNodesGridsList.Add(acoManeger.actualNodeGridsList[neighborNode]);
                    availableNodes++;

                }
            }
        }
    }
}
