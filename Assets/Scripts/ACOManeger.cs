using System;
using System.Collections.Generic;
using UnityEngine;

public class ACOManeger : MonoBehaviour
{
    List<ACOAnt> antsList = new List<ACOAnt>();
    public List<ACONodeGrid> actualNodeGridsList = new List<ACONodeGrid>();

    [Header("Bake Settings")]
    public int distanceEachGridNode = 10;
    [Range(0.0f, 90.0f)]
    public float maxSlope = 35.0f;

    [Header("General Settings")]
    public ACOVillage initialVillage;
    public ACOVillage destinationVillage;

    int initialNode;
    [HideInInspector] 
    public int destinationNode;

    [Header("Draw Settings")]
    [Range(1, 3)]
    public int drawAntDebug = 3;
    int antSpawnNum = 3;

    public void ACOBakeGrid()
    {
        if (GetComponent<Terrain>() != null)
        {
            List<ACONodeGrid> l_nodeGridsList;

            l_nodeGridsList = new List<ACONodeGrid>();

            Terrain l_terrain = GetComponent<Terrain>();

            Vector3 l_trrainSize = l_terrain.terrainData.size;
            int l_numNodesX = (int)l_trrainSize.x + 1;
            int l_numNodesZ = (int)l_trrainSize.z + 1;

            int l_gridX = 0;
            int l_gridZ = 0;

            for (int x = 0; x < l_numNodesX; x += distanceEachGridNode)
            {
                l_gridX++;
                for (int z = 0; z < l_numNodesZ; z += distanceEachGridNode)
                {
                    l_gridZ++;
                    Vector3 l_Position = new Vector3(x, (l_terrain.SampleHeight(new Vector3(x, 0f, z)) + l_terrain.GetPosition().y), z);

                    Vector3 l_coordenadaNormalizada = new Vector3((l_Position.x - l_terrain.GetPosition().x) / l_terrain.terrainData.size.x, 0, (l_Position.z - l_terrain.GetPosition().z) / l_terrain.terrainData.size.z);
                    Vector3 l_normal = l_terrain.terrainData.GetInterpolatedNormal(l_coordenadaNormalizada.x, l_coordenadaNormalizada.z);
                    float l_actualNormal = Vector3.Angle(l_normal, Vector3.up);
                    l_nodeGridsList.Add(new ACONodeGrid(l_Position, l_actualNormal, l_nodeGridsList.Count, maxSlope));
                }
            }
            l_gridZ = l_gridZ / l_gridX;
            foreach (ACONodeGrid nodeGrid in l_nodeGridsList)
            {
                nodeGrid.SetNeighborNodes(l_nodeGridsList, l_gridZ, l_gridX);
            }

            if (GetComponent<ACOGrid>() != null)
                DestroyImmediate(GetComponent<ACOGrid>());

            Debug.Log("Grid dimentions: " + l_gridZ + " x " + l_gridX);
            Debug.Log("Grid Done With " + l_nodeGridsList.Count + " Nodes");

            ACOGrid l_acoGrid = gameObject.AddComponent<ACOGrid>();
            l_acoGrid.SetACOGrid(l_nodeGridsList);


        }
        else
            Debug.LogError("Terrain component not found in inspector.");
    }


    public void CreatePathway()
    {
        if (GetComponent<ACOGrid>() != null)
        {
            if (initialVillage != null && destinationVillage != null)
            {
                Create();
                Debug.Log("Pathway Created Successfully");
            }
            else
                Debug.LogError("You Have To Instantiate The Villages In The Inspector");
        }
        else
            Debug.LogError("You Have To Bake A Grid");
    }

    private void Create()
    {
        actualNodeGridsList = GetComponent<ACOGrid>().nodeGridsList;
        FindInitialAndFinalNode();
        SpawnAnts();
        UpdateAnts();
        drawAntDebug = antsList.Count;
        DrawPathAnt();
    }

    private void UpdateAnts()
    {

        foreach (ACOAnt ant in antsList)
        {
            while (!ant.UpdateAnt()) { }
            Debug.Log("Ant " + ant.antID + ": path created with " + ant.nodeGridsList.Count + "interactions");

            int i = 0;
            foreach (ACONodeGrid nodeGrid in ant.nodeGridsList)
            {
                i++;
                actualNodeGridsList[nodeGrid.ID].pheromonesPower += ((float)i / (float)ant.nodeGridsList.Count) * 1000;
                //Debug.Log("ID: " + nodeGrid.ID + " Power: " + actualNodeGridsList[nodeGrid.ID].pheromonesPower);
            }
        }
    }

    private void SpawnAnts()
    {
        antsList.Clear();
        for (int i = 1; i <= antSpawnNum; i++)
            antsList.Add(new ACOAnt(initialNode, this, i));
    }

    private void FindInitialAndFinalNode()
    {
        float closestDistanceDestination = Mathf.Infinity;
        float closestDistanceInitial = Mathf.Infinity;

        foreach (ACONodeGrid nodeGrid in actualNodeGridsList)
        {
            float distanceInitial = Vector3.Distance(nodeGrid.position, new Vector3(initialVillage.GetPosition().x, nodeGrid.position.y, initialVillage.GetPosition().z));

            if (distanceInitial < closestDistanceInitial)
            {
                closestDistanceInitial = distanceInitial;
                initialNode = nodeGrid.ID;
            }

            float distanceDestination = Vector3.Distance(nodeGrid.position, new Vector3(destinationVillage.GetPosition().x, nodeGrid.position.y, destinationVillage.GetPosition().z));

            if (distanceDestination < closestDistanceDestination)
            {
                closestDistanceDestination = distanceDestination;
                destinationNode = nodeGrid.ID;
            }
        }

        actualNodeGridsList[initialNode].initialNode = true;
        actualNodeGridsList[destinationNode].destinationNode = true;
    }

    public void DrawPathAnt()
    {
        if (antsList != null)
        {
            Debug.Log("Ant path draw: " + drawAntDebug);

            Vector3[] l_points = new Vector3[antsList[drawAntDebug - 1].nodeGridsList.Count];
            int i = 0;
            foreach (ACONodeGrid node in antsList[drawAntDebug - 1].nodeGridsList)
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


}
