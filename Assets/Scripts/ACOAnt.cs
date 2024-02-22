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

    public List<ACONodeGraph> nodeGraphList = new List<ACONodeGraph>();
    public List<ACONodeGraph> neighborNodesGraphList = new List<ACONodeGraph>();

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
        ACONodeGraph l_powersNode = null;
        if (antID != 1)
        {
            foreach (ACONodeGraph node in neighborNodesGraphList)
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
            SetActualNode(l_powersNode.ID);
        else
        {
            if (noWayFound)
            {
                int l_randomNumber = Random.Range(0, availableNodes);
                SetActualNode(neighborNodesGraphList[l_randomNumber].ID);
            }
            else
            {
                float l_closestDistance = Mathf.Infinity;
                int l_nearestNode = acoManeger.actualNodeGraphist[actualNode].ID;
                foreach (var node in neighborNodesGraphList)
                {
                    float l_distance = Vector3.Distance(node.position, acoManeger.actualNodeGraphist[acoManeger.destinationNode].position);
                    if (l_distance < l_closestDistance)
                    {
                        l_closestDistance = l_distance;
                        l_nearestNode = node.ID;
                    }
                }
                SetActualNode(l_nearestNode);
            }
        }
    }
    private void AnalyzeNeighborNodes()
    {
        availableNodes = 0;
        neighborNodesGraphList.Clear();

        List<ACONodeGraph> l_NeighborNodesGraphListV2 = new List<ACONodeGraph>();
        int l_AvailableNodesV2 = 0;

        foreach (int neighborNode in acoManeger.actualNodeGraphist[actualNode].neighborNodesGraphList)
        {
            if (neighborNode != -1 /*&& acoManeger.actualNodeGraphList[neighborNode].slopePoint <= acoManeger.MaxSlope*/)
            {
                if (!nodeGraphList.Contains(acoManeger.actualNodeGraphist[neighborNode]))
                {
                    neighborNodesGraphList.Add(acoManeger.actualNodeGraphist[neighborNode]);
                    availableNodes++;
                    noWayFound = false;
                }
                else
                {
                    l_NeighborNodesGraphListV2.Add(acoManeger.actualNodeGraphist[neighborNode]);
                    l_AvailableNodesV2++;
                }
                if (acoManeger.actualNodeGraphist[neighborNode].destinationNode)
                {
                    findDestination = true;
                    SetActualNode(neighborNode);
                    return;
                }
            }
        }
        if (availableNodes == 0)
        {
            noWayFound = true;
            neighborNodesGraphList = l_NeighborNodesGraphListV2;
            availableNodes = l_AvailableNodesV2;
        }
    }
    void SetActualNode(int l_NodeID)
    {
        actualNode = l_NodeID;
        ACONodeGraph l_findID = nodeGraphList.Find(find => find.ID == l_NodeID);

        if (l_findID != null) 
            nodeGraphList.Remove(l_findID);

        nodeGraphList.Add(acoManeger.actualNodeGraphist[l_NodeID]);

    }
}
