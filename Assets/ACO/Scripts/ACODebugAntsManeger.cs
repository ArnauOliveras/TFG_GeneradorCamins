using System.Collections.Generic;
using UnityEngine;

public class ACODebugAntsManeger : MonoBehaviour
{
    public ACOManager manager;
    public GameObject perfabDebugAnt;
    List<ACOAnt> antsList = new List<ACOAnt>();
    List<ACODebugAnts> debugAntsList = new List<ACODebugAnts>();
    public float antSpeed;
    public int maxAnts = 200;

    void Start()
    {


    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            antsList = manager.antsList;
            int i = 0;
            foreach (ACOAnt ant in antsList)
            {
                GameObject l_ThisAnt = Instantiate(perfabDebugAnt, ant.nodeGraphList[0].position, Quaternion.identity);
                l_ThisAnt.transform.parent = transform;
                l_ThisAnt.name = "Ant " + ant.ID;
                ACODebugAnts l_Ant = l_ThisAnt.GetComponent<ACODebugAnts>();
                l_Ant.nodeGraphList = ant.nodeGraphList;
                l_Ant.speed = antSpeed;
                debugAntsList.Add(l_Ant);
                i++;
                if (i > maxAnts)
                    return;
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            foreach (Transform child in transform)
            {
                debugAntsList.Clear();
                Destroy(child.gameObject);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (ACODebugAnts ant in debugAntsList)
            {
                if (ant.speed == 0)
                    ant.speed = antSpeed;
                else
                    ant.speed = 0;
            }
        }


    }

}
