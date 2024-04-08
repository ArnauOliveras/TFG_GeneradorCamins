using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class ACOAnt
{
    ACOManager acoManager;
    ACOBlackboard acoBlackboard;

    public int ID;
    public bool analyzingAnt = false;

    int actualNode;

    int availableNodes = 0;
    bool noWayFound = false;

    public List<ACONodeGraph> nodeGraphList = new List<ACONodeGraph>();
    public List<ACONodeGraph> neighborNodesGraphList = new List<ACONodeGraph>();

    public bool findDestination = false;
    public bool lostAnt = false;

    Vector3 position;
    public int interactions = 0;

    [Header("Initial Power")]
    public float initialPower;

    [Header("Power of pheromones")]
    public float powersNode1;
    public float powersNode2;
    public float powersNode3;

    [Header("Closer to the final destination")]
    public float nearestNode1;
    public float nearestNode2;
    public float nearestNode3;

    [Header("Slope favoritisms")]
    public float slopeFavoritismsREC;
    public float slopeFavoritismsMAX;

    [Header("Next Slope Similar")]
    public float similarNode1;
    public float similarNode2;
    public float similarNode3;
    public float similarNode4;
    public float similarNode5;
    public float similarNode6;
    public float similarNode7;
    public float similarNode8;

    [Header("MAX Multiplier")]
    public int MAXMultiplier;



    public ACOAnt(int l_initialNode, ACOManager l_manager, int l_ID, ACOBlackboard l_acoBlackboard)
    {
        actualNode = l_initialNode;
        acoManager = l_manager;
        ID = l_ID;
        nodeGraphList.Add(acoManager.actualNodeGraphist[l_initialNode]);
        position = acoManager.actualNodeGraphist[acoManager.initialNode].position;
        acoBlackboard = l_acoBlackboard;

        //The ants with IDs 1, 2, the penultimate, and the last one are analyzers.
        if (ID <= 2 || ID >= acoManager.antSpawnNum - 3)
            analyzingAnt = true;
    }
    public void StartAnt()
    {

        initialPower = acoBlackboard.initialPower;

        powersNode1 = acoBlackboard.powersNode1;
        powersNode2 = acoBlackboard.powersNode2;
        powersNode3 = acoBlackboard.powersNode3;

        nearestNode1 = acoBlackboard.nearestNode1;
        nearestNode2 = acoBlackboard.nearestNode2;
        nearestNode3 = acoBlackboard.nearestNode3;

        slopeFavoritismsREC = acoBlackboard.slopeFavoritismsREC;
        slopeFavoritismsMAX = acoBlackboard.slopeFavoritismsMAX;

        similarNode1 = acoBlackboard.similarNode1;
        similarNode2 = acoBlackboard.similarNode2;
        similarNode3 = acoBlackboard.similarNode3;
        similarNode4 = acoBlackboard.similarNode4;
        similarNode5 = acoBlackboard.similarNode5;
        similarNode6 = acoBlackboard.similarNode6;
        similarNode7 = acoBlackboard.similarNode7;
        similarNode8 = acoBlackboard.similarNode8;

        MAXMultiplier = acoBlackboard.MAXMultiplier;

        if (ID >= (acoManager.antSpawnNum / 2))
            powersNode1 += (ID - (acoManager.antSpawnNum / 2));
    }
    public bool UpdateAnt()
    {
        if (analyzingAnt)
            UpdateAnalyzeAnt();
        else
            UpdateNormalAnt();

        if (findDestination || lostAnt)
            return true;

        return false;
    }
    void SetActualNode(int l_NodeID)
    {
        actualNode = l_NodeID;
        position = acoManager.actualNodeGraphist[l_NodeID].position;
        ACONodeGraph l_findID = nodeGraphList.Find(find => find.ID == l_NodeID);

        if (l_findID != null)
            nodeGraphList.Remove(l_findID);

        nodeGraphList.Add(acoManager.actualNodeGraphist[l_NodeID]);

        interactions++;
        if (interactions == acoManager.m_MAXInteractions && !analyzingAnt)
            lostAnt = true;

        if ((interactions == acoManager.m_MAXInteractions * 5 && analyzingAnt) && !(ID <= 2))
        {
            Debug.LogError("Fail ant " + ID + " , MAX interactions: " + acoManager.m_MAXInteractions * 5);
            lostAnt = true;
        }

    }

    #region Normal_Ant
    private void UpdateNormalAnt()
    {
        SetNeighborNodes();
        AnalyzeNeighborNodes();
        if (findDestination)
            return;
        MoveNextNode();
    }

    private void SetNeighborNodes()
    {
        availableNodes = 0;
        neighborNodesGraphList.Clear();

        List<ACONodeGraph> l_NeighborNodesGraphListV2 = new List<ACONodeGraph>();
        int l_AvailableNodesV2 = 0;

        foreach (int neighborNode in acoManager.actualNodeGraphist[actualNode].neighborNodesGraphList)
        {
            if (neighborNode != -1)
            {
                if (!nodeGraphList.Contains(acoManager.actualNodeGraphist[neighborNode]))
                {
                    neighborNodesGraphList.Add(acoManager.actualNodeGraphist[neighborNode]);
                    availableNodes++;
                    noWayFound = false;
                }
                else
                {
                    l_NeighborNodesGraphListV2.Add(acoManager.actualNodeGraphist[neighborNode]);
                    l_AvailableNodesV2++;
                }

                if (acoManager.actualNodeGraphist[neighborNode].destinationNode)
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


    private void AnalyzeNeighborNodes()
    {
        foreach (ACONodeGraph neighborNode in neighborNodesGraphList)
        {
            /*if (noWayFound)
                neighborNode.probabilityToGoThisNode = 1.0f;
            else
                neighborNode.probabilityToGoThisNode = 0.0f;*/

            neighborNode.probabilityToGoThisNode = initialPower;
        }

        /*
         *  Things that don't depend if a path has been found.
         */
        AnalyzePheromonesPower();

        /*
         * Things that  depend if a path has been found.
         */
        if (!noWayFound)
        {
            AnalyzeCloserNodeToFinalDestination();
            AnalyzeNextSlopeSimilar();
            AnalyzeSlopeFavoritisms();
        }
    }

    private void AnalyzeNextSlopeSimilar()
    {
        //Next Slope Similar

        float l_closestDistance1 = Mathf.Infinity;
        float l_closestDistance2 = Mathf.Infinity;
        float l_closestDistance3 = Mathf.Infinity;
        float l_closestDistance4 = Mathf.Infinity;
        float l_closestDistance5 = Mathf.Infinity;
        float l_closestDistance6 = Mathf.Infinity;
        float l_closestDistance7 = Mathf.Infinity;
        float l_closestDistance8 = Mathf.Infinity;
        int l_SimilarNode1 = -1;
        int l_SimilarNode2 = -1;
        int l_SimilarNode3 = -1;
        int l_SimilarNode4 = -1;
        int l_SimilarNode5 = -1;
        int l_SimilarNode6 = -1;
        int l_SimilarNode7 = -1;
        int l_SimilarNode8 = -1;
        int i = 0;
        foreach (ACONodeGraph node in neighborNodesGraphList)
        {
            float l_distance = Mathf.Abs(node.position.y - acoManager.actualNodeGraphist[acoManager.destinationNode].position.y);
            if (l_distance < l_closestDistance1)
            {
                l_closestDistance1 = l_distance;
                l_SimilarNode1 = i;
            }
            else if (l_distance < l_closestDistance2)
            {
                l_closestDistance2 = l_distance;
                l_SimilarNode2 = i;
            }
            else if (l_distance < l_closestDistance3)
            {
                l_closestDistance3 = l_distance;
                l_SimilarNode3 = i;
            }
            else if (l_distance < l_closestDistance4)
            {
                l_closestDistance4 = l_distance;
                l_SimilarNode4 = i;
            }
            else if (l_distance < l_closestDistance5)
            {
                l_closestDistance5 = l_distance;
                l_SimilarNode5 = i;
            }
            else if (l_distance < l_closestDistance6)
            {
                l_closestDistance6 = l_distance;
                l_SimilarNode6 = i;
            }
            else if (l_distance < l_closestDistance7)
            {
                l_closestDistance7 = l_distance;
                l_SimilarNode7 = i;
            }
            else if (l_distance < l_closestDistance8)
            {
                l_closestDistance8 = l_distance;
                l_SimilarNode8 = i;
            }
            i++;
        }
        if (l_SimilarNode1 != -1)
            neighborNodesGraphList[l_SimilarNode1].probabilityToGoThisNode *= similarNode1;
        if (l_SimilarNode2 != -1)
            neighborNodesGraphList[l_SimilarNode2].probabilityToGoThisNode *= similarNode2;
        if (l_SimilarNode3 != -1)
            neighborNodesGraphList[l_SimilarNode3].probabilityToGoThisNode *= similarNode3;
        if (l_SimilarNode4 != -1)
            neighborNodesGraphList[l_SimilarNode3].probabilityToGoThisNode *= similarNode4;
        if (l_SimilarNode5 != -1)
            neighborNodesGraphList[l_SimilarNode3].probabilityToGoThisNode *= similarNode5;
        if (l_SimilarNode6 != -1)
            neighborNodesGraphList[l_SimilarNode3].probabilityToGoThisNode *= similarNode6;
        if (l_SimilarNode7 != -1)
            neighborNodesGraphList[l_SimilarNode3].probabilityToGoThisNode *= similarNode7;
        if (l_SimilarNode8 != -1)
            neighborNodesGraphList[l_SimilarNode3].probabilityToGoThisNode *= similarNode8;

    }

    private void AnalyzeSlopeFavoritisms()
    {
        //Slope favoritisms

        //Vector3 l_ActualNode = acoManager.actualNodeGraphist[actualNode].position;
        int i = 0;
        foreach (ACONodeGraph node in neighborNodesGraphList)
        {
            //float l_slope = Mathf.Abs(((node.position.y - l_ActualNode.y) / (Vector3.Distance(l_ActualNode, node.position))) * 100f);

            if (neighborNodesGraphList[i].probabilityToGoThisNode != 0)
            {
                if (node.slopePoint <= acoManager.recommendedSlope)
                    neighborNodesGraphList[i].probabilityToGoThisNode *= slopeFavoritismsREC;
                else if (node.slopePoint <= acoManager.maxSlope)
                {
                    neighborNodesGraphList[i].probabilityToGoThisNode *= slopeFavoritismsMAX;
                    if (neighborNodesGraphList[i].probabilityToGoThisNode < 0)
                        neighborNodesGraphList[i].probabilityToGoThisNode = 0;
                }
            }
            i++;
        }
    }

    private void AnalyzeCloserNodeToFinalDestination()
    {
        //Closer to the final destination

        float l_closestDistance1 = Mathf.Infinity;
        float l_closestDistance2 = Mathf.Infinity;
        float l_closestDistance3 = Mathf.Infinity;
        int l_nearestNode1 = -1;
        int l_nearestNode2 = -1;
        int l_nearestNode3 = -1;
        int i = 0;
        foreach (ACONodeGraph node in neighborNodesGraphList)
        {
            float l_distance = Vector3.Distance(node.position, acoManager.actualNodeGraphist[acoManager.destinationNode].position);
            if (l_distance < l_closestDistance1)
            {
                l_closestDistance1 = l_distance;
                l_nearestNode1 = i;
            }
            else if (l_distance < l_closestDistance2)
            {
                l_closestDistance2 = l_distance;
                l_nearestNode2 = i;
            }
            else if (l_distance < l_closestDistance3)
            {
                l_closestDistance3 = l_distance;
                l_nearestNode3 = i;
            }
            i++;
        }
        if (l_nearestNode1 != -1)
            neighborNodesGraphList[l_nearestNode1].probabilityToGoThisNode += nearestNode1;
        if (l_nearestNode2 != -1)
            neighborNodesGraphList[l_nearestNode2].probabilityToGoThisNode += nearestNode2;
        if (l_nearestNode3 != -1)
            neighborNodesGraphList[l_nearestNode3].probabilityToGoThisNode += nearestNode3;
    }

    private void AnalyzePheromonesPower()
    {
        // power of pheromones

        float l_pheromonesPower1 = 0;
        float l_pheromonesPower2 = 0;
        float l_pheromonesPower3 = 0;
        int l_powersNode1 = -1;
        int l_powersNode2 = -1;
        int l_powersNode3 = -1;
        int i = 0;
        foreach (ACONodeGraph node in neighborNodesGraphList)
        {
            float l_power = node.pheromonesPower;
            if (l_power > l_pheromonesPower1)
            {
                l_pheromonesPower1 = l_power;
                l_powersNode1 = i;
            }
            else if (l_power > l_pheromonesPower2)
            {
                l_pheromonesPower2 = l_power;
                l_powersNode2 = i;
            }
            else if (l_power > l_pheromonesPower3)
            {
                l_pheromonesPower3 = l_power;
                l_powersNode3 = i;
            }
            i++;
        }
        if (l_powersNode1 != -1)
            neighborNodesGraphList[l_powersNode1].probabilityToGoThisNode += powersNode1;
        if (l_powersNode2 != -1)
            neighborNodesGraphList[l_powersNode2].probabilityToGoThisNode += powersNode2;
        if (l_powersNode3 != -1)
            neighborNodesGraphList[l_powersNode3].probabilityToGoThisNode += powersNode3;
    }

    private void MoveNextNode()
    {
        float l_ActualSumProbability = 0;
        neighborNodesGraphList.ForEach(n => l_ActualSumProbability += n.probabilityToGoThisNode);
        neighborNodesGraphList.ForEach(n => n.probabilityToGoThisNode = n.probabilityToGoThisNode / l_ActualSumProbability);

        float randomNumber = Random.Range(0.0f, 1.0f);
        float cumulativeProbability = 0f;

        for (int i = 0; i < neighborNodesGraphList.Count; i++)
        {
            cumulativeProbability += neighborNodesGraphList[i].probabilityToGoThisNode;

            if (randomNumber <= cumulativeProbability)
            {
                SetActualNode(neighborNodesGraphList[i].ID);
                return;
            }

        }
    }

    #endregion

    #region Analyzing_Ant
    private void UpdateAnalyzeAnt()
    {
        AnalyzeNeighborNodesFirst();
        if (findDestination)
            return;
        MoveNextNodeFirst();
    }
    private void MoveNextNodeFirst()
    {
        float l_pheromonesPower = 0;
        ACONodeGraph l_powersNode = null;
        if (ID != 1)
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
                int l_nearestNode = acoManager.actualNodeGraphist[actualNode].ID;
                foreach (var node in neighborNodesGraphList)
                {
                    float l_distance = Vector3.Distance(node.position, acoManager.actualNodeGraphist[acoManager.destinationNode].position);
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
    private void AnalyzeNeighborNodesFirst()
    {
        availableNodes = 0;
        neighborNodesGraphList.Clear();

        List<ACONodeGraph> l_NeighborNodesGraphListV2 = new List<ACONodeGraph>();
        int l_AvailableNodesV2 = 0;

        foreach (int neighborNode in acoManager.actualNodeGraphist[actualNode].neighborNodesGraphList)
        {
            if (neighborNode != -1)
            {
                if (!nodeGraphList.Contains(acoManager.actualNodeGraphist[neighborNode]))
                {
                    neighborNodesGraphList.Add(acoManager.actualNodeGraphist[neighborNode]);
                    availableNodes++;
                    noWayFound = false;
                }
                else
                {
                    l_NeighborNodesGraphListV2.Add(acoManager.actualNodeGraphist[neighborNode]);
                    l_AvailableNodesV2++;
                }
                if (acoManager.actualNodeGraphist[neighborNode].destinationNode)
                {
                    findDestination = true;
                    SetActualNode(neighborNode);
                    return;
                }
            }
        }
        if (availableNodes == 0 && l_AvailableNodesV2 != 0)
        {
            noWayFound = true;
            neighborNodesGraphList = l_NeighborNodesGraphListV2;
            availableNodes = l_AvailableNodesV2;
        }
    }
    #endregion
}
