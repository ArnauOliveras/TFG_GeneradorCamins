using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntMaster : MonoBehaviour
{
    List<Pheromone> pheromones = new List<Pheromone>();
    ACOManeger ACO_Maneger;


    float speed = 2;
    float rotationSpeed = 1000;

    Vector3 destination;

    Terrain terrain;

    Vector3 direction;
    Vector3 thisDirection;




    void Start()
    {
        ACO_Maneger = ACOManeger.Instance;
        pheromones = ACO_Maneger.GetPheromones_P3();
        terrain = ACO_Maneger.GetTerrain();



        direction = SetDirection();
        thisDirection = direction;
    }


    private void Update()
    {
        
        if(Vector3.Distance(destination, transform.position) <= 5)
            direction = SetDirection();


        thisDirection = Vector3.MoveTowards(thisDirection, direction, rotationSpeed*1000);

        transform.Translate(thisDirection * speed);
        transform.position = new Vector3(transform.position.x, terrain.SampleHeight(transform.position) + terrain.GetPosition().y + 0.5f, transform.position.z);



        //if (Vector3.Distance(destination, transform.position) <= 5)
    }




    Vector3 SetDirection()
    {
        Pheromone phNearest = null;
        float min_dist = float.MaxValue;

        foreach(Pheromone ph in pheromones)
        {
            if(Vector3.Distance(transform.position,ph.position) < min_dist)
            {
                min_dist = Vector3.Distance(transform.position,ph.position);
                phNearest = ph;
            }
        }

        pheromones.RemoveAll(pheromones => pheromones == phNearest);

        destination = phNearest.position;
        return (phNearest.position - transform.position).normalized;

    }




}
