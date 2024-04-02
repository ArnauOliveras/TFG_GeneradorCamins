using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACODebugAnts : MonoBehaviour
{
    public List<ACONodeGraph> nodeGraphList = new List<ACONodeGraph>();
    public float speed = 10;
    int i = 1;
    float j = 1;

    void Update()
    {
        if (i + 1 >= nodeGraphList.Count)
            Destroy(gameObject);

        transform.position = nodeGraphList[i].position;

        j += speed * Time.deltaTime;
        i = (int)j;


    }
}
