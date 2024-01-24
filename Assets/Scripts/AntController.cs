using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Random = System.Random;

public class AntController : MonoBehaviour
{
    float speed = 100;
    float rotationSpeed = 50;
    float maxSlope = 35;
    float distanceNextPos = 3;

    Vector3 destination;
    ACOManeger ACO_Maneger;

    Terrain terrain;
    Vector3 terrainPos;
    Vector3 terrainSize;

    Vector3[] nextPositions;
    int nextPosQuantity = 5;
    int samplesDirections = 10;


    Vector3 direction;
    Vector3 thisDirection;

    int MAXpheromones;
    [HideInInspector]
    public int pheromones = 0;

    Vector3 posLastPheromone = Vector3.zero;
    float distancePheromones;
    float distanceToDetectPheromones;

    Random random = new Random();

    bool endPheromones = false;

    [HideInInspector]
    public bool ant1 = true;

    void Start()
    {
        ACO_Maneger = ACOManeger.Instance;
        SetParameters();
        StartCoroutine(FindPath());
    }

    private void SetParameters()
    {
        destination = ACO_Maneger.GetDestinationVillage(ant1);
        terrain = ACO_Maneger.GetTerrain();


        terrainPos = terrain.GetPosition();
        terrainSize = terrain.terrainData.size;

        destination.y = terrain.SampleHeight(destination) + terrain.GetPosition().y;

        nextPositions = new Vector3[nextPosQuantity];

        direction = (destination - transform.position).normalized;
        thisDirection = direction;


        speed = ACO_Maneger.speed;
        rotationSpeed = ACO_Maneger.rotationSpeed;
        maxSlope = ACO_Maneger.maxSlope;
        distanceNextPos = ACO_Maneger.distanceNextPos;
        nextPosQuantity = ACO_Maneger.nextPosQuantity;
        samplesDirections = ACO_Maneger.samplesDirections;
        MAXpheromones = ACO_Maneger.MAXpheromones;
        distancePheromones = ACO_Maneger.distancePheromones;
        distanceToDetectPheromones = ACO_Maneger.distanceToDetectPheromones;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawCube(transform.position, new Vector3(10, 10, 10));
    }




    void Update()
    {


        if (ACO_Maneger.test_P2_On)
        {
            for (int i = 0; i <= pheromones; i++)
            {
                foreach (Pheromone ph in ACO_Maneger.GetPheromoneList(ant1))
                {

                    if (ph.ant == gameObject)
                    {
                        ACO_Maneger.RemovePheromones(gameObject);
                        break;
                    }
                }
            }

            Destroy(gameObject);
        }

        if (ACO_Maneger.test_P1_On)
        {
            if (Vector3.Distance(transform.position, destination) > 10) // Condition to arrive
            {
                GoTo(direction);

                if (Vector3.Distance(transform.position, posLastPheromone) > distancePheromones) AddPheromone();
            }
            else if (!endPheromones)
            {
                endPheromones = true;
                int i = 1;
                foreach (Pheromone ph in ACO_Maneger.GetPheromoneList(ant1))
                {

                    if (ph.ant == gameObject)
                    {
                        i++;
                        ph.ID = int.MaxValue - i;
                    }
                }
                Destroy(gameObject);
            }
        }


    }



    IEnumerator FindPath()
    {
        float i = random.Next();
        yield return new WaitForSeconds(i);
        direction = NewDirection();

        StartCoroutine(FindPath());
    }




    private void AddPheromone()
    {
        pheromones++;

        if (pheromones > MAXpheromones)
        {
            //ACO_Maneger.RemovePheromones(gameObject);
            //pheromones--;
        }

        ACO_Maneger.AddPheromone(transform.position, gameObject, ant1);

        posLastPheromone = transform.position;
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

                break;
            }

        }

        direction = l_direction;


        thisDirection = Vector3.MoveTowards(thisDirection, direction, rotationSpeed);

        transform.Translate(thisDirection * speed);
        transform.position = new Vector3(transform.position.x, terrain.SampleHeight(transform.position) + terrain.GetPosition().y + 0.5f, transform.position.z);
    }


    Vector3 NewDirection()
    {
        Vector3 directionPh = GetPherormoneDirection();
        Vector3 directionPhRv = GetPherormoneDirectionReverse();
        Vector3 directionNe = GetNearestDirectionToDestination();

        if (directionPh != Vector3.zero)
        {
            if (random.NextDouble() < 0.1f)
            {
                //if(directionPhRv != Vector3.zero)
                //{
                //    if (random.NextDouble() < 0.40f)
                //        return directionPh;
                //    else
                //        return directionPhRv;
                //}

                return directionPh;
            }
            else
            {
                return directionNe;
            }
        }
        return directionNe;



    }

    Vector3 GetNearestDirectionToDestination()
    {
        Vector3 directionToDestination = (destination - transform.position).normalized;
        Vector3 bestDirection = directionToDestination;
        float bestScore = float.MaxValue;

        int samples = samplesDirections;
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

                if (score < bestScore)
                {
                    bestScore = score;
                    bestDirection = directionSample.normalized;
                }
            }


        }

        return bestDirection;
    }

    Vector3 GetPherormoneDirection()
    {
        int max_id = 0;
        Vector3 bestPheromoneDirection = Vector3.zero;

        foreach (Pheromone ph in ACO_Maneger.GetPheromoneList(ant1))
        {
            if (ph.ant != gameObject && Vector3.Distance(transform.position, ph.position) <= distanceToDetectPheromones && ph.ID > max_id && NoValidSlope((ph.position - transform.position).normalized * distanceNextPos))
            {
                max_id = ph.ID;
                bestPheromoneDirection = (ph.position - transform.position).normalized;
            }
        }

        return bestPheromoneDirection;
    }
    Vector3 GetPherormoneDirectionReverse()
    {
        int min_id = int.MaxValue;
        Vector3 bestPheromoneDirection = Vector3.zero;

        foreach (Pheromone ph in ACO_Maneger.GetPheromoneList(!ant1))
        {
            if (ph.ant != gameObject && Vector3.Distance(transform.position, ph.position) <= distanceToDetectPheromones && ph.ID < min_id && NoValidSlope((ph.position - transform.position).normalized * distanceNextPos))
            {
                min_id = ph.ID;
                bestPheromoneDirection = (ph.position - transform.position).normalized;
            }
        }

        return bestPheromoneDirection;
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
