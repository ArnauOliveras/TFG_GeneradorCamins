using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pheromone
{
    public Vector3 position = Vector3.zero;
    public GameObject ant = null;
    public int ID = 0;

    public Pheromone(Vector3 l_position, GameObject l_ant, int l_ID)
    {
        position = l_position;
        ant = l_ant;
        ID = l_ID;
    }


    

}
