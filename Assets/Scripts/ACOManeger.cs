using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACOManeger : MonoBehaviour
{
    public static ACOManeger Instance;

    public Transform destVillage;
    Vector3 destVillageV3;

    public Terrain terrain;


    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        destVillageV3 = destVillage.position;
        if (terrain == null) {terrain = Terrain.activeTerrain; }
    }

    void Start()
    {
    }

    
    void Update()
    {
        
    }

    public Vector3 GetDestinationVillage()
    {
        return destVillageV3;
    }
    
    public Terrain GetTerrain()
    {
        return terrain;
    }



}
