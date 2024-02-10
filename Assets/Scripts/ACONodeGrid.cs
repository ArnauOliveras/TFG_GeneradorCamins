using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ACONodeGrid
{
    public Vector3 position;
    public float slopePoint;

    public List<ACONodeGrid> neighborNodesGridsList = new List<ACONodeGrid>();
    bool haveNeighbor = false;

    public bool initialNode = false;
    public bool destinationNode = false;

    List<ACONodeGrid> nodeGridsList = new List<ACONodeGrid>();
    float distanceEachGridNode;

    public ACONodeGrid(Vector3 l_position, float l_slope)
    {
        position = l_position;
        slopePoint = l_slope;
    }

    public void SetACOGrid(List<ACONodeGrid> l_grid, float l_distanceEachGridNode)
    {
        nodeGridsList = l_grid;
        distanceEachGridNode = l_distanceEachGridNode;

    }

    public void SetNeighborNodes()
    {
        if (!haveNeighbor)
        {
            ACONodeGrid l_node1 = nodeGridsList.Find(nd => Vector3.Distance(nd.position, new Vector3(position.x + distanceEachGridNode, nd.position.y, position.z)) < 0.5f);
            ACONodeGrid l_node2 = nodeGridsList.Find(nd => Vector3.Distance(nd.position, new Vector3(position.x - distanceEachGridNode, nd.position.y, position.z)) < 0.5f);
            ACONodeGrid l_node3 = nodeGridsList.Find(nd => Vector3.Distance(nd.position, new Vector3(position.x, nd.position.y, position.z + distanceEachGridNode)) < 0.5f);
            ACONodeGrid l_node4 = nodeGridsList.Find(nd => Vector3.Distance(nd.position, new Vector3(position.x, nd.position.y, position.z - distanceEachGridNode)) < 0.5f);

            ACONodeGrid l_node5 = nodeGridsList.Find(nd => Vector3.Distance(nd.position, new Vector3(position.x + distanceEachGridNode, nd.position.y, position.z + distanceEachGridNode)) < 0.5f);
            ACONodeGrid l_node6 = nodeGridsList.Find(nd => Vector3.Distance(nd.position, new Vector3(position.x - distanceEachGridNode, nd.position.y, position.z - distanceEachGridNode)) < 0.5f);
            ACONodeGrid l_node7 = nodeGridsList.Find(nd => Vector3.Distance(nd.position, new Vector3(position.x - distanceEachGridNode, nd.position.y, position.z + distanceEachGridNode)) < 0.5f);
            ACONodeGrid l_node8 = nodeGridsList.Find(nd => Vector3.Distance(nd.position, new Vector3(position.x + distanceEachGridNode, nd.position.y, position.z - distanceEachGridNode)) < 0.5f);

            neighborNodesGridsList.Add(l_node1);
            neighborNodesGridsList.Add(l_node2);
            neighborNodesGridsList.Add(l_node3);
            neighborNodesGridsList.Add(l_node4);
            neighborNodesGridsList.Add(l_node5);
            neighborNodesGridsList.Add(l_node6);
            neighborNodesGridsList.Add(l_node7);
            neighborNodesGridsList.Add(l_node8);

            haveNeighbor = true;
        }
    }

}
