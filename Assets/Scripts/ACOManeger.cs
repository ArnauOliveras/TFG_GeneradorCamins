using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(ACOGridTerrainSettings))]
public class ACOManeger : MonoBehaviour
{
    List<ACOAnt> antsList = new List<ACOAnt>();
    List<ACONodeGrid> nodeGridsList = new List<ACONodeGrid>();

    public ACOVillage initialVillage;
    public ACOVillage destinationVillage;

    ACONodeGrid initialNode;
    [HideInInspector]
    public ACONodeGrid destinationNode;

    bool canGizmos = false;

    public int drawAntDebug = 1;

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

        nodeGridsList = GetComponent<ACOGrid>().GetACOGrid();
        FindInitialAndFinalNode();
        SpawnAnts();
        UpdateAnts();
        canGizmos = true;
    }

    private void UpdateAnts()
    {

        foreach (ACOAnt ant in antsList)
        {
            while (!ant.UpdateAnt()) { }
            Debug.Log("Path created with " + ant.nodeGridsList.Count + "interactions");
        }
    }

    private void SpawnAnts()
    {
        antsList.Clear();
        antsList.Add(new ACOAnt(initialNode, this));
    }

    private void FindInitialAndFinalNode()
    {
        float closestDistanceDestination = Mathf.Infinity;
        float closestDistanceInitial = Mathf.Infinity;

        foreach (ACONodeGrid nodeGrid in nodeGridsList)
        {
            float distanceInitial = Vector3.Distance(nodeGrid.position, new Vector3(initialVillage.GetPosition().x, nodeGrid.position.y, initialVillage.GetPosition().z));

            if (distanceInitial < closestDistanceInitial)
            {
                closestDistanceInitial = distanceInitial;
                initialNode = nodeGrid;
            }

            float distanceDestination = Vector3.Distance(nodeGrid.position, new Vector3(destinationVillage.GetPosition().x, nodeGrid.position.y, destinationVillage.GetPosition().z));

            if (distanceDestination < closestDistanceDestination)
            {
                closestDistanceDestination = distanceDestination;
                destinationNode = nodeGrid;
            }
        }

        initialNode.initialNode = true;
        destinationNode.destinationNode = true;
    }

    public void DrawPathAnt()
    {
        if (antsList != null)
        {

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





}
