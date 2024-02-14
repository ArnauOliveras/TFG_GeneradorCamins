using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ACOGridTerrainSettings : MonoBehaviour
{

    Terrain terrain;
    public int distanceEachGridNode = 10;

    [Range(0.0f, 90.0f)]
    public float maxSlope = 35.0f;

    public void ACOBakeGrid()
    {
        if (GetComponent<Terrain>() != null)
        {
            List<ACONodeGrid> l_nodeGridsList;

            l_nodeGridsList = new List<ACONodeGrid>();

            terrain = GetComponent<Terrain>();

            Vector3 l_trrainSize = terrain.terrainData.size;
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
                    Vector3 l_Position = new Vector3(x, (terrain.SampleHeight(new Vector3(x, 0f, z)) + terrain.GetPosition().y), z);

                    Vector3 l_coordenadaNormalizada = new Vector3((l_Position.x - terrain.GetPosition().x) / terrain.terrainData.size.x, 0, (l_Position.z - terrain.GetPosition().z) / terrain.terrainData.size.z);
                    Vector3 l_normal = terrain.terrainData.GetInterpolatedNormal(l_coordenadaNormalizada.x, l_coordenadaNormalizada.z);
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
}
