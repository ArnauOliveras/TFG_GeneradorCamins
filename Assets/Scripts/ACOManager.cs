using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ACOManager : MonoBehaviour
{
    public List<ACOAnt> antsList = new List<ACOAnt>();
    public List<ACONodeGraph> actualNodeGraphist = new List<ACONodeGraph>();

    [Header("Bake Graph Settings")]
    public int distanceEachGraphNode = 5;
    [Range(0.0f, 90.0f)]
    public float maxSlope = 35.0f;
    [Range(0.0f, 90.0f)]
    public float recommendedSlope = 25.0f;

    [Header("General Settings")]
    public ACOVillage initialVillage;
    public ACOVillage destinationVillage;
    ACOGraph acoGraph;

    [HideInInspector]
    public int initialNode;
    [HideInInspector]
    public int destinationNode;

    [HideInInspector]
    public int m_MAXInteractions = 0;
    public int antSpawnNum = 1;

    [Header("Draw Settings")]
    public int drawAntDebug = 1;

    #region Graph_Creation
    public void ACOBakeGraph()
    {
        if (GetComponent<Terrain>() != null)
        {
            List<ACONodeGraph> l_nodeGraphList;

            l_nodeGraphList = new List<ACONodeGraph>();

            Terrain l_terrain = GetComponent<Terrain>();

            Vector3 l_trrainSize = l_terrain.terrainData.size;
            int l_numNodesX = (int)l_trrainSize.x + 1;
            int l_numNodesZ = (int)l_trrainSize.z + 1;

            int l_graphX = 0;
            int l_graphZ = 0;

            for (int x = 0; x < l_numNodesX; x += distanceEachGraphNode)
            {
                l_graphX++;
                for (int z = 0; z < l_numNodesZ; z += distanceEachGraphNode)
                {
                    l_graphZ++;
                    Vector3 l_Position = new Vector3(x, (l_terrain.SampleHeight(new Vector3(x, 0f, z)) + l_terrain.GetPosition().y), z);

                    Vector3 l_coordenadaNormalizada = new Vector3((l_Position.x - l_terrain.GetPosition().x) / l_terrain.terrainData.size.x, 0, (l_Position.z - l_terrain.GetPosition().z) / l_terrain.terrainData.size.z);
                    Vector3 l_normal = l_terrain.terrainData.GetInterpolatedNormal(l_coordenadaNormalizada.x, l_coordenadaNormalizada.z);
                    float l_actualNormal = Vector3.Angle(l_normal, Vector3.up);
                    l_nodeGraphList.Add(new ACONodeGraph(l_Position, l_actualNormal, l_nodeGraphList.Count, maxSlope));
                }
            }
            l_graphZ = l_graphZ / l_graphX;
            foreach (ACONodeGraph nodeGraph in l_nodeGraphList)
            {
                nodeGraph.SetNeighborNodes(l_nodeGraphList, l_graphZ, l_graphX);
            }

            Debug.Log("Graph dimentions: " + l_graphZ + " x " + l_graphX);
            Debug.Log("Graph Done With " + l_nodeGraphList.Count + " Nodes");

            actualNodeGraphist = new List<ACONodeGraph>();
            actualNodeGraphist = l_nodeGraphList;
        }
        else
            Debug.LogError("Terrain component not found in inspector.");
    }
    #endregion

    #region Create_Path
    public void CreatePathway()
    {

        if (initialVillage != null && destinationVillage != null)
        {
            Create();
            Debug.Log("Pathway Created Successfully");
        }
        else
            Debug.LogError("You Have To Instantiate The Villages In The Inspector");

    }

    private void Create()
    {
        ACOBakeGraph();
        StartCreate();
        FindInitialAndFinalNode();
        SpawnAnts();
        UpdateAnts();
        drawAntDebug = antsList.Count;
        DrawPathAnt();
        EndCreate();
    }

    private void EndCreate()
    {
    }

    private void StartCreate()
    {
        antsList = new List<ACOAnt>();
    }

    private void FindInitialAndFinalNode()
    {
        float closestDistanceDestination = Mathf.Infinity;
        float closestDistanceInitial = Mathf.Infinity;

        foreach (ACONodeGraph nodeGraph in actualNodeGraphist)
        {
            float distanceInitial = Vector3.Distance(nodeGraph.position, new Vector3(initialVillage.GetPosition().x, nodeGraph.position.y, initialVillage.GetPosition().z));

            if (distanceInitial < closestDistanceInitial)
            {
                closestDistanceInitial = distanceInitial;
                initialNode = nodeGraph.ID;
            }

            float distanceDestination = Vector3.Distance(nodeGraph.position, new Vector3(destinationVillage.GetPosition().x, nodeGraph.position.y, destinationVillage.GetPosition().z));

            if (distanceDestination < closestDistanceDestination)
            {
                closestDistanceDestination = distanceDestination;
                destinationNode = nodeGraph.ID;
            }
        }
        actualNodeGraphist[initialNode].initialNode = true;
        actualNodeGraphist[destinationNode].destinationNode = true;
    }

    private void SpawnAnts()
    {
        for (int i = 1; i <= antSpawnNum; i++)
        {
            antsList.Add(new ACOAnt(initialNode, this, i, GetComponent<ACOBlackboard>()));
        }
    }

    private void UpdateAnts()
    {
        int l_lostAnts = 0;
        int l_endAnts = 0;
        int l_MAXmultiplier = GetComponent<ACOBlackboard>().MAXMultiplier;

        foreach (ACOAnt ant in antsList)
        {
            while (!ant.UpdateAnt()) { }

            if (ant.findDestination)
            {
                l_endAnts++;
                //Debug.Log("Ant " + ant.antID + ": path created with " + ant.interactions + "interactions");
            }
            if (ant.lostAnt)
            {
                l_lostAnts++;
                //Debug.Log("Ant " + ant.antID + " is lost");
            }

            if (ant.antID == 2)
            {
                actualNodeGraphist.ForEach(n => n.pheromonesPower = 0);
                m_MAXInteractions = ant.interactions * l_MAXmultiplier;
                Debug.Log("Max Interactions to lost: " + m_MAXInteractions);
            }
            if (ant.antID == antSpawnNum - 1)
            {
                actualNodeGraphist.ForEach(n => n.pheromonesPower = 0);
            }



            if (!ant.lostAnt)
            {
                if (!ant.analyzingAnt)
                {
                    int i = m_MAXInteractions / l_MAXmultiplier;
                    for (int j = ant.nodeGraphList.Count - 1; j >= ant.nodeGraphList.Count - (m_MAXInteractions / l_MAXmultiplier) - 1; j--)
                    {
                        if (j < 0)
                            break;

                        actualNodeGraphist[ant.nodeGraphList[j].ID].pheromonesPower += ((float)i / ((float)m_MAXInteractions/ l_MAXmultiplier));
                        i--;
                    }
                }
                else
                {
                    int i = 0;
                    foreach (ACONodeGraph nodeGraph in ant.nodeGraphList)
                    {
                        i++;
                        actualNodeGraphist[nodeGraph.ID].pheromonesPower += ((float)i / (float)ant.nodeGraphList.Count);
                    }
                }
            }

        }

        Debug.Log("Lost ants: " + l_lostAnts + " / " + (antSpawnNum - 4));
        Debug.Log("Find Destination ants: " + (l_endAnts - 4) + " / " + (antSpawnNum - 4));

    }



    #endregion

    #region Draw
    public void DrawPathAnt()
    {
        if (antsList != null)
        {
            Debug.Log("Ant path draw: " + drawAntDebug);

            Vector3[] l_points = new Vector3[antsList[drawAntDebug - 1].nodeGraphList.Count];
            int i = 0;
            foreach (ACONodeGraph node in antsList[drawAntDebug - 1].nodeGraphList)
            {
                l_points[i] = new Vector3(node.position.x, node.position.y + 0.5f, node.position.z);
                i++;
            }
            LineRenderer l_lineRenderer;

            if (GetComponent<LineRenderer>() != null)
            {
                l_lineRenderer = gameObject.GetComponent<LineRenderer>();
                l_lineRenderer.enabled = true;
            }
            else
                l_lineRenderer = gameObject.AddComponent<LineRenderer>();

            l_lineRenderer.positionCount = l_points.Length;
            l_lineRenderer.SetPositions(l_points);
        }
        else
            Debug.LogError("You Have To Create Pathway");
    }


    public void DrawTexture()
    {
        Debug.LogError("Encara no està implementat");
    }
    #endregion

}

[CustomEditor(typeof(ACOManager))]
public class ACOManagerButtons : Editor
{
    public override void OnInspectorGUI()
    {
        ACOManager manager = (ACOManager)target;

        if (GUILayout.Button("Create Pathway"))
        {
            manager.CreatePathway();
        }
        if (GUILayout.Button("Draw Path Ant Selected"))
        {
            manager.DrawPathAnt();
        }
        if (GUILayout.Button("Draw Texture"))
        {
            manager.DrawTexture();
        }

        base.OnInspectorGUI();
    }
}