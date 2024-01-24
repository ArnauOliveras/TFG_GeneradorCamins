using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ACOManeger : MonoBehaviour
{
    public static ACOManeger Instance;
    public GameObject Ant;
    public GameObject AntMaster;

    public Transform initVillage;
    public Transform destVillage;
    public Terrain terrain;

    public float timeTest = 30;

    [Header("Ant Blackboard")]
    public float speed = 100;
    public float rotationSpeed = 50;
    public float maxSlope = 35;
    public float distanceNextPos = 3;
    public int nextPosQuantity = 5;
    public int samplesDirections = 10;
    public int MAXpheromones = 20;
    public float distancePheromones = 10;
    public float distanceToDetectPheromones = 20;

    [Header("PheromonesController")]
    List<Pheromone> pheromones_ant1 = new List<Pheromone>();
    List<Pheromone> pheromones_ant2 = new List<Pheromone>();
    List<Pheromone> pheromones_P2 = new List<Pheromone>();
    List<Pheromone> pheromones_P3 = new List<Pheromone>();
    int pheromonesCount = 0;

    public bool test_P1_On = true;
    public bool test_P2_On = false;
    public bool test_P3_On = false;


    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        if (terrain == null) { terrain = Terrain.activeTerrain; }
    }

    void Start()
    {
        pheromones_ant1 = new List<Pheromone>();
        StartCoroutine(SpawnAnt());
        StartCoroutine(EndPart1());
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (pheromones_ant1 != null)
        {
            foreach (Pheromone pheromone in pheromones_ant1)
            {
                if (pheromone != null)
                {
                    Gizmos.DrawSphere(pheromone.position, 1);
                }
            }
        }
        Gizmos.color = Color.cyan;
        if (pheromones_ant2 != null)
        {
            foreach (Pheromone pheromone in pheromones_ant2)
            {
                if (pheromone != null)
                {
                    Gizmos.DrawSphere(pheromone.position, 1);
                }
            }
        }


        Gizmos.color = Color.yellow;
        if (pheromones_P2 != null)
        {
            foreach (Pheromone pheromone in pheromones_P2)
            {
                if (pheromone != null)
                {
                    Gizmos.DrawSphere(pheromone.position, 1);
                }
            }
        }

        Gizmos.color = Color.black;
        if (pheromones_P3 != null)
        {
            foreach (Pheromone pheromone in pheromones_P3)
            {
                if (pheromone != null)
                {
                    Gizmos.DrawSphere(pheromone.position, 1);
                }
            }
        }
    }




    void Update()
    {
        //print("Pheromones: " + pheromones.Count);
    }

    IEnumerator EndPart1()
    {
        yield return new WaitForSeconds(timeTest);
        test_P1_On = false;
        StartCoroutine(StartPart2());
    }

    IEnumerator StartPart2()
    {
        yield return new WaitForSeconds(0.5f);
        test_P2_On = true;
        AnalyzePheromones();
    }

    #region Part 1

    IEnumerator SpawnAnt()
    {
        yield return new WaitForSeconds(0.3f);
        if (test_P1_On)
        {
            GameObject ant1 = Instantiate(Ant, initVillage.position, Quaternion.identity);
            ant1.GetComponent<AntController>().ant1 = true;
            //GameObject ant2 = Instantiate(Ant, destVillage.position, Quaternion.identity);
            //ant2.GetComponent<AntController>().ant1 = false;
            StartCoroutine(SpawnAnt());
        }
    }


    public Vector3 GetDestinationVillage(bool dest)
    {
        if (dest)
        {
            return destVillage.position;
        }
        return initVillage.position;
    }

    public Terrain GetTerrain()
    {
        return terrain;
    }

    public void AddPheromone(Vector3 pos, GameObject ant, bool ant1)
    {
        Pheromone ph = new Pheromone(pos, ant, pheromonesCount);
        if (ant1)
        {
            pheromones_ant1.Add(ph);
        }
        else
        {
            pheromones_ant2.Add(ph);
        }
        pheromonesCount++;
        //StartCoroutine(TimePheromone(ph));
    }


    IEnumerator TimePheromone(Pheromone ph)
    {
        yield return new WaitForSeconds(20);
        if (test_P1_On)
        {
            RemovePheromones(ph);
        }
    }


    public void RemovePheromones(GameObject ant)
    {
        if (ant.GetComponent<AntController>().ant1)
        {
            pheromones_ant1.RemoveAll(pheromone => pheromone.ant == ant);
        }
        else
        {
            pheromones_ant2.RemoveAll(pheromone => pheromone.ant == ant);
        }

    }


    public List<Pheromone> GetPheromoneList(bool ant1)
    {
        if (ant1)
        {
            return pheromones_ant1;
        }
        return pheromones_ant2;
    }

    #endregion



    #region Part 2

    void AnalyzePheromones()
    {

        foreach (Pheromone ph in pheromones_ant1)
        {
            int counterPheromones = 0;

            foreach (Pheromone l_ph in pheromones_ant1)
            {
                if (Vector3.Distance(ph.position, l_ph.position) <= 5)
                {
                    counterPheromones++;
                }
            }

            if (counterPheromones <= 5)
            {
                ph.position = Vector3.zero;
            }

        }

        foreach (Pheromone ph in pheromones_ant1)
        {
            int counterPheromones = 0;

            foreach (Pheromone l_ph in pheromones_ant1)
            {
                if (Vector3.Distance(ph.position, l_ph.position) <= 20)
                {
                    counterPheromones++;
                }
            }

            if (counterPheromones <= 20)
            {
                ph.position = Vector3.zero;
            }
            else
            {
                if (ph.position != Vector3.zero)
                {
                    pheromones_P2.Add(ph);
                }
            }

        }
        pheromones_ant1.Clear();


        foreach (Pheromone ph in pheromones_P2)
        {
            int counterPheromones = 0;

            foreach (Pheromone l_ph in pheromones_P2)
            {
                if (Vector3.Distance(ph.position, l_ph.position) <= 10)
                {
                    counterPheromones++;
                }
            }

            if (counterPheromones > 1)
            {
                ph.position = Vector3.zero;
            }
            else
            {
                if (ph.position != Vector3.zero)
                {
                    pheromones_P3.Add(ph);
                }
            }
        }

        pheromones_P2.Clear();










        //////////////////////////////////////////////////////////////////////////////
        ///

        foreach (Pheromone ph in pheromones_ant2)
        {
            int counterPheromones = 0;

            foreach (Pheromone l_ph in pheromones_ant2)
            {
                if (Vector3.Distance(ph.position, l_ph.position) <= 5)
                {
                    counterPheromones++;
                }
            }

            if (counterPheromones <= 5)
            {
                ph.position = Vector3.zero;
            }

        }

        foreach (Pheromone ph in pheromones_ant2)
        {
            int counterPheromones = 0;

            foreach (Pheromone l_ph in pheromones_ant2)
            {
                if (Vector3.Distance(ph.position, l_ph.position) <= 20)
                {
                    counterPheromones++;
                }
            }

            if (counterPheromones <= 10)
            {
                ph.position = Vector3.zero;
            }
            else
            {
                if (ph.position != Vector3.zero)
                {
                    pheromones_P2.Add(ph);
                }
            }

        }
        pheromones_ant2.Clear();



        StartCoroutine(Part_03());

    }





    #endregion


    #region Part 3

    IEnumerator Part_03()
    {

        yield return new WaitForSeconds(0.5f);
        test_P2_On = false;
        test_P3_On = true;

        Instantiate(AntMaster, initVillage.position, Quaternion.identity);
    }


    public List<Pheromone> GetPheromones_P3()
    {
        return pheromones_P3;
    }

    #endregion

    public void RemovePheromones(Pheromone l_ph)
    {


        foreach (Pheromone ph in pheromones_ant1)
        {
            if (ph == l_ph)
            {
                if (l_ph.ant != null)
                {
                    l_ph.ant.GetComponent<AntController>().pheromones--;
                }
                pheromones_ant1.Remove(ph);
                break;
            }
        }
    }
}
