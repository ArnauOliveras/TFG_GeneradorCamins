using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AntController : MonoBehaviour
{
    public float speed = 100;
    public float rotationSpeed = 50;
    public float maxSlope = 35;
    float distanceNextPos = 3;

    Vector3 destination;
    ACOManeger ACO_Maneger;

    Terrain terrain;
    Vector3 terrainPos;
    Vector3 terrainSize;

    public Vector3[] nextPositions;
    int nextPosQuantity = 5;


    Vector3 direction;
    Vector3 thisDirection;

    void Start()
    {
        ACO_Maneger = ACOManeger.Instance;

        destination = ACO_Maneger.GetDestinationVillage();
        terrain = ACO_Maneger.GetTerrain();

        terrainPos = terrain.GetPosition();
        terrainSize = terrain.terrainData.size;

        destination.y = terrain.SampleHeight(destination) + terrain.GetPosition().y;

        nextPositions = new Vector3[nextPosQuantity];

        direction = (destination - transform.position).normalized;
        thisDirection = direction;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    if (nextPositions != null)
    //    {
    //        foreach (Vector3 nextPos in nextPositions)
    //        {
    //            if (nextPos != null)
    //            {
    //                Gizmos.DrawSphere(nextPos, 1);
    //            }
    //        }
    //    }
    //}


    float timeGoToDestination = 1;
    float timerGTD = 0;

    void Update()
    {
        timerGTD -= Time.deltaTime;

        if (timerGTD <= 0)
        {
            direction = (destination - transform.position).normalized;
        }

        GoTo(direction);


        transform.position = new Vector3(transform.position.x, terrain.SampleHeight(transform.position) + terrain.GetPosition().y + 0.5f, transform.position.z);


    }

    void GoTo(Vector3 l_direction)
    {

        for (int i = 1; i <= nextPosQuantity; i++)
        {
            nextPositions[i - 1] = transform.position + l_direction * distanceNextPos * i;
        }

        foreach (Vector3 nextPos in nextPositions)
        {

            if (NoValidSlope(nextPos))
            {
                l_direction = NewDirection();
                timerGTD = timeGoToDestination;
                break;
            }

        }

        direction = l_direction;


        thisDirection = Vector3.MoveTowards(thisDirection, direction, rotationSpeed * Time.deltaTime);

        transform.Translate(thisDirection * speed * Time.deltaTime);
    }


    Vector3 NewDirection()
    {

        Vector3 directionToDestination = (destination - transform.position).normalized;
        Vector3 bestDirection = directionToDestination;
        float bestScore = float.MaxValue;

        int samples = 10;
        for (int i = 0; i < samples; i++)
        {

            float angle = (i / (float)samples) * 360f;
            Vector3 directionSample = Quaternion.Euler(0, angle, 0) * directionToDestination;
            Vector3 testPos = Vector3.zero;

            for (int u = 1; u <= nextPosQuantity; u++)
            {
                nextPositions[u - 1] = transform.position + directionSample * distanceNextPos * u;
            }

            bool l_noValid = false;

            foreach (Vector3 samplePos in nextPositions)
            {
                if (NoValidSlope(samplePos))
                {
                    l_noValid = true;
                    break;
                }

                testPos = samplePos;
            }

            if (!l_noValid) // If the direction score less, it's better to choose the path
            {
                float distanceToDestination = Vector3.Distance(testPos, destination);
                float score = distanceToDestination;

                print(score);
                if (score < bestScore)
                {
                    bestScore = score;
                    bestDirection = directionSample;
                }
            }


        }

        return bestDirection.normalized;
    }



    bool NoValidSlope(Vector3 l_nextPos)
    {
        Vector3 coordenadaNormalizada = new Vector3((l_nextPos.x - terrain.GetPosition().x) / terrain.terrainData.size.x, 0, (l_nextPos.z - terrain.GetPosition().z) / terrain.terrainData.size.z);
        Vector3 normal = terrain.terrainData.GetInterpolatedNormal(coordenadaNormalizada.x, coordenadaNormalizada.z);
        float actual_normal = Vector3.Angle(normal, Vector3.up);


        if (actual_normal <= maxSlope) 
        {
            return l_nextPos.x < terrainPos.x || l_nextPos.x > terrainPos.x + terrainSize.x || l_nextPos.z < terrainPos.z || l_nextPos.z > terrainPos.z + terrainSize.z;
        }
        return true;
    }

}
