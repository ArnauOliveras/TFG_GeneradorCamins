using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ACOGridTerrainSettings : MonoBehaviour
{
    List<ACONodeGrid> nodeGridsList;
    Terrain terrain;
    public int distanceEachGridNode = 10;

    [Range(0.0f, 90.0f)]
    public float MaxSlope = 35f;


    public void ACOBakeGrid()
    {
        if (GetComponent<Terrain>() != null)
        {
            if (nodeGridsList != null)
                nodeGridsList.Clear();
            else
                nodeGridsList = new List<ACONodeGrid>();

            terrain = GetComponent<Terrain>();

            Vector3 trrainSize = terrain.terrainData.size;
            int numNodesX = (int)trrainSize.x + 1;
            int numNodesZ = (int)trrainSize.z + 1;

            for (int x = 0; x < numNodesX; x += distanceEachGridNode)
            {
                for (int z = 0; z < numNodesZ; z += distanceEachGridNode)
                {
                    Vector3 l_Position = new Vector3(x, (terrain.SampleHeight(new Vector3(x, 0f, z)) + terrain.GetPosition().y), z);

                    Vector3 coordenadaNormalizada = new Vector3((l_Position.x - terrain.GetPosition().x) / terrain.terrainData.size.x, 0, (l_Position.z - terrain.GetPosition().z) / terrain.terrainData.size.z);
                    Vector3 normal = terrain.terrainData.GetInterpolatedNormal(coordenadaNormalizada.x, coordenadaNormalizada.z);
                    float actual_normal = Vector3.Angle(normal, Vector3.up);
                    if (actual_normal <= MaxSlope)
                        nodeGridsList.Add(new ACONodeGrid(l_Position, actual_normal));
                }
            }

            foreach (ACONodeGrid nodeGrid in nodeGridsList)
            {
                nodeGrid.SetACOGrid(nodeGridsList, distanceEachGridNode);
                nodeGrid.SetNeighborNodes();
            }

            if (GetComponent<ACOGrid>() != null)
                DestroyImmediate(GetComponent<ACOGrid>());

            ACOGrid acoGrid = gameObject.AddComponent<ACOGrid>();
            acoGrid.SetACOGrid(nodeGridsList);

            Debug.Log("Grid Done With " + nodeGridsList.Count + " Nodes");

        }
        else
            Debug.LogError("Terrain component not found.");
    }
}
