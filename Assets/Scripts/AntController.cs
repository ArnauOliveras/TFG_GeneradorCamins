using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AntController : MonoBehaviour
{
    public float speed = 100;
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
        transform.Translate(l_direction * speed * Time.deltaTime);
    }


    Vector3 NewDirection()
    {

        Vector3 direccionAlNido = (destination - transform.position).normalized;
        Vector3 mejorDireccion = direccionAlNido;
        float mejorPuntaje = float.MaxValue;

        int muestras = 10;
        for (int i = 0; i < muestras; i++)
        {

            float angulo = (i / (float)muestras) * 360f;
            Vector3 direccionPrueba = Quaternion.Euler(0, angulo, 0) * direccionAlNido;
            Vector3 testPos = Vector3.zero;

            for (int u = 1; u <= nextPosQuantity; u++)
            {
                nextPositions[u - 1] = transform.position + direccionPrueba * distanceNextPos * u;
            }

            bool l_noValid = false;

            foreach (Vector3 puntoPrueba in nextPositions)
            {
                if (NoValidSlope(puntoPrueba))
                {
                    l_noValid = true;
                    break;
                }

                testPos = puntoPrueba;
            }

            if (!l_noValid)
            {
                float distanciaAlNido = Vector3.Distance(testPos, destination);
                float puntaje = distanciaAlNido;

                print(puntaje);
                if (puntaje < mejorPuntaje)
                {
                    mejorPuntaje = puntaje;
                    mejorDireccion = direccionPrueba;
                }
            }


        }

        return mejorDireccion.normalized;
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
